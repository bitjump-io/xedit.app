module App

open Feliz
open Elmish
open Feliz.MaterialUI
open Browser
open Browser.Types
open Browser.Dom
open Thoth.Elmish
open System
open MonacoEditor
open Fable.Core
open DomEx

[<Literal>]
let FileInputElementId = "file-input"

[<Literal>]
let MainContainerElementId = "main-container"

[<Emit("debugger")>]
let debugger () : unit = jsNative

type Height = | Height of int

[<StringEnum>]
type EditorLanguage =
  | [<CompiledName("plaintext")>] PlainText
  | [<CompiledName("javascript")>] JavaScript
  | [<CompiledName("typescript")>] TypeScript
  member x.displayText =
    match x with
    | PlainText -> "Plain text"
    | JavaScript -> "JavaScript"
    | TypeScript -> "TypeScript"
  
  static member all = [PlainText; JavaScript; TypeScript]

type EditorOptions = { WrapText: bool }
  with static member initial = { WrapText = false }

type DragModel = { DragenterCount: int; DragleaveCount: int }
  with 
    static member initial = { DragenterCount = 0; DragleaveCount = 0 }
    member x.isDragging = x.DragenterCount > x.DragleaveCount

type TabItemModel = { ModelIndex: int; Name: string; Language: EditorLanguage }
  with
    static member initial = { ModelIndex = 0; Name = "Untitled 1"; Language = PlainText }

[<RequireQualifiedAccess>]
type ControlId =
  | None
  | WrapText
  | EditorLanguage

// Not included in model because quite large.
let mutable monacoEditor: IMonacoEditor option = None

// Model holds the current state.
type Model = 
  { SelectedTabId: int
    TabItems: TabItemModel list
    EditorHeight: int
    EditorOptions: EditorOptions
    EditorLanguage: EditorLanguage
    ShowTooltipControlId: ControlId
    WindowInnerWidth: float
    DevicePixelRatio: float
    Debouncer: Debouncer.State
    DragModel: DragModel }

// Msg is an action that we need to apply to the current state.
type Msg =
  | EditorCreated of Height
  | ToggleWrapText
  | OpenFilePicker
  | FilesAdded of File list
  | WindowWidthChaned of float // Triggered each time a window.resize event is emitted.
  | DebouncerSelfMsg of Debouncer.SelfMessage<Msg> // This is the message used by the Debouncer.
  | EndOfWindowWidthChaned // Message we want to debounce.
  | TabChanged of int
  | AddTab of TabItemModel
  | RemoveTab of int
  | EditorLanguageChanged of EditorLanguage
  | ShowTooltipChanged of ControlId
  | OnDragenter
  | OnDragleave
  | OnDrop of File list
  | OnPromiseError of exn

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

let getElementById id =
  let el = document.getElementById(id)
  if isNull el then 
    None
  else 
    Some el

let click (el: HTMLElement) =
  el.click()

let getLangugeFromFilename (fileName: string) =
  if fileName.EndsWith(".js") then JavaScript
  elif fileName.EndsWith(".ts") then TypeScript
  else PlainText

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
  | _ -> failwith (sprintf "No case implemented to update EditorOptions for message %A" msg)

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
            let syntaxLang = getLangugeFromFilename(file.name)
            let modelIndex = Editor.addTextModel (text, unbox<string> syntaxLang) monacoEditorVal
            { ModelIndex = modelIndex; Name = file.name; Language = syntaxLang }
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
  | AddTab newTab ->
    // Add new tabs to the right of the currently selected tab.
    let tabsLeftToNewTab = model.TabItems.[..model.SelectedTabId]
    let tabsRightToNewTab = model.TabItems.[(model.SelectedTabId + 1)..]
    let tabs = tabsLeftToNewTab @ newTab :: tabsRightToNewTab
    Option.iter (Editor.setTextModelIndex(newTab.ModelIndex)) monacoEditor
    { model with TabItems = tabs; SelectedTabId = model.SelectedTabId + 1 }, Cmd.none
  | RemoveTab index ->
    model, Cmd.none
  | OnPromiseError error ->
    console.error("An error occured when fetching data", error)
    model, Cmd.none

// Styles documentation links
// - https://github.com/cmeeren/Feliz.MaterialUI/blob/master/docs-app/public/pages/samples/sign-in/SignIn.fs
// - https://cmeeren.github.io/Feliz.MaterialUI/#usage/styling
// - https://material-ui.com/styles/basics/

type CssClasses = { RootDiv: string }

let useStyles = Styles.makeStyles(fun styles theme ->
  {
    RootDiv = styles.create [
      style.backgroundColor theme.palette.background.paper
      style.padding 10
      style.fontSize 16
      style.color "#fff"
      style.height (length.percent 100)
      style.fontFamily "system-ui, -apple-system, BlinkMacSystemFont, Roboto, Helvetica, sans-serif"
      // Next line not needed but kept as an example for nested styles.
      //style.custom ("&:focus", styles.create [style.borderRadius 4; style.borderColor "#ff0000"; style.boxShadow (0, 0, 0, 20, "rgba(0,123,255,.25)")] )
    ]
  }
)

// Material UI extensions.
type MuiEx =
  static member inline buttonOutlined (value: string, ?onClick: MouseEvent -> unit) =
    Mui.button [
      button.variant.outlined
      button.children value
      if onClick.IsSome
        then prop.onClick onClick.Value
    ]
  static member inline withTooltip (title: string, showTooltip: bool, element: ReactElement) =
    Mui.tooltip [
      tooltip.title title
      tooltip.arrow true
      tooltip.placement.bottom
      tooltip.children(element)
      tooltip.open' showTooltip
    ]

type HtmlEx =
  static member inline redText (value: string) =
    Html.span [
      prop.style [style.color.red]
      prop.children [Html.text value]
    ]

// The MonacoEditor as a react component.
[<ReactComponent>]
let EditorComponent (model, dispatch) =
  let divEl = React.useElementRef()
  React.useEffectOnce(fun () ->
    let editor = 
      match divEl.current with
      | Some x -> 
        let width = int x.clientWidth
        let height = 600
        monacoEditor <- Some(Editor.create(x, { width = width; height = height }))
        EditorCreated (Height height) |> dispatch
        monacoEditor
      | None -> None
    React.createDisposable(fun () -> console.error("Should never dispose EditorComponent"))
  )
  Html.div [
    prop.style [style.display.block; style.width length.auto; style.height length.auto; style.minHeight 100; style.border (1, borderStyle.solid, "#858585")]
    prop.ref divEl
  ]

let headerElement model dispatch =
  Html.div [
    Html.input [
      prop.id FileInputElementId
      prop.type' "file"
      prop.multiple true
      prop.style [style.display.none]
      prop.onChange (FilesAdded >> dispatch)
    ]
    Mui.typography [
      typography.variant.h2
      typography.children [
        Html.text "xedit.app - Client side "
        HtmlEx.redText "x"
        Html.text "tra large file "
        HtmlEx.redText "edit"
        Html.text "or app"
      ]
    ]
    Mui.typography [
      typography.variant.body1
      prop.style [style.marginBottom 10; style.marginTop 10]
      typography.children [
        Html.text "Drag & drop anywhere to open files or use the "
        MuiEx.buttonOutlined ("file picker", fun _ -> dispatch OpenFilePicker)
      ]
    ]
  ]

let toolbarElement model dispatch (classes: CssClasses) =
  Html.div [
    prop.style [style.marginTop 14]
    prop.children [
      MuiEx.withTooltip (
        "Wrap text",
        model.ShowTooltipControlId = ControlId.WrapText,
        Mui.iconButton [ 
          prop.style [style.verticalAlign.bottom; style.height 38; style.width 38; style.marginRight 5]
          prop.onClick (fun _ -> dispatch ToggleWrapText)
          prop.onMouseEnter (fun _ -> (ShowTooltipChanged ControlId.WrapText) |> dispatch)
          prop.onMouseLeave (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
          iconButton.children (Icons.wrapTextIcon [ if model.EditorOptions.WrapText then icon.color.primary else () ]) 
        ])
      MuiEx.withTooltip (
        "Language",
        model.ShowTooltipControlId = ControlId.EditorLanguage,
        Mui.formControl [
          formControl.size.small
          formControl.variant.outlined
          formControl.children [
            Mui.select [
              select.value model.EditorLanguage
              select.onChange (EditorLanguageChanged >> dispatch)
              select.onOpen (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
              prop.onMouseEnter (fun _ -> (ShowTooltipChanged ControlId.EditorLanguage) |> dispatch)
              prop.onMouseLeave (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
              select.input (
                Mui.inputBase []
              )
              select.children [
                for el in EditorLanguage.all -> 
                  Mui.menuItem [
                    prop.value (unbox<string> el)
                    menuItem.children [
                      Html.text el.displayText
                    ]
                  ]
              ]
            ]
          ]
        ]
      )
    ]
  ]

let tabsWithContentElement model dispatch =
  Mui.tabContext [
    tabContext.value (string model.SelectedTabId)
    tabContext.children [
      Mui.paper [
        prop.style [style.flexGrow 1; style.marginTop 5]
        paper.children [
          Mui.tabs [
            tabs.value model.SelectedTabId
            tabs.onChange (TabChanged >> dispatch)
            tabs.indicatorColor.primary
            tabs.textColor.primary
            tabs.children [
              for t in model.TabItems ->
                Mui.tab [
                  tab.label t.Name
                ]
            ]
          ]
        ]
      ]
      Html.div [
        EditorComponent(model, dispatch)
      ]
    ]
  ]

let contentBelowTabsElement =
  Html.div [
    prop.style [style.marginTop 5]
    prop.children [
      Html.text "Tipps"
      Html.ul [
        // Better remap default keybindings to CodeMirror keybindings.
        // https://github.com/microsoft/monaco-editor/issues/102
        // https://github.com/microsoft/monaco-editor/issues/1350
        Html.li "To see the context menu, right-MouseClick (Crl-MouseClick on Mac)."
        Html.li "Hold down Alt (option on Mac) and navigate with the left and right arrows to move word-by-word. Additionally hold down Shift to select while moving."
        Html.li "Hold down Ctrl (command on Mac) and navigate with the left and right arrows to move to the start and end of the line. Additionally hold down Shift to select while moving."
        Html.li "Hold down Alt (option on Mac) and navigate with the up and down arrows to move the current row up or down."
        Html.li "Add additional cursors by holding down Alt (option on Mac) then MouseClick where you want the cursors then release Alt."
        Html.li "To add multiple cursors in the same column at once, MouseClick in the row and column where the firt cursor should be then holding down Shift + Alt (Shift + option on Mac) and MouseClick the last row of the same column."
        Html.li "To select text in column mode, MouseClick at the upper left start of the text that you want to select, then hold down Shift + Alt (Shift + option on Mac) and MouseClick or MousePress-and-Drag to select the bottom right of the selection."
        Html.li "You can drag selected text with the mouse."
        Html.li "Ctrl-a selects everything."
        Html.li "Ctrl-c is copy."
        Html.li "Ctrl-v is paste."
        Html.li "Ctrl-x is cut."
        Html.li "Ctrl-z is undo."
        Html.li "Ctrl-Shift-z is redo."
        Html.li "Ctrl-MouseClick to open links."
        Html.li "Hold down Shift and move the cursor to select text. Esc to undo the selection."
        Html.li "Press F1 to open the command palette that shows all available commands and keyboard shortcuts."
      ]
    ]
  ]

[<ReactComponent>]
let RootDivComponent (model, dispatch) =
  let classes = useStyles ()
  Html.div [
    prop.className classes.RootDiv
    prop.children [
      Mui.container [
        prop.id MainContainerElementId
        container.component' "main"
        container.disableGutters true
        prop.children [
          Html.div [
            prop.children [
              headerElement model dispatch
              toolbarElement model dispatch classes
              tabsWithContentElement model dispatch
              contentBelowTabsElement
            ]
          ]
        ]
      ]
    ]
  ]

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
