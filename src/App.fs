module App

open Feliz
open Elmish
open Feliz.MaterialUI
open Browser
open Browser.Types
open Thoth.Elmish
open System
open MonacoEditor

[<Literal>]
let FileInputElementId = "file-input"

[<Literal>]
let MainContainerElementId = "main-container"

type Height = | Height of int

type EditorOptions = { WrapText: bool }

// Model holds the current state.
type Model = { SelectedTabId: int; Editor: IMonacoEditor option; EditorHeight: int; EditorOptions: EditorOptions; WindowInnerWidth: float; DevicePixelRatio: float; Debouncer: Debouncer.State }

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

// The init function will produce an initial state once the program starts running.  It can take any arguments.
let init () =
  { Editor = None
    EditorHeight = 0
    EditorOptions = { WrapText = false }
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

// Helper to update nested state.
let updateEditorOptions (msg: Msg) (model: EditorOptions) =
  match msg with
  | ToggleWrapText -> { model with WrapText = not model.WrapText }, Cmd.none
  | _ -> failwith (sprintf "No case implemented to update EditorOptions for message %A" msg)

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
    printfn "files added %A" (files.[0].name)
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
    let mainContainerWidth = int (document.getElementById(MainContainerElementId).clientWidth)
    let widthAdjustment = -2 // Needed so main-content and editor get the same computed width.
    Option.iter (Editor.layout({ width = mainContainerWidth + widthAdjustment; height = model.EditorHeight })) model.Editor
    model, Cmd.none
  | TabChanged selectedTabId ->
    { model with SelectedTabId = selectedTabId }, Cmd.none

// Styles documentation links
// - https://github.com/cmeeren/Feliz.MaterialUI/blob/master/docs-app/public/pages/samples/sign-in/SignIn.fs
// - https://cmeeren.github.io/Feliz.MaterialUI/#usage/styling
// - https://material-ui.com/styles/basics/

let useStyles = Styles.makeStyles(fun styles theme ->
  {|
    rootDiv = styles.create [
      style.backgroundColor "#1e1e1e"
      style.padding 10
      style.fontSize 16
      style.color "#fff"
      style.fontFamily "system-ui, -apple-system, BlinkMacSystemFont, Roboto, Helvetica, sans-serif"
    ]
  |}
)

// The MonacoEditor as a react component.
let EditorComponent = React.functionComponent(fun (state, dispatch) ->
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
  static member inline withTooltip (title: string, element: ReactElement) =
    Mui.tooltip [
      tooltip.title title
      tooltip.enterDelay 700
      tooltip.children(element)
    ]

let calcMainContainerMaxWidth model =
  let normalizedWindowWidth =  int (model.WindowInnerWidth / model.DevicePixelRatio)
  let normalizedMainContainerWidth =
    match normalizedWindowWidth with
    | x when x < 1000 -> x
    | x when x >= 1000 -> 1000
    | x -> x
  int (float normalizedMainContainerWidth * model.DevicePixelRatio)

// Website markup definition.
let app = React.functionComponent(fun (model, dispatch) ->
  React.useEffectOnce(fun () ->
    window.addEventListener("resize", fun _ -> WindowWidthChaned window.innerWidth |> dispatch)
  )
  let mainContainerWidth = calcMainContainerMaxWidth model
  printfn "test %i" mainContainerWidth
  let classes = useStyles ()
  Mui.themeProvider [
    themeProvider.theme Themes.darkTheme
    themeProvider.children [
      Html.div [
        prop.className classes.rootDiv
        prop.children [
          Html.main [
            prop.id MainContainerElementId
            prop.style [style.position.relative; style.marginLeft length.auto; style.marginRight length.auto; style.width (length.percent 100); style.maxWidth mainContainerWidth]
            prop.children [
              Html.div [
                prop.children [
                  Html.div [
                    Html.input [
                      prop.id FileInputElementId
                      prop.type' "file"
                      prop.multiple true
                      prop.style [style.display.none]
                      prop.onChange (FilesAdded >> dispatch)
                    ]
                    Html.div [
                      prop.style [style.fontSize 24]
                      prop.children [
                        Html.text "Drag & drop anywhere to open files or use the "
                        MuiEx.buttonOutlined ("file picker", fun _ -> dispatch OpenFilePicker)
                        let mainContainerElem = document.getElementById(MainContainerElementId)
                        let mainContainerWidth = if isNull mainContainerElem then 0 else int (mainContainerElem.clientWidth)
                        Html.text (sprintf "Window width: %i, editor height: %i, main container width: %i, " (int model.WindowInnerWidth) model.EditorHeight mainContainerWidth)
                      ]
                    ]
                  ]
                  Html.div [
                    MuiEx.withTooltip (
                      "Wrap text",
                      Mui.iconButton [ 
                        prop.onClick (fun _ -> dispatch ToggleWrapText)
                        iconButton.children (Icons.wrapTextIcon [ if model.EditorOptions.WrapText then icon.color.primary else () ]) 
                      ])
                  ]
                  Mui.tabContext [
                    tabContext.value (string model.SelectedTabId)
                    tabContext.children [
                      Mui.paper [
                        prop.style [style.flexGrow 1]
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
                  Html.div [
                    Html.text "Tipps"
                    Html.ul [
                      Html.li "To see the context menu, right-click with the mouse."
                      Html.li "Select columns (column mode) by holding down Shift + Alt, then click and drag with the mouse."
                      Html.li "Use multiple cursors by holding down Alt, then click with the mouse."
                    ]
                  ]
                ]
              ]
            ]
          ]
        ]
      ]
    ]
  ]
)

let render (state: Model) (dispatch: Msg -> unit) =
  app (state, dispatch)
