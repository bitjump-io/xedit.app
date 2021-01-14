module Styles

open Feliz
open Feliz.MaterialUI
open Model

// Styles documentation links
// - https://github.com/cmeeren/Feliz.MaterialUI/blob/master/docs-app/public/pages/samples/sign-in/SignIn.fs
// - https://cmeeren.github.io/Feliz.MaterialUI/#usage/styling
// - https://material-ui.com/styles/basics/

let useStyles: unit -> CssClasses = Styles.makeStyles(fun styles theme ->
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