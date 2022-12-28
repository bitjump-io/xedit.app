module Styles

open Feliz
open Model
open HtmlEx

//inset = Interop.mkStyle "borderStyle" "inset"
//static member inline none = Interop.mkStyle "boxShadow" "none"

//type CssClasses = { RootDiv: string; TabButton: string; BorderLeft: string; ShowCloseBtnOnHover: string; CloseBtn: string; TabsScrollButton: string }

let paperColor = "#1e1e1e"

let rootDiv = [
    style.backgroundColor paperColor
    style.marginTop 4
    style.fontSize 16
    style.color "#ccc"
    style.height (length.percent 100)
    style.fontFamily "system-ui, -apple-system, BlinkMacSystemFont, Roboto, Helvetica, sans-serif"
    // Next line not needed but kept as an example for nested styles.
    //style.custom ("&:focus", styles.create [style.borderRadius 4; style.borderColor "#ff0000"; style.boxShadow (0, 0, 0, 20, "rgba(0,123,255,.25)")] )
    style.custom ("& kbd", 
        [
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

let tabButton = [
    style.custom ("borderRadius", "0 !important")
    style.custom ("padding", "6px 0 6px 16px !important")
    ]

let borderLeft = [
    style.custom ("borderLeft", "1px solid rgb(35, 35, 35) !important")
    ]

let showCloseBtnOnHover = [
    style.custom ("&:hover div", [style.color (color.rgb(133, 133, 133) + " !important")] )
    ]

let closeBtn = [
    style.custom ("color", "transparent !important")
    ]

let tabsScrollButton = [
    style.custom ("min-width", "0 !important")
    ]
