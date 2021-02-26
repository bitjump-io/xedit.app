// FileHandle API declarations see https://developer.mozilla.org/en-US/docs/Web/API/File_Handle_API 
// and https://searchfox.org/mozilla-central/source/dom/indexedDB/IDBMutableFile.h

interface IDBDatabaseFirefox extends IDBDatabase {
    createMutableFile(name: string, type?: string): IDBRequest //IDBFileRequest
}

interface IDBMutableFile { // previously called FileHandle
  name: string;
  type: string;
  // mode can be 'readwrite' or 'readonly'
  open(mode: string): IDBFileHandle;
  // abort and error events.
}

// Also called LockedFile.
interface IDBFileHandle {
  // The FileHandle object from which the lock was opened.
  fileHandle: IDBMutableFile;
  // The mode for accessing the file; can be readonly or readwrite.
  mode: string;
  // A flag indicating if the file can be accessed (true) or not (false).
  active: boolean;
  // The location property is a zero-based index representing the position of the read/write pointer within the file. Its value indicates at which bytes within the file any write or read operation will start.
  // This value is changed automatically after every read and write operation. The special value null means end-of-file.
  // This property can be changed at will.
  location: number;
  getMetadata(): FileRequest;
  readAsArrayBuffer(size: number): FileRequest 
  readAsText(size: number): FileRequest
  // write some data in the file starting at the location offset. 
  write(data: string | ArrayBuffer | ArrayBufferView | Blob): FileRequest
  append(data: string | ArrayBuffer | ArrayBufferView | Blob): FileRequest
  truncate(): FileRequest
  flush(): FileRequest
  abort(): void;
  // The complete event is triggered each time a read or write operation is successful.
  // The abort event is triggered each time the abort() method is called.
  // The error event is triggered each time something goes wrong.
}

interface FileRequest {
  onsuccess(): void;
  onerror(): void;
  // onprogress
  // lockedFile 
}

// End FileHandle API declarations

//let aggrBlob = new Blob([a, b, c, d], { type: 'application/octet-stream' })
// URL.createObjectURL(aggrBlob) and the download will be performed

const hasFileSystemAccessApi = !!navigator.storage && !!navigator.storage.getDirectory;
const hasFileHandleApi = !!window.indexedDB && !!(IDBDatabase.prototype as IDBDatabaseFirefox).createMutableFile;

// ArrayBuffer to String helper.
function ab2str(buf: ArrayBuffer) {
  return String.fromCharCode.apply(null, new Uint16Array(buf));
}

// String to ArrayBuffer helper.
function str2ab(str: string) {
  var buf = new ArrayBuffer(str.length*2); // 2 bytes for each char
  var bufView = new Uint16Array(buf);
  for (var i=0, strLen=str.length; i < strLen; i++) {
    bufView[i] = str.charCodeAt(i);
  }
  return buf;
}

export function saveAsFileSupported() {
  return hasFileSystemAccessApi || hasFileHandleApi;
}

export function saveAsFile(filename: string, data: string): Promise<void> {
  if (hasFileSystemAccessApi) {
    return saveAsFileChrome(filename, data);
  }
  else if (hasFileHandleApi) {
    return openIndexDb().then(db => saveAsFileFirefox(db, filename, data));
  }
  return Promise.reject("Browser file storage not supported");
}

export function getSavedFilesNames(): Promise<string[]> {
  if (hasFileSystemAccessApi) {
    return getAllFileNamesChrome();
  }
  else if (hasFileHandleApi) {
    return openIndexDb().then(db => getAllFileNamesFirefox(db));
  }
  return Promise.reject("Browser file storage not supported");
}

export function readFile(filename: string): Promise<string> {
  if (hasFileSystemAccessApi) {
    return readFileChrome(filename);
  }
  else if (hasFileHandleApi) {
    return openIndexDb().then(db => readFileFirefox(db, filename));
  }
  return Promise.reject("Browser file storage not supported");
}

export function deleteFile(filename: string): Promise<void> {
  if (hasFileSystemAccessApi) {
    return deleteFileChrome(filename);
  }
  else if (hasFileHandleApi) {
    return openIndexDb().then(db => deleteFileFirefox(db, filename));
  }
  return Promise.reject("Browser file storage not supported");
}

function openIndexDb() {
  return new Promise<IDBDatabase>((resolve, reject) => {
    const dbReq = window.indexedDB.open('fileStorageDB', 1);

    dbReq.onupgradeneeded = function () {
      this.result.createObjectStore('files');
    }
    dbReq.onsuccess = function () {
      const db = this.result;
      resolve(db);
    }
    dbReq.onerror = function() {
      // VersionError if opening fails because the actual version is higher than the requested.
      reject(this.error);
    }
    dbReq.onblocked = function() {
      alert("IndexDB blocked. Please close older browser windows with this website.");
    }
  });
}

function getAllFileNamesFirefox(db: IDBDatabase) {
  return new Promise<string[]>((resolve, reject) => {
    const objectStore = db.transaction(["files"]).objectStore('files');
    const request = objectStore.getAll();

    request.onerror = function() {
      reject(this.error);
    };
    request.onsuccess = function() {
      const fileHandles = request.result as IDBMutableFile[];
      resolve(fileHandles.map(h => h.name));
    };
  });
}

function readFileFirefox(db: IDBDatabase, filename: string) {
  return new Promise<string>((resolve, reject) => {
    const objectStore = db.transaction(["files"]).objectStore('files');
    const request = objectStore.get(filename);

    request.onerror = function() {
      reject(this.error);
    };
    request.onsuccess = function() {
      const fileHandle = request.result as IDBMutableFile;

      try {
        const myFile = fileHandle.open('readonly');
        const getmeta = myFile.getMetadata();

        getmeta.onsuccess = function () {
          const size = this.result.size;
          myFile.location = 0;
          const reading = myFile.readAsText(size);

          reading.onsuccess = function () {
            // result must be string because readAsText was called.
            resolve(this.result as string);
          };
          reading.onerror = function () {
            reject(this.error);
          };
        };
      }
      catch(ex) {
        // Possible exceptions see: https://developer.mozilla.org/en-US/docs/Web/API/IDBObjectStore/put#exceptions
        reject(ex.message);
      }
    };
  });
}

function deleteFileFirefox(db: IDBDatabase, filename: string) {
  return new Promise<void>((resolve, reject) => {
    const request = db.transaction(['files'], 'readwrite')
                  .objectStore('files')
                  .delete(filename);

    request.onsuccess = function() {
      resolve();
    };
    request.onerror = function() {
      reject(this.error);
    };
  });
}

function saveAsFileFirefox(db: IDBDatabase, filename: string, data: string) {
  return new Promise<void>((resolve, reject) => {
    const handleReq = (db as IDBDatabaseFirefox).createMutableFile(filename, "plain/text"); //'application/octet-stream'
    
    handleReq.onsuccess = function () {
      const fileHandle = this.result as IDBMutableFile;

      try {
        const file = fileHandle.open('readwrite');
        const writing = file.append(data);

        writing.onerror = function () {
          reject(this.error);
        };
        writing.onsuccess = function () {
          const store = db.transaction(['files'], 'readwrite').objectStore('files');
          const storeReq = store.put(fileHandle, fileHandle.name);

          storeReq.onsuccess = function () {
            //resolve(storeReq.result);
            resolve();
          };
          storeReq.onerror = function () {
            reject(storeReq.error);
          };
        };
      }
      catch(ex)
      {
        // Possible exceptions see: https://developer.mozilla.org/en-US/docs/Web/API/IDBObjectStore/put#exceptions
        reject(ex.message);
      }
    }
  });
}

// https://wicg.github.io/file-system-access/
// https://www.html5rocks.com/de/tutorials/file/filesystem//

// https://wicg.github.io/file-system-access/#sandboxed-filesystem
// https://web.dev/file-system-access/

// interface IStorageManager extends StorageManager {
//   // Returns the root directory of the origin private file system. Available as of Chrome 85.
//   getDirectory(): Promise<FileSystemDirectoryHandle>;
// }

// interface FileSystemDirectoryHandle {
//   getFileHandle(): FileSystemFileHandle
// }

async function saveAsFileChrome(filename: string, content: string) {
  const rootDir = await navigator.storage.getDirectory();
  const fileHandle = await rootDir.getFileHandle(filename, { create: true });
  const fileStream = await fileHandle.createWritable();
  await fileStream.write(content);
  await fileStream.close();
}

function getAllFileNamesChrome() {
  return new Promise<string[]>(async (resolve, reject) => {
    try
    {
      const rootDir = await navigator.storage.getDirectory();
      const fileNames = [];

      for await (let [name, handle] of rootDir.entries()) {
        fileNames.push(name);
      }
      resolve(fileNames);
    }
    catch(ex) {
      reject(ex);
    }
  });
}

async function readFileChrome(filename: string) {
  const rootDir = await navigator.storage.getDirectory();
  const fileHandle = await rootDir.getFileHandle(filename, { create: false });
  const file = await fileHandle.getFile();
  return file.text();
}

async function deleteFileChrome(filename: string) {
  const rootDir = await navigator.storage.getDirectory();
  return await rootDir.removeEntry(filename);
}

// function onError(err) {
//   console.error(err);
// }

// function saveFileChrome() {
//   function onFs(fs) {
//     fs.root.getFile('log4.txt', {create: true, exclusive: true},
//         function(fileEntry) {
//           writeToFileChrome(fileEntry);
//         },
//         onError
//     );
//   }
  
//   window.webkitRequestFileSystem(window.TEMPORARY, 1024*1024 /*1MB*/, onFs, onError);
// }

// function writeToFileChrome(fileEntry) {
//   fileEntry.createWriter(function(fileWriter) {
//     fileWriter.onwriteend = function(e) {
//       console.log('Write completed.');
//     };

//     fileWriter.onerror = function(e) {
//       console.log('Write failed: ' + e.toString());
//     };

//     const blob = new Blob(["test xyz"], {type : 'plain/text'});

//     fileWriter.write(blob);
//   }, onError);
// }

// function readFileChrome() {
//   function onFs(fs) {
//     fs.root.getFile('log4.txt', {}, function(fileEntry) {
//       console.log(fileEntry);
//       // fileEntry.file(function(file) {
//       //   const reader = new FileReader();

//       //   reader.onloadend = function(e) {
//       //     const textarea = document.createElement('textarea');
//       //     textarea.textContent = this.result as string;
//       //     document.body.appendChild(textarea);
//       //   };

//       //   reader.readAsText(file);
//       // }, onError);

//     }, onError);
//   }

//   window.webkitRequestFileSystem(window.TEMPORARY, 1024*1024 /*1MB*/, onFs, onError);
// }

// window.addEventListener('load', async (event) => {
//   console.log("in load");
//   // await saveAsFile("firstfile.txt", str2ab("hello world how are you"));
//   // await getSavedFilesNames().then(names => (names as string[]).forEach(n => console.log(n)));
//   // await readFile("firstfile.txt").then(c => console.log(ab2str(c)));

//   await saveAsFileChrome("firstfile.txt", str2ab("hello world how are you"));
//   await getAllFileNamesChrome().then(names => (names as string[]).forEach(n => console.log(n)));;
//   await readFileChrome("firstfile.txt").then(c => console.log(ab2str(c)));
// }); 
