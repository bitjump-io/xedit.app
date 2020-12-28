module App

open Feliz
open Elmish
//open Zanaptak.TypedCssClasses
open Feliz.MaterialUI
open Elmish
open Feliz.ElmishComponents
open MonacoEditor
open Browser
open Browser.Types

// Holds the editor's width and size state. Also used to update the editor's size.
type Dimension =
  { w: int; h: int }
  interface IDimension with
      member x.width: int = x.w
      member x.height: int = x.h

// Model holds the current state.
type Model = { count: int; editor: option<IMonacoEditor>; editorDimension: Dimension }

// Msg is an action that we need to apply to the current state.
type Msg =
  | Increment
  | Decrement
  | ResizeEditor of Dimension
  | EditorCreated of IMonacoEditor * Dimension

// The init function will produce an initial state once the program starts running.  It can take any arguments.
let init () = { count = 0; editor = None; editorDimension = { w = 0; h = 0} }, Cmd.none

// The update function will receive the change required by Msg, and the current state. It will produce a new state and potentially new command(s).
let update (msg: Msg) (state: Model) =
  match msg with
  | Increment -> { state with count = state.count + 1 }, Cmd.none
  | Decrement -> { state with count = state.count - 1 }, Cmd.none
  | ResizeEditor dimension ->
    state.editor.Value.layout dimension
    { state with editorDimension = dimension }, Cmd.none
  | EditorCreated (editorInst, dimension) -> { state with editor = Some editorInst; editorDimension = dimension }, Cmd.none

// Themes documentation links
// - https://cmeeren.github.io/Feliz.MaterialUI/#usage/themes
// - https://material-ui.com/customization/theming/
let defaultTheme = Styles.createMuiTheme()

let darkTheme = Styles.createMuiTheme([
  theme.palette.type'.dark
  theme.palette.primary Colors.lightBlue
  theme.palette.secondary Colors.pink
  theme.palette.background.default' defaultTheme.palette.grey.``900``
  theme.typography.h1.fontSize "3rem"
  theme.typography.h2.fontSize "2rem"
  theme.typography.h3.fontSize "1.5rem"

  theme.overrides.muiAppBar.colorDefault [
    style.backgroundColor defaultTheme.palette.grey.A400
  ]
  theme.overrides.muiPaper.root [
    style.backgroundColor defaultTheme.palette.grey.A400
  ]
  theme.overrides.muiDrawer.paper [
    style.backgroundColor defaultTheme.palette.grey.``900``
  ]
  theme.props.muiAppBar [
    appBar.color.default'
  ]
])

// Styles documentation links
// - https://github.com/cmeeren/Feliz.MaterialUI/blob/master/docs-app/public/pages/samples/sign-in/SignIn.fs
// - https://cmeeren.github.io/Feliz.MaterialUI/#usage/styling
// - https://material-ui.com/styles/basics/

let useStyles = Styles.makeStyles(fun styles theme ->
  {|
    rootDiv = styles.create [
      style.backgroundColor "#333333"
      style.height (length.percent 100)
      style.padding 10
      style.fontSize 16
      style.color "#fff"
      style.fontFamily "system-ui, -apple-system, BlinkMacSystemFont, Roboto, Helvetica, sans-serif"
    ]
  |}
)

let EditorComponent = React.functionComponent(fun (state, dispatch) ->
  let divEl = React.useElementRef()
  React.useEffectOnce(fun () ->
    let dimension = { w = 800; h = 600 }
    let editor = 
      match divEl.current with
        | Some x -> Some (Editor.create (x, dimension))
        | None -> None
    if editor.IsSome then (EditorCreated (editor.Value, dimension) |> dispatch)
    React.createDisposable(fun () -> if editor.IsSome then editor.Value.dispose())
  )
  Html.div [
    prop.style [ style.display.inlineBlock; style.width length.auto; style.height length.auto; style.border (1, borderStyle.solid, color.white) ]
    prop.ref divEl
  ]
)

type MuiEx =
  static member inline buttonOutlined (value: string, ?onClick: MouseEvent -> unit) =
    Mui.button [
      button.variant.outlined
      button.children value
      if onClick.IsSome
        then prop.onClick onClick.Value
    ]

let app = React.functionComponent(fun (state, dispatch) ->
  let classes = useStyles ()
  Mui.themeProvider [
    themeProvider.theme darkTheme
    themeProvider.children [
      Html.div [
        prop.className classes.rootDiv
        prop.children [
          Html.input [
            prop.type' "file"
            prop.multiple true
            prop.style [style.display.none]
          ]
          Html.div [
            prop.style [style.fontSize 24]
            prop.children [
              Html.text "Drag & drop anywhere to open files or use the "
              MuiEx.buttonOutlined "file picker"
              MuiEx.buttonOutlined ("test resize", fun _ -> ResizeEditor { w = 900; h = 400 } |> dispatch)
              Html.text (sprintf "Current editor size w: %i, h: %i" state.editorDimension.w state.editorDimension.h)
            ]
          ]
          EditorComponent(state, dispatch)
        ]
      ]
    ]
  ]
)

let render (state: Model) (dispatch: Msg -> unit) =
  app (state, dispatch)
