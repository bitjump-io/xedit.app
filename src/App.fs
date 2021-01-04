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
  | PlainText
  | JavaScript
  member x.displayText =
    match x with
    | PlainText -> "Plain text"
    | JavaScript -> "JavaScript"

type EditorOptions = { WrapText: bool }

[<RequireQualifiedAccess>]
type ControlId =
  | None
  | WrapText
  | EditorLanguage

type EditorLanguageTooltip = { ControlId: ControlId }

// Model holds the current state.
type Model = 
  { SelectedTabId: int
    Editor: IMonacoEditor option
    EditorHeight: int
    EditorOptions: EditorOptions
    EditorLanguage: EditorLanguage
    ShowTooltip: EditorLanguageTooltip
    WindowInnerWidth: float
    DevicePixelRatio: float
    Debouncer: Debouncer.State }

// Msg is an action that we need to apply to the current state.
type Msg =
  | EditorCreated of IMonacoEditor * Height
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
  { Editor = None
    EditorHeight = 0
    EditorOptions = { WrapText = false }
    EditorLanguage = PlainText
    ShowTooltip = { ControlId = ControlId.None }
    WindowInnerWidth = window.innerWidth
    DevicePixelRatio = window.devicePixelRatio
    Debouncer = Debouncer.create()
    SelectedTabId = 0
  }, 
  Cmd.none

let getElementById id =
  let el = document.getElementById(id)
  if isNull el then
    eprintfn "Element with id '%s' is null" id
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

  // Helper to update nested state.
let updateEditorLanguageTooltip (msg: Msg) (model: EditorLanguageTooltip) =
  match msg with
  | ShowTooltipChanged controlId ->
    { model with ControlId = controlId }, Cmd.none
  | _ -> failwith (sprintf "No case implemented to update EditorLanguageTooltip for message %A" msg)

// The update function will receive the change required by Msg, and the current state. It will produce a new state and potentially new command(s).
let update (msg: Msg) (model: Model) =
  match msg with
  | EditorCreated (editorInst, Height height) -> { model with Editor = Some editorInst; EditorHeight = height }, Cmd.none
  | ToggleWrapText ->
    Option.iter (Editor.setWordWrap(not model.EditorOptions.WrapText)) model.Editor
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
      Promise.iter (fun text -> Option.iter (Editor.setValue(text)) model.Editor) contentPromise
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
    let mainContainerWidth = if getElementById(MainContainerElementId).IsSome then int (getElementById(MainContainerElementId).Value.clientWidth) else 100 // todo
    let widthAdjustment = -2 // Needed so main-content and editor get the same computed width.
    Option.iter (Editor.layout({ width = mainContainerWidth + widthAdjustment; height = model.EditorHeight })) model.Editor
    model, Cmd.none
  | TabChanged selectedTabId ->
    { model with SelectedTabId = selectedTabId }, Cmd.none
  | EditorLanguageChanged editorLanguage ->
    { model with EditorLanguage = editorLanguage }, Cmd.none
  | ShowTooltipChanged _ ->
    let (editorLanguageTooltipModel, editorLanguageTooltipCmd) = updateEditorLanguageTooltip msg model.ShowTooltip
    { model with ShowTooltip = editorLanguageTooltipModel }, editorLanguageTooltipCmd


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

let calcMainContainerIntendedWidth model =
  match int model.WindowInnerWidth with
  | x when x < 1100 -> x
  | x when x >= 1100 -> 1100
  | x -> x

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
        let editor = Editor.create(x, { width = width; height = height })
        EditorCreated (editor, Height height) |> dispatch
        Some editor
      | None -> None
    React.createDisposable(fun () -> if editor.IsSome then Editor.dispose(editor.Value))
  )
  Html.div [
    prop.style [ style.display.block; style.width length.auto; style.height length.auto; style.minHeight 100; style.border (1, borderStyle.solid, "#858585") ]
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
    Html.div [
      prop.style [style.fontSize 24; style.marginBottom 10]
      prop.children [
        Html.text "Drag & drop anywhere to open files or use the "
        MuiEx.buttonOutlined ("file picker", fun _ -> dispatch OpenFilePicker)
        let mainContainerElem = getElementById MainContainerElementId
        let mainContainerWidth = if mainContainerElem.IsSome then int (mainContainerElem.Value.clientWidth) else 0
        Html.text (sprintf "Window width: %i, editor height: %i, main container width: %i, " (int model.WindowInnerWidth) model.EditorHeight mainContainerWidth)
      ]
    ]
  ]

let toolbarElement model dispatch (classes: CssClasses) =
  Html.div [
    prop.style [style.marginTop 14]
    prop.children [
      MuiEx.withTooltip (
        "Wrap text",
        model.ShowTooltip.ControlId = ControlId.WrapText,
        Mui.iconButton [ 
          prop.style [style.verticalAlign.bottom; style.height 38; style.width 38; style.marginRight 5]
          prop.onClick (fun _ -> dispatch ToggleWrapText)
          prop.onMouseEnter (fun _ -> (ShowTooltipChanged ControlId.WrapText) |> dispatch)
          prop.onMouseLeave (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
          iconButton.children (Icons.wrapTextIcon [ if model.EditorOptions.WrapText then icon.color.primary else () ]) 
        ])
      MuiEx.withTooltip (
        "Language",
        model.ShowTooltip.ControlId = ControlId.EditorLanguage,
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
                Mui.menuItem [
                  prop.value (unbox<string> PlainText)
                  menuItem.children [
                    Html.text PlainText.displayText
                  ]
                ]
                Mui.menuItem [
                  prop.value (unbox<string> JavaScript)
                  menuItem.children [
                    Html.text JavaScript.displayText
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
  let mainContainerWidth = calcMainContainerIntendedWidth model
  Html.div [
    prop.className classes.RootDiv
    prop.children [
      Html.main [
        prop.id MainContainerElementId
        prop.style [style.position.relative; style.marginLeft length.auto; style.marginRight length.auto; style.width (length.percent 100); style.maxWidth mainContainerWidth]
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
