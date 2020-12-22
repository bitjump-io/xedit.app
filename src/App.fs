module App

open Feliz
open Elmish
//open Zanaptak.TypedCssClasses
open Feliz.MaterialUI

type State = { Count: int }

type Msg =
    | Increment
    | Decrement

let init() = { Count = 0 }, Cmd.none

let update (msg: Msg) (state: State) =
    match msg with
    | Increment -> { state with Count = state.Count + 1 }, Cmd.none
    | Decrement -> { state with Count = state.Count - 1 }, Cmd.none

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

let EditorComponent = React.functionComponent(fun (model, dispatch) ->
  let divEl = React.useElementRef()
  React.useEffectOnce(fun () ->
    let editor =
      if divEl.current.IsSome
        then Some (MonacoEditor.Editor.create divEl.current.Value)
      else None
    React.createDisposable(fun () -> if editor.IsSome then editor.Value.dispose())
  )
  Html.div [
    prop.style [ style.height 300; style.width 600 ]
    prop.ref divEl
  ]
)

let app = React.functionComponent(fun (model, dispatch) ->
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
                        prop.style [ style.fontSize 24 ]
                        prop.children [
                            Html.text "Drag & drop anywhere to open files or use the "
                            Mui.button [
                                button.variant.outlined
                                button.children "file picker"
                            ]
                        ]
                    ]
                    EditorComponent(model, dispatch)
                ]
            ]
        ]
    ]
)

//let render model dispatch =

let render (state: State) (dispatch: Msg -> unit) =
  app (state, dispatch)

// let getSample (key: string) =
//   React.elmishComponent("SignIn", init, update, render, key)