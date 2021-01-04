module App

open Feliz
open Elmish
open Feliz.MaterialUI
open Browser
open Browser.Types
open Thoth.Elmish
open System
open MonacoEditor
open Fable.Core

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
    EditorHeight: int
    EditorOptions: EditorOptions
    EditorLanguage: EditorLanguage
    ShowTooltipControlId: ControlId
    WindowInnerWidth: float
    DevicePixelRatio: float
    Debouncer: Debouncer.State }

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
  | EditorLanguageChanged of EditorLanguage
  | ShowTooltipChanged of ControlId

// The init function will produce an initial state once the program starts running.  It can take any arguments.
let init () =
  { EditorHeight = 0
    EditorOptions = { WrapText = false }
    EditorLanguage = PlainText
    ShowTooltipControlId = ControlId.None
    WindowInnerWidth = window.innerWidth
    DevicePixelRatio = window.devicePixelRatio
    Debouncer = Debouncer.create()
    SelectedTabId = 0
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

// Helpers to update nested state.
let updateEditorOptions (msg: Msg) (model: EditorOptions) =
  match msg with
  | ToggleWrapText -> { model with WrapText = not model.WrapText }, Cmd.none
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
  | FilesAdded files ->
    if not files.IsEmpty then
      printfn "files added %A" (files.[0].name)
      let contentPromise = FileTools.readAsText (0, files.[0])
      Promise.iter (fun text -> Option.iter (Editor.setValue(text)) monacoEditor) contentPromise
    model, Cmd.none
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
    { model with SelectedTabId = selectedTabId }, Cmd.none
  | EditorLanguageChanged editorLanguage ->
    Option.iter (Editor.setLanguage(unbox<string> editorLanguage)) monacoEditor 
    { model with EditorLanguage = editorLanguage }, Cmd.none
  | ShowTooltipChanged controlId ->
    { model with ShowTooltipControlId = controlId }, Cmd.none

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
    React.createDisposable(fun () -> if editor.IsSome then Editor.dispose(editor.Value))
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
        MuiEx.redText "x"
        Html.text "tra large file "
        MuiEx.redText "edit"
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
              Mui.tab [
                tab.label "tab 1"
              ]
              Mui.tab [
                tab.label "tab 2"
              ]
            ]
          ]
        ]
      ]
      Mui.tabPanel [
        prop.style [style.padding 0]
        tabPanel.value "0"
        tabPanel.children [
          EditorComponent(model, dispatch)
        ]
      ]
      Mui.tabPanel [
        tabPanel.value "1"
        tabPanel.children [
          Html.text "Just some plain text"
        ]
      ]
    ]
  ]

let contentBelowTabsElement =
  Html.div [
    prop.style [style.marginTop 5]
    prop.children [
      Html.text "Tipps"
      Html.ul [
        Html.li "To see the context menu, right-click with the mouse."
        Html.li "Select columns (column mode) by holding down Shift + Alt, then click and drag with the mouse."
        Html.li "Use multiple cursors by holding down Alt, then click with the mouse."
      ]
    ]
  ]

[<ReactComponent>]
let rootDivComponent (model, dispatch) =
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

// Website markup definition.
[<ReactComponent>]
let app (model, dispatch) =
  React.useEffectOnce(fun () ->
    window.addEventListener("resize", fun _ -> WindowWidthChaned window.innerWidth |> dispatch)
  )
  Mui.themeProvider [
    themeProvider.theme Themes.darkTheme
    themeProvider.children [
      rootDivComponent(model, dispatch)
    ]
  ]

let render (state: Model) (dispatch: Msg -> unit) =
  app (state, dispatch)
