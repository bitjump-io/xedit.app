module Update

open Elmish
open Browser
open Thoth.Elmish
open System
open Model
open Msg
open HtmlEx
open Literals
open DomEx
open Browser.Dom
open JavaScriptHelper
open Fable.Core
open Fable.Core.JsInterop
open EventHelper

let getOS() =
  match navigatorObj with
  | Some n -> 
    let userAgent = n.userAgent
    if userAgent.Contains("Windows") then OS.Windows
    elif userAgent.Contains("Macintosh") then OS.Mac
    elif (userAgent.Contains("Macintosh") || userAgent.Contains("iPad") || userAgent.Contains("iPhone")) && forceBool(n.maxTouchPoints) && n.maxTouchPoints > 0 then OS.Mac
    elif userAgent.Contains("Linux") then OS.Linux
    else OS.Unknown
  | None -> OS.Unknown

let os = getOS()

// The init function will produce an initial state once the program starts running.  It can take any arguments.
let init () =
  { SelectedTabId = 0
    TabItems = [TabItemModel.initial]
    EditorHeight = 0
    EditorOptions = EditorOptions.initial
    EditorLanguage = PlainText
    EditorDomElementId = None
    IsMonacoEditorModulePromiseResolved = false
    ShowTooltipControlId = ControlId.None
    WindowInnerWidth = window.innerWidth
    DevicePixelRatio = window.devicePixelRatio
    Debouncer = Debouncer.create()
    DragModel = DragModel.initial
    ThemeKind = ThemeKind.Dark
    OS = os
    ShowKeyBindingsFor = os
  },
  Cmd.none

let getLanguageFromFilename (fileName: string) =
  if fileName.EndsWith(".md") then Markdown
  elif fileName.EndsWith(".js") then JavaScript
  elif fileName.EndsWith(".ts") then TypeScript
  elif fileName.EndsWith(".json") then Json
  elif fileName.EndsWith(".xml") then Xml
  elif fileName.EndsWith(".yaml") then Yaml
  elif fileName.EndsWith(".sql") then Sql
  else PlainText

let addItemAtIndex (list: 'a list, index: int, newItem: 'a) =
  let tabsLeftToNewTab = list.[..index - 1]
  let tabsRightToNewTab = list.[(index)..]
  tabsLeftToNewTab @ newItem :: tabsRightToNewTab

let removeItemAtIndex (list: 'a list, index: int) =
  list.[..(index - 1)] @ list.[(index + 1)..]

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
  | OnDrop _ -> DragModel.initial, Cmd.none
  | OnDragenter _ -> { model with DragenterCount = model.DragenterCount + 1 }, Cmd.none
  | OnDragleave _ -> { model with DragleaveCount = model.DragleaveCount + 1 }, Cmd.none
  | _ -> failwith (sprintf "No case implemented to update updateDragModel for message %A" msg)

// The update function will receive the change required by Msg, and the current state. It will produce a new state and potentially new command(s).
let update (msg: Msg) (model: Model) =
  match msg with
  | EditorCreated (Height height) -> 
    let cmd =
      if monacoEditor.IsSome then
        monacoEditor.Value.focus()

        let sub dispatch =
          monacoEditor.Value.onDidChangeContent 0 (fun e -> ModelContentChange e.changes |> dispatch)
        Cmd.ofSub sub
      else
        Cmd.none
    { model with EditorHeight = height }, cmd
  | ToggleWrapText ->
    monacoEditor |> Option.iter (fun editor -> editor.setWordWrap(not model.EditorOptions.WrapText); editor.focus())
    let (editorOptionsModel, editorOptionsCmd) = updateEditorOptions msg model.EditorOptions
    { model with EditorOptions = editorOptionsModel }, editorOptionsCmd
  | OnDrop files ->
    let (dragModel, dragCmd) = updateDragModel msg model.DragModel
    let tabItemModelPromises = 
      if monacoEditor.IsSome then
        [for file in files -> 
          promise {
            let! text = FileTools.readAsText (0, file)
            let syntaxLang = getLanguageFromFilename(file.name)
            let modelIndex = monacoEditor.Value.addTextModel (text, unbox<string> syntaxLang)
            return { Name = file.name; ModelIndex = modelIndex; Language = syntaxLang; UntitledIndex = 0; ContentSize = text.Length }
          }
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
      monacoEditor |> Option.iter (fun editor -> editor.layout({ width = mainContainerWidth + widthAdjustment; height = model.EditorHeight }))
    | None -> ()
    model, Cmd.none
  | TabChanged selectedTabId ->
    let tabModel = model.TabItems.Item(selectedTabId)
    monacoEditor |> Option.iter (fun editor -> editor.setTextModelIndex(tabModel.ModelIndex); editor.focus())
    { model with SelectedTabId = selectedTabId; EditorLanguage = tabModel.Language }, Cmd.none
  | EditorLanguageChanged editorLanguage ->
    monacoEditor |> Option.iter (fun editor -> editor.setLanguage(unbox<string> editorLanguage))
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
        let modelIndex = monacoEditor.Value.addTextModel ("", unbox<string> PlainText)
        let untitledIndex = 
          if List.isEmpty model.TabItems then 1
          else 1 + (model.TabItems |> (List.map (fun x -> x.UntitledIndex) >> List.max))
        Cmd.ofMsg (AddTab { Name = "Untitled " + string (untitledIndex); ModelIndex = modelIndex; Language = PlainText; UntitledIndex = untitledIndex; ContentSize = 0 })
      else
        Cmd.none
    model, msg
  | AddTab newTab ->
    let tabs = model.TabItems @ [newTab]
    let tabToSelect = List.length tabs - 1 
    { model with TabItems = tabs }, Cmd.ofMsg (TabChanged tabToSelect)
  | RemoveTab index ->
    let tabs = removeItemAtIndex (model.TabItems, index)
    let selectedTabId = if model.SelectedTabId > index || model.SelectedTabId = List.length tabs then model.SelectedTabId - 1 else model.SelectedTabId
    let cmd = if List.isEmpty tabs then Cmd.ofMsg AddEmptyTab else Cmd.ofMsg (TabChanged selectedTabId)
    { model with TabItems = tabs}, cmd
  | OnPromiseError error ->
    console.error("An promise was rejected: ", error)
    model, Cmd.none
  | ThemeKind themeKind ->
    { model with ThemeKind = themeKind }, Cmd.none
  | IncreaseFontSize ->
    monacoEditor |> Option.iter (fun editor -> editor.increaseFontSize(); editor.focus())
    model, Cmd.none
  | DecreaseFontSize ->
    monacoEditor |> Option.iter (fun editor -> editor.decreaseFontSize(); editor.focus())
    model, Cmd.none
  | ShowKeyBindingsForChanged os ->
    { model with ShowKeyBindingsFor = os }, Cmd.none
  | ModelContentChange changes ->
    let sizeChange = changes |> Seq.sumBy (fun c -> c.text.Length - c.rangeLength)
    let tabModel = model.TabItems.Item(model.SelectedTabId)
    let newTabModel = { tabModel with ContentSize = tabModel.ContentSize + sizeChange }
    let newTabModels = replaceItemAtIndex(model.TabItems, model.SelectedTabId, newTabModel)
    console.log("New size: ", newTabModel.ContentSize)
    { model with TabItems = newTabModels }, Cmd.none
  | MonacoEditorModulePromiseResolved ->
    (window :?> IWindow).performance.mark("MonacoEditorModulePromiseResolved")
    let cmd = 
      if model.EditorDomElementId.IsSome
      then Cmd.ofMsg CreateEditor
      else Cmd.none
    { model with IsMonacoEditorModulePromiseResolved = true }, cmd
  | EditorDomElementCreated id ->
    let cmd = 
      if model.IsMonacoEditorModulePromiseResolved
      then Cmd.ofMsg CreateEditor
      else Cmd.none
    { model with EditorDomElementId = Some id }, cmd
  | CreateEditor ->
    let editorDomElementId = model.EditorDomElementId.Value
    let editorElem = getElementValueById editorDomElementId
    let width = int editorElem.clientWidth
    let height = int editorElem.clientHeight
    let editorCreatedPromise = 
      importDynamic "../src/editor/MonacoEditor.ts" :> JS.Promise<MonacoEditorTypes.IExports>
      |> Promise.map (fun p -> 
        monacoEditor <- Some (p.create(editorElem, { width = width; height = height }))
        Height height)
    model, Cmd.OfPromise.either (fun _ -> editorCreatedPromise) () EditorCreated OnPromiseError
