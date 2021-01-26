module Styles

open Feliz
open Feliz.MaterialUI
open Model
open HtmlEx

// Styles documentation links
// - https://github.com/cmeeren/Feliz.MaterialUI/blob/master/docs-app/public/pages/samples/sign-in/SignIn.fs
// - https://cmeeren.github.io/Feliz.MaterialUI/#usage/styling
// - https://material-ui.com/styles/basics/

//inset = Interop.mkStyle "borderStyle" "inset"
//static member inline none = Interop.mkStyle "boxShadow" "none"

let useStyles: unit -> CssClasses = Styles.makeStyles(fun styles theme ->
  {
    RootDiv = styles.create [
      style.backgroundColor theme.palette.background.paper
      style.padding 10
      style.fontSize 16
      style.color "#ccc"
      style.height (length.percent 100)
      style.fontFamily "system-ui, -apple-system, BlinkMacSystemFont, Roboto, Helvetica, sans-serif"
      // Next line not needed but kept as an example for nested styles.
      //style.custom ("&:focus", styles.create [style.borderRadius 4; style.borderColor "#ff0000"; style.boxShadow (0, 0, 0, 20, "rgba(0,123,255,.25)")] )
      style.custom ("& kbd", 
        styles.create [
          style.backgroundColor("hsla(0,0%,50%,.47)")
          style.padding (1, 2, 1, 2)
          style.marginRight 2
          style.marginLeft 2
          style.verticalAlign.middle
          style.border (1, borderStyle.solid, color.transparent)
          //style.borderBottomColor (color.rgba (68, 68, 68, 0.6))
          style.borderRadius 3
          //StyleEx.boxShadowInset (0, -1, 0, 0, "rgba(68,68,68,.6)")
          ])
    ]
  }
)
