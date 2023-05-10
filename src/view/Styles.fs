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
      style.marginTop 4
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
    ];
    TabButton = styles.create [
      style.custom ("borderRadius", "0 !important")
      style.custom ("padding", "6px 0 6px 16px !important")
    ];
    BorderLeft = styles.create [
      style.custom ("borderLeft", "1px solid rgb(35, 35, 35) !important")
    ];
    ShowCloseBtnOnHover = styles.create [
      style.custom ("&:hover div", styles.create [style.color (color.rgb(133, 133, 133) + " !important")] )
    ];
    CloseBtn = styles.create [
      style.custom ("color", "transparent !important")
    ];
    TabsScrollButton = styles.create [
      style.custom ("min-width", "0 !important")
    ];
    TabWrapper = styles.create [
      // Let tab width expand to tab label length.
      style.width (length.percent 100)
      style.paddingRight 2
      style.display.inlineFlex
      style.fontSize 16
    ];
    ButtonHover = styles.create [
      style.custom ("&:hover div", styles.create [style.backgroundColor (color.rgba(255, 255, 255, 0.08) + " !important")] )
    ];
  }
)
