module Icons

open Feliz
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Feliz.MaterialUI

let inline gitHubIcon b : ReactElement = 
  ofImport "default" "@material-ui/icons/GitHub" (keyValueList CaseRules.LowerFirst b) []

let inline wrapTextIcon b : ReactElement = 
  ofImport "default" "@material-ui/icons/WrapText" (keyValueList CaseRules.LowerFirst b) []

let inline AddIcon b : ReactElement = 
  ofImport "default" "@material-ui/icons/Add" (keyValueList CaseRules.LowerFirst b) []

let inline CloseIcon b : ReactElement =
  ofImport "default"  "@material-ui/icons/Close" (keyValueList CaseRules.LowerFirst b) []

let inline VerticalBar b: ReactElement =
  Mui.svgIcon [
    prop.stroke color.white
    prop.strokeWidth 1
    prop.viewBox (0, 0, 3, 50)
    prop.style [style.height 38; style.margin.auto; style.marginLeft 0; style.marginRight 0]
    prop.children [
      Svg.line [
        Interop.svgAttribute "x1" "50%"
        Interop.svgAttribute "y1" "15%"
        Interop.svgAttribute "x2" "50%"
        Interop.svgAttribute "y2" "85%"
      ]
    ]
  ]