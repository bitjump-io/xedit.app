module App

open Feliz
open Elmish
open Feliz.MaterialUI
open Browser
open Browser.Types
open Thoth.Elmish
open System
open MonacoEditor
open Model
open Msg
open DomEx
open Literals
open RootDiv
open Editor
open HtmlEx

// The init function will produce an initial state once the program starts running.  It can take any arguments.
let init () =
  { SelectedTabId = 0
    TabItems = [TabItemModel.initial]
    EditorHeight = 0
    EditorOptions = EditorOptions.initial
    EditorLanguage = PlainText
    ShowTooltipControlId = ControlId.None
    WindowInnerWidth = window.innerWidth
    DevicePixelRatio = window.devicePixelRatio
    Debouncer = Debouncer.create()
    DragModel = DragModel.initial
  }, 
  Cmd.none

let getLanguageFromFilename (fileName: string) =
  if fileName.EndsWith(".js") then JavaScript
  elif fileName.EndsWith(".ts") then TypeScript
  else PlainText

let addItemAtIndex (list: 'a list, index: int, newItem: 'a) =
  let tabsLeftToNewTab = list.[..index - 1]
  let tabsRightToNewTab = list.[(index)..]
  tabsLeftToNewTab @ newItem :: tabsRightToNewTab

let replaceItemAtIndex (list: 'a list, index: int, newItem: 'a) =
  list.[..(index - 1)] @ newItem :: list.[(index + 1)..]

// Helpers to update nested state.
let updateEditorOptions (msg: Msg) (model: EditorOptions) =
  match msg with
  | ToggleWrapText -> { model with WrapText = not model.WrapText }, Cmd.none
  | _ -> failwith (sprintf "No case implemented to update EditorOptions for message %A" msg)

// Helpers to update nested state.
let updateDragModel (msg: Msg) (model: DragModel) =
  match msg with
  | FilesAdded _ -> DragModel.initial, Cmd.none
  | OnDrop _ -> DragModel.initial, Cmd.none
  | OnDragenter _ -> { model with DragenterCount = model.DragenterCount + 1 }, Cmd.none
  | OnDragleave _ -> { model with DragleaveCount = model.DragleaveCount + 1 }, Cmd.none
  | _ -> failwith (sprintf "No case implemented to update updateDragModel for message %A" msg)

// The update function will receive the change required by Msg, and the current state. It will produce a new state and potentially new command(s).
let update (msg: Msg) (model: Model) =
  match msg with
  | EditorCreated (Height height) -> { model with EditorHeight = height }, Cmd.none
  | ToggleWrapText ->
    Option.iter (Editor.setWordWrap(not model.EditorOptions.WrapText)) monacoEditor
    let (editorOptionsModel, editorOptionsCmd) = updateEditorOptions msg model.EditorOptions
    { model with EditorOptions = editorOptionsModel }, editorOptionsCmd
  | OpenFilePicker ->
    // The default html file picker is not nice so it is displayed invisible and the click event is triggered here.
    Option.iter click (getElementById(FileInputElementId))
    model, Cmd.none
  | FilesAdded files
  | OnDrop files ->
    let (dragModel, dragCmd) = updateDragModel msg model.DragModel
    let tabItemModelPromises = 
      if monacoEditor.IsSome then
        let monacoEditorVal = monacoEditor.Value
        [for file in files -> 
          FileTools.readAsText (0, file)
          |> Promise.map (fun text -> 
            let syntaxLang = getLanguageFromFilename(file.name)
            let modelIndex = Editor.addTextModel (text, unbox<string> syntaxLang) monacoEditorVal
            { ModelIndex = modelIndex; Name = file.name; Language = syntaxLang; IsUntitled = false }
          )
        ]
      else
        []
    let newModel = { model with DragModel = dragModel }
    newModel, Cmd.batch (dragCmd :: [for t in tabItemModelPromises -> Cmd.OfPromise.either (fun _ -> t) () AddTab OnPromiseError])
  | WindowWidthChaned newWidth ->
    let (debouncerModel, debouncerCmd) =
      model.Debouncer
      |> Debouncer.bounce (TimeSpan.FromSeconds 1.5) "unused" EndOfWindowWidthChaned
    { model with WindowInnerWidth = newWidth; Debouncer = debouncerModel }, Cmd.batch [ Cmd.map DebouncerSelfMsg debouncerCmd ]
  | DebouncerSelfMsg debouncerMsg ->
    let (debouncerModel, debouncerCmd) = Debouncer.update debouncerMsg model.Debouncer
    { model with Debouncer = debouncerModel }, debouncerCmd
  | EndOfWindowWidthChaned ->
    let widthAdjustment = -2 // Needed so main-content and editor get the same computed width.
    match getElementById(MainContainerElementId) with
    | Some mainContainerElement -> 
      let mainContainerWidth = int mainContainerElement.clientWidth
      Option.iter (Editor.layout({ width = mainContainerWidth + widthAdjustment; height = model.EditorHeight })) monacoEditor
    | None -> ()
    model, Cmd.none
  | TabChanged selectedTabId ->
    console.log("tab changed " + string selectedTabId)
    let tabModel = model.TabItems.Item(selectedTabId)
    Option.iter (Editor.setTextModelIndex(tabModel.ModelIndex)) monacoEditor
    { model with SelectedTabId = selectedTabId; EditorLanguage = tabModel.Language }, Cmd.none
  | EditorLanguageChanged editorLanguage ->
    Option.iter (Editor.setLanguage(unbox<string> editorLanguage)) monacoEditor 
    let tabModel = model.TabItems.Item(model.SelectedTabId)
    let newTabModel = { tabModel with Language = editorLanguage }
    let newTabModels = replaceItemAtIndex(model.TabItems, model.SelectedTabId, newTabModel)
    { model with EditorLanguage = editorLanguage; TabItems = newTabModels }, Cmd.none
  | ShowTooltipChanged controlId ->
    { model with ShowTooltipControlId = controlId }, Cmd.none
  | OnDragenter
  | OnDragleave ->
    let (dragModel, dragCmd) = updateDragModel msg model.DragModel
    { model with DragModel = dragModel }, dragCmd
  | AddEmptyTab ->
    let msg =
      if monacoEditor.IsSome then
        let modelIndex = Editor.addTextModel ("", unbox<string> PlainText) monacoEditor.Value
        let untitledCount = model.TabItems |> (List.filter (fun t -> t.IsUntitled) >> List.length)
        Cmd.ofMsg (AddTab { ModelIndex = modelIndex; Name = "Untitled " + string (untitledCount + 1); Language = PlainText; IsUntitled = true })
      else
        Cmd.none
    model, msg
  | AddTab newTab ->
    // Add new tabs to the right of the currently selected tab.
    let tabs = addItemAtIndex (model.TabItems, model.SelectedTabId + 1, newTab)
    Option.iter (Editor.setTextModelIndex(newTab.ModelIndex)) monacoEditor
    { model with TabItems = tabs; SelectedTabId = model.SelectedTabId + 1 }, Cmd.none
  | RemoveTab index ->
    model, Cmd.none
  | OnPromiseError error ->
    console.error("An error occured when fetching data", error)
    model, Cmd.none

let addDragAndDropListener (dispatch: Msg -> unit) =
  document.addEventListener("dragover", fun e ->
    e.stopPropagation()
     // Without preventDefault, the dragged file is opened by the browser.
    e.preventDefault()
    (e :?> DragEvent).dataTransfer.dropEffect <- "copy"
  )

  document.addEventListener("dragenter", fun e ->
    e.stopPropagation()
    e.preventDefault()
    dispatch OnDragenter
  )

  document.addEventListener("dragleave", fun e ->
    e.stopPropagation()
    e.preventDefault()
    dispatch OnDragleave
  )

  document.addEventListener("drop", fun e ->
    e.stopPropagation()
    e.preventDefault()
    let fileList = (e :?> DragEvent).dataTransfer.files
    let files = [for i in 0..(fileList.length - 1) -> fileList.item(i)]
    OnDrop files |> dispatch
  )

// Website markup definition.
[<ReactComponent>]
let App (model: Model, dispatch) =
  React.useEffectOnce(fun () ->
    window.addEventListener("resize", fun _ -> WindowWidthChaned window.innerWidth |> dispatch)
    addDragAndDropListener dispatch
  )
  React.useEffect(
    (fun () -> 
      let editAreaElem = (document.querySelector(".monaco-editor-background") :?> IHTMLElement)
      let marginElem = (document.querySelector(".monaco-editor .margin") :?> IHTMLElement)
      if model.DragModel.isDragging then
        editAreaElem.style.backgroundColor <- "#737373"
        marginElem.style.backgroundColor <- "#737373"
      else
        editAreaElem.style.backgroundColor <- ""
        marginElem.style.backgroundColor <- ""
    ),
    [|model.DragModel.isDragging :> obj|])
  Mui.themeProvider [
    themeProvider.theme Themes.darkTheme
    themeProvider.children [
      RootDivComponent(model, dispatch)
    ]
  ]

let render (state: Model) (dispatch: Msg -> unit) =
  App (state, dispatch)
