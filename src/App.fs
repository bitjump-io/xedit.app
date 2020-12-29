module App

open Feliz
open Elmish
open Feliz.MaterialUI
open Browser
open Browser.Types

// Holds the editor's width and size state.
type Dimension = { Width: int; Height: int }

type EditorOptions = { WrapText: bool }

// Model holds the current state.
type Model = { Count: int; Editor: ME.IMonacoEditor option; EditorDimension: Dimension; EditorOptions: EditorOptions }

// Msg is an action that we need to apply to the current state.
type Msg =
  | Increment
  | Decrement
  | ResizeEditor of Dimension
  | EditorCreated of ME.IMonacoEditor * Dimension
  | ToggleWrapText

// The init function will produce an initial state once the program starts running.  It can take any arguments.
let init () =
  { Count = 0;
    Editor = None;
    EditorDimension = { Width = 0; Height = 0 };
    EditorOptions = { WrapText = false }
  }, 
  Cmd.none

// Helper to update nested state.
let updateEditorOptions (msg: Msg) (state: EditorOptions) =
  match msg with
  | ToggleWrapText -> { state with WrapText = not state.WrapText }
  | _ -> failwith (sprintf "No case implemented to update EditorOptions for message %A" msg)

// The update function will receive the change required by Msg, and the current state. It will produce a new state and potentially new command(s).
let update (msg: Msg) (state: Model) =
  match msg with
  | Increment -> { state with Count = state.Count + 1 }, Cmd.none
  | Decrement -> { state with Count = state.Count - 1 }, Cmd.none
  | ResizeEditor dimension ->
    match state.Editor with
    | Some editorInst -> editorInst.layout({ width = dimension.Width; height = dimension.Height })
    | None -> ()
    { state with EditorDimension = dimension }, Cmd.none
  | EditorCreated (editorInst, dimension) -> { state with Editor = Some editorInst; EditorDimension = dimension }, Cmd.none
  | ToggleWrapText ->
    match state.Editor with
    | Some editorInst -> editorInst.setWordWrap(not state.EditorOptions.WrapText)
    | None -> ()
    { state with EditorOptions = updateEditorOptions msg state.EditorOptions }, Cmd.none

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

// The MonacoEditor as a react component.
let EditorComponent = React.functionComponent(fun (state, dispatch) ->
  let divEl = React.useElementRef()
  React.useEffectOnce(fun () ->
    let dimension = { Width = 800; Height = 600 }
    let editor = 
      match divEl.current with
      | Some x -> Some (ME.Editor.create(x, { width = dimension.Width; height = dimension.Height }))
      | None -> None
    if editor.IsSome then EditorCreated (editor.Value, dimension) |> dispatch
    React.createDisposable(fun () -> if editor.IsSome then editor.Value.dispose())
  )
  Html.div [
    prop.style [ style.display.inlineBlock; style.width length.auto; style.height length.auto; style.border (1, borderStyle.solid, color.white) ]
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
  static member withTooltip (title: string, element: ReactElement) =
    Mui.tooltip [
      tooltip.title title
      tooltip.enterDelay 700
      tooltip.children(element)
    ]

// Website markup definition.
let app = React.functionComponent(fun (state, dispatch) ->
  let classes = useStyles ()
  Mui.themeProvider [
    themeProvider.theme Themes.darkTheme
    themeProvider.children [
      Html.div [
        prop.className classes.rootDiv
        prop.children [
          Html.div [
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
                MuiEx.buttonOutlined ("test resize", fun _ -> ResizeEditor { Width = 900; Height = 400 } |> dispatch)
                Html.text (sprintf "Current editor size w: %i, h: %i" state.EditorDimension.Width state.EditorDimension.Height)
              ]
            ]
          ]
          Html.div [
            MuiEx.withTooltip (
              "Wrap text",
              Mui.iconButton [ 
                prop.onClick (fun _ -> dispatch ToggleWrapText)
                iconButton.children (Icons.wrapTextIcon [ if state.EditorOptions.WrapText then icon.color.secondary else () ]) 
              ])
          ]
          EditorComponent(state, dispatch)
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
)

let render (state: Model) (dispatch: Msg -> unit) =
  app (state, dispatch)
