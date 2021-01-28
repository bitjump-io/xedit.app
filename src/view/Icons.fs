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
  static member mouseScrollPath = "M48.004,35.177V16.824l-4.59,4.59c-0.781,0.781-2.046,0.781-2.828,0c-0.782-0.781-0.782-2.047,0-2.828l8-8   c0.781-0.781,2.047-0.781,2.828,0l8,8c0.781,0.781,0.781,2.047,0,2.828C59.023,21.805,58.512,22,58,22s-1.023-0.195-1.414-0.586   l-4.596-4.596v18.363l4.596-4.595c0.781-0.782,2.047-0.782,2.828,0c0.781,0.781,0.781,2.046,0,2.828l-8,8   C51.023,41.805,50.512,42,50,42s-1.023-0.195-1.414-0.586l-8-8c-0.782-0.781-0.782-2.047,0-2.828c0.781-0.782,2.046-0.782,2.828,0   L48.004,35.177z M86,36v28c0,19.85-16.149,36-35.998,36c-19.85,0-35.998-16.15-35.998-36V50.019C14.004,50.012,14,50.006,14,50   s0.004-0.012,0.004-0.019V36c0-19.851,16.148-36,35.998-36C69.851,0,86,16.149,86,36z M18.004,36v12H82V36   C82,18.355,67.646,4,50.002,4C32.358,4,18.004,18.355,18.004,36z M82,64V52H18.004v12c0,17.645,14.354,32,31.998,32   C67.646,96,82,81.645,82,64z"
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

let mouseLeftClickIcon() = Mdi.icon [
  icon.path Mdi.mouseLeftClickPath
  icon.size "25px"
  prop.style [style.margin (-2, 0, -8, 0)]
  prop.viewBox (0, 0, 1080, 1080)
]

let mouseRightClickIcon() = Mdi.icon [
  icon.path Mdi.mouseLeftClickPath
  icon.size "25px"
  prop.style [style.margin (-2, 0, -8, 0)]
  prop.viewBox (0, 0, 1080, 1080)
]

let mouseScrollIcon() = Mdi.icon [
  icon.path Mdi.mouseScrollPath
  icon.size "20px"
  prop.style [style.margin (0, 0, -6, 0)]
  prop.viewBox (0, 0, 100, 100)
]

let fontSizeIncreaseIcon() = Mdi.icon [
  icon.path Mdi.mdiFormatFontSizeIncreasePath
  icon.size "30px"
]
let fontSizeDecreaseIcon() = Mdi.icon [
  icon.path Mdi.mdiFormatFontSizeDecreasePath
  icon.size "22px"
]
