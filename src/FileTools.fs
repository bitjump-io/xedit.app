module FileTools

open Browser
open Browser.Types

// Reading is fast even above 100 MB but reading a 5 GB file at once gives an error.
let chunkSize = 1024 * 1024 * 2

let readAsText (offset: int, file: File) =
  Promise.create(fun resolve reject ->
    let fr = FileReader.Create ()
    fr.addEventListener("load", fun _ ->
      let blobContent = fr.result
      resolve(blobContent :?> string) // Must be string because readAsText api is used.
    )
    fr.addEventListener("error", fun err ->
      console.error(err)
      reject(failwith (sprintf "%A" err))
    )
    let blob = file.slice(offset, chunkSize)
    fr.readAsText(blob)
  )
