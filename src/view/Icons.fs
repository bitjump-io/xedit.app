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

let inline addIcon b : ReactElement = 
  ofImport "default" "@material-ui/icons/Add" (keyValueList CaseRules.LowerFirst b) []

let inline closeIcon b : ReactElement =
  ofImport "default"  "@material-ui/icons/Close" (keyValueList CaseRules.LowerFirst b) []

let inline verticalBar b: ReactElement =
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

type private Mdi =
  static member inline icon props = createElement (importDefault "@mdi/react/Icon") props
  static member mouseLeftClickPath = "M539,203.646c-117.475,0-213,95.572-213,213.047v246.613c0,117.475,95.525,213.047,213,213.047s213-95.572,213-213.047  V416.693C752,299.219,656.475,203.646,539,203.646z M730,663.307c0,105.388-85.613,191.126-191,191.126s-191-85.738-191-191.126V533  h382V663.307z M730,511H549V225.846c101,5.357,181,88.908,181,190.848V511z"
  [<Import("mdiFormatFontSizeIncrease","@mdi/js")>]
  static member mdiFormatFontSizeIncreasePath = jsNative
  [<Import("mdiFormatFontSizeDecrease","@mdi/js")>]
  static member mdiFormatFontSizeDecreasePath = jsNative

// Props for Mdi.icon
[<Erase>]
// fsharplint:disable-next-line
type private icon =
  static member inline path (value: string) =  Interop.mkAttr "path" value
  static member inline size (value: string) =  Interop.mkAttr "size" value
  static member inline title (value: string) =  Interop.mkAttr "title" value
  static member inline flipHorizontal (value: bool) =  Interop.mkAttr "horizontal" value
  static member inline flipVertical (value: bool) =  Interop.mkAttr "vertical" value

let mouseLeftClickIcon = Mdi.icon [
  icon.path Mdi.mouseLeftClickPath
  icon.size "25px"
  prop.style [style.margin (-2, 0, -8, 0)]
  prop.viewBox (0, 0, 1080, 1080)
]

let mouseRightClickIcon = Mdi.icon [
  icon.path Mdi.mouseLeftClickPath
  icon.size "25px"
  prop.style [style.margin (-2, 0, -8, 0)]
  prop.viewBox (0, 0, 1080, 1080)
]

let fontSizeIncreaseIcon = Mdi.icon [
  icon.path Mdi.mdiFormatFontSizeIncreasePath
  icon.size "30px"
]
let fontSizeDecreaseIcon = Mdi.icon [
  icon.path Mdi.mdiFormatFontSizeDecreasePath
  icon.size "22px"
]
