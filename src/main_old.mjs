const fromEvent = rxjs.fromEvent;
const Observable = rxjs.Observable;
const merge = rxjs.merge;
const zip = rxjs.zip;
const combineLatest = rxjs.combineLatest;
const map = rxjs.operators.map;
const distinctUntilChanged = rxjs.operators.distinctUntilChanged;
const tap = rxjs.operators.tap;
const mapTo = rxjs.operators.mapTo;
const switchMap = rxjs.operators.switchMap;
const takeUntil = rxjs.operators.takeUntil;
const takeWhile = rxjs.operators.takeWhile;
const repeat = rxjs.operators.repeat;

let codeMirrorInst = null

const elemFileInput = document.getElementById("fileInput");
const elemFileSelect = document.getElementById("fileSelect");
const elemErrorText = document.getElementById("errorText")
const elemInfoText = document.getElementById("infoText");
const elemTextareaContainer = document.getElementById("textareaContainer");
const elemEditor = document.getElementById("editor");
const elemTextareaColCount = document.getElementById("textareaColCount");
const elemWrapBtn = document.getElementById("wrapBtn");
const elemTab1Name = document.getElementById("tab1Name");
const elemTab1Tooltip = document.getElementById("tab1Tooltip");
const elemIsSpellcheck = document.getElementById("isSpellcheck");
const elemMode = document.getElementById("mode");
let elemCodeMirrorBackground;
const elemCanvas = document.createElement("canvas");
const elemCanvasCtx = elemCanvas.getContext("2d");
const elemsTabs = [];
elemsTabs.push(document.getElementById("tab1"));
elemsTabs.push(document.getElementById("tab2"));

if (!document.fonts) {
    alert("Browser not supported");
}


let opt1 = document.createElement("option");
opt1.value = null;
opt1.innerText = "Mode: Plain text";
let opt2 = document.createElement("option");
opt2.value = "markdown";
opt2.innerText = "Mode: Markdown";
opt2.selected = true;
elemMode.appendChild(opt1);
elemMode.appendChild(opt2);
elemMode.addEventListener("change", e => {
    codeMirrorInst.setOption("mode", e.target.value);
    // Trigger spellcheck with focus.
    codeMirrorInst.focus();
});

export function drawCrosshair(posX, posY) {
    const width = 50;
    const height = 50;

    const canvas = document.createElement("canvas");
    canvas.style.position = "fixed";
    // Adjust width and height for hdpi displays.
    var ratio = window.devicePixelRatio;
    canvas.width = width * ratio;
    canvas.height = height * ratio;
    canvas.style.width = width + "px";
    canvas.style.height = height + "px";
    canvas.getContext("2d").scale(ratio, ratio);

    canvas.style.left = (posX - (width / 2)) + "px";
    canvas.style.top = (posY - (height / 2)) + "px";
    canvas.style.backgroundColor = "transparent";
    canvas.style.zIndex = "1000";
    document.body.appendChild(canvas);
    const ctx = canvas.getContext("2d");
    ctx.strokeStyle = "#ff0000";
    ctx.lineWidth = 1;
    ctx.moveTo(0, 25);
    ctx.lineTo(50, 25);
    ctx.moveTo(25, 0);
    ctx.lineTo(25, 50);
    ctx.stroke();
    return () => document.body.removeChild(canvas);
}

// update if font size changes
let currentCharWidth = 9.6015625;
//let currentCharActualBoundingBoxAscent = 11.375

function characterWith(fontSize) {
    // for font 16px Roboto Mono
    // latin character width: 9.6015625
    // chinese character width: 48
    elemCanvasCtx.font = fontSize + "px Roboto Mono";
    const textMetrics = elemCanvasCtx.measureText("H");
    //let textMetrics2 = ctx.measureText('文')
    console.log(textMetrics);
    return textMetrics.width;
}

//document.fonts.ready.then(() => characterWith(16))


class RandomAccessFileReader {
    // Reading is fast even above 100 MB but reading a 5 GB file at once gives an error.
    static chunkSize = 1024 * 1024 * 2;

    constructor(file) {
        this.file = file;
    }

    readAsText(offset) {
        return new Promise((resolve, reject) => {
            const blob = this.file.slice(offset, RandomAccessFileReader.chunkSize);
            const fr = new FileReader();

            fr.onload = e => {
                const chunkContent = e.target.result;
                resolve(chunkContent);
            }

            fr.onerror = err => {
                if (err instanceof ProgressEvent) {
                    reject(err.target.error);
                }
                else {
                    reject(err);
                }
            }

            fr.readAsText(blob);
        })
    }
}

function showInfo(msg) {
    elemInfoText.textContent = msg;
}

function showError(msg) {
    elemErrorText.textContent = msg;
}

function onFileAdded(fileList) {
    showError("");
    showInfo("");

    if (fileList.length > 0) {
        const file = fileList[0];
        elemTab1Name.textContent = file.name;
        elemTab1Tooltip.textContent = file.name;

        //elemTextarea.scrollTop = 0;

        const sfr = new RandomAccessFileReader(file);
        sfr.readAsText(0)
            .then(res => codeMirrorInst.setValue(res.slice(0, 1024 * 8)))
            .catch(showError);
    }
}

elemFileSelect.addEventListener("click", _ => elemFileInput.click());
elemFileInput.addEventListener("change", e => onFileAdded(e.target.files));
elemWrapBtn.addEventListener("click", _ => {
    if (elemWrapBtn.classList.contains("commandBarBtnSelected")) {
        elemWrapBtn.classList.remove("commandBarBtnSelected");
        codeMirrorInst.setOption("lineWrapping", false);
    }
    else {
        elemWrapBtn.classList.add("commandBarBtnSelected");
        codeMirrorInst.setOption("lineWrapping", true);
    }
})

const dropArea = document.getElementById("drop-area");

document.body.addEventListener("dragover", e => {
    e.stopPropagation();
    e.preventDefault();
    // Style the drag-and-drop as a "copy file" operation.
    e.dataTransfer.dropEffect = "copy";
    elemCodeMirrorBackground.style.backgroundColor = "#737373";
    //elemTextarea.style.backgroundColor = "#737373";
});

document.body.addEventListener("drop", e => {
    e.stopPropagation();
    e.preventDefault();
    elemCodeMirrorBackground.style.backgroundColor = "initial";
    //elemTextarea.style.backgroundColor = "inherit";
    onFileAdded(e.dataTransfer.files);
});

function onTabClick(e) {
    elemsTabs.forEach(elem => {
        if (e.currentTarget === elem) {
            elem.classList.remove("inactive-tab");
            // todo: show tab content.
        }
        else {
            elem.classList.add("inactive-tab");
        }
    })
    e.stopPropagation();
}

elemsTabs.forEach(elem => elem.addEventListener("click", onTabClick));

let elemCodeMirror;
let textPaddingLeft;
let textPaddingRight;
let gutterWidth;

function onCodeMirrorInitialized() {
    elemCodeMirror = document.querySelector('.CodeMirror');
    elemCodeMirrorBackground = document.querySelector(".cm-s-darcula.CodeMirror");
    const elemGutters = document.querySelector('.CodeMirror-gutters');
    const elemCodeMirrorLine = document.querySelector(".CodeMirror-line");

    //CodeMirror-measure

    // 30px -> width 29px plus border 1px
    gutterWidth = elemGutters.offsetWidth;
    console.log("gutterWidth: " + gutterWidth);

    const codeMirrorLineStyle = getComputedStyle(elemCodeMirrorLine);
    // 4px and 4px
    textPaddingLeft = parseInt(codeMirrorLineStyle.getPropertyValue("padding-left"));
    textPaddingRight = parseInt(codeMirrorLineStyle.getPropertyValue("padding-right"));

    console.log("textPaddingLeft:: " + textPaddingLeft);

    const codeMirrorClientWidth = elemCodeMirror.clientWidth;
    const textWidth = codeMirrorClientWidth - gutterWidth - textPaddingLeft - textPaddingRight;
    console.log("textWidth: " + textWidth);

    const textWidth2 = elemCodeMirrorLine.offsetWidth - textPaddingLeft - textPaddingRight;
    console.log("textWidth2: " + textWidth2);

    const colCount = Math.floor(textWidth2 / currentCharWidth);
    elemTextareaColCount.textContent = colCount;


    fromEvent(document, "mousedown").pipe(
        // Add mousedown coordinates and DOMRect of textarea to state.
        map(e => ({ xAtMousedown: e.clientX, yAtMousedown: e.clientY, textareaDOMRectAtMousedown: elemCodeMirror.getBoundingClientRect(), textareaContainerOffsetWidthAtMousedown: elemTextareaContainer.offsetWidth, codeMirrorClientWidthAtMousedown: elemCodeMirror.clientWidth })),
        // Continue only if mouse was pressed close to right border.
        takeWhile(state => calcIsMouseNearRightBorder(state.textareaDOMRectAtMousedown, state.xAtMousedown, state.yAtMousedown)),
        // Once a mousedown event happened emit mousemove events until mouse is released.
        switchMap(state => fromEvent(document, "mousemove").pipe(map(e => ({ ...state, xAtMousemove: e.clientX, yAtMousemove: e.clientY })))),
        // Process mousemove.
        tap(onPressedMouseMove),
        // When mouse is released, end emitting mousemove.
        takeUntil(fromEvent(document, "mouseup")),
        // Repeat (waiting for next mousedown).
        repeat()
    ).subscribe();


    fromEvent(document, "mousemove").pipe(
        map(e => calcIsMouseNearRightBorder(elemCodeMirror.getBoundingClientRect(), e.clientX, e.clientY)),
        distinctUntilChanged(),
        tap(isMouseNearRightBorder => {
            if (isMouseNearRightBorder) {
                elemCodeMirror.classList.add("cursor-col-resize");
                document.documentElement.classList.add("cursor-col-resize");
            }
            else {
                elemCodeMirror.classList.remove("cursor-col-resize");
                document.documentElement.classList.remove("cursor-col-resize");
            }
        })
    ).subscribe();
}

function calcIsMouseNearRightBorder(domRect, mouseX, mouseY) {
    return mouseX < domRect.right + 8 && mouseX > domRect.right - 2 && mouseY > domRect.top && mouseY < domRect.bottom;
}

function onPressedMouseMove({ xAtMousedown, textareaContainerOffsetWidthAtMousedown, codeMirrorClientWidthAtMousedown, xAtMousemove }) {
    const mouseDeltaX = xAtMousemove - xAtMousedown;
    const newWidth = textareaContainerOffsetWidthAtMousedown + mouseDeltaX;
    elemTextareaContainer.style.width = newWidth + "px";

    const newTextWidth = codeMirrorClientWidthAtMousedown - gutterWidth - textPaddingLeft - textPaddingRight + mouseDeltaX;
    const cols = Math.floor(newTextWidth / currentCharWidth);
    elemTextareaColCount.textContent = cols;
}

document.onkeydown = e => {
    // Ctrl-f
    if ((e.ctrlKey || e.metaKey) && e.code === "KeyF") {
        e.preventDefault();
        alert("Custom search here");
        return false;
    }
}

elemIsSpellcheck.addEventListener("click", e => {
    // // Spellcheck is not applied/unapplied to existing text, so remove and add it.
    // const text = elemTextarea.value;
    // const selectionStart = elemTextarea.selectionStart;
    // const selectionEnd = elemTextarea.selectionEnd;
    // elemTextarea.value = "";
    // elemTextarea.spellcheck = e.target.checked;
    // elemTextarea.value = text;
    // elemTextarea.selectionStart = selectionStart;
    // elemTextarea.selectionEnd = selectionEnd;
    // elemTextarea.focus();

    codeMirrorInst.setOption("spellcheck", e.target.checked);

    const val = codeMirrorInst.getValue();
    codeMirrorInst.setValue("");
    codeMirrorInst.setValue(val);
    // document.querySelector("div.CodeMirror-code").setAttribute("spellcheck", e.target.checked);
    codeMirrorInst.focus();
});

//codeMirror.setOption("inputStyle", "contenteditable");
// var codeMirror = CodeMirror(elemEditor, {
//     mode: "markdown",
//     lineNumbers: true,
//     theme: "darcula",
//     inputStyle: "contenteditable"
// });
//<script src="/submodules/CodeMirror/mode/javascript/javascript.js"></script>
//<script src="/submodules/CodeMirror/mode/markdown/markdown.js"></script>

function initializeCodeMirror() {
    codeMirrorInst = CodeMirror.fromTextArea(elemEditor, {
        inputStyle: "contenteditable",
        spellcheck: true,

        mode: "markdown",
        lineNumbers: true,
        theme: "darcula",
    });

    codeMirrorInst.setValue("Hello world this is an example text with speellingmistace to have some content %$/YX=ß*à!");
    codeMirrorInst.focus();

    onCodeMirrorInitialized();
}

import("./submodules/CodeMirror/src/codemirror.js")
    .then((module) => {
        const CodeMirror = module.default
        // CodeMirror must be global so other modules find it.
        window.CodeMirror = CodeMirror
        import("./submodules/CodeMirror/mode/javascript/javascript.js").then(module => {
            window.editor = CodeMirror(document.getElementById("code"), {
                mode: null,
                //extraKeys: {"Cmd-Space": "autocomplete"},
                value: document.documentElement.innerHTML
            });
            initializeCodeMirror();
        });
    });



