module Themes
open Fable.Core.JsInterop
open Feliz
open Feliz.MaterialUI

// Themes documentation links
// - https://cmeeren.github.io/Feliz.MaterialUI/#usage/themes
// - https://material-ui.com/customization/theming/
let defaultTheme = Styles.createMuiTheme()

let darkTheme = Styles.createMuiTheme([
  theme.palette.type'.dark
  theme.palette.primary Colors.blue
  theme.palette.secondary Colors.teal
  theme.palette.background.default' defaultTheme.palette.grey.``900``
  theme.palette.background.paper "#1e1e1e"
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
  theme.overrides.muiInputBase.input [
    style.borderRadius 4
    style.position.relative
    style.backgroundColor "#1e1e1e" //theme.palette.background.paper //backgroundColor
    style.border (1, borderStyle.solid, "#ced4da")
    style.fontSize 16
    style.padding (8, 24, 8, 10)
    Interop.mkStyle "transition" (defaultTheme.transitions.create ([|"border-color"; "box-shadow"|]))
    style.custom ("&:focus", [style.borderRadius 4; style.borderColor "#ff0000"; style.boxShadow (0, 0, 0, 20, "rgba(0,123,255,.25)")] ) //&$focus 
  ]
  theme.overrides.muiInputBase.inputMarginDense [
    style.paddingTop length.auto
  ]
])
