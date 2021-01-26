module HtmlEx

// Extensions for both the Feliz.Html module and other DOM helper functions.

open Feliz
open Browser
open Browser.Types
open Feliz.MaterialUI
open Fable.Core.JsInterop
open Fable.Core

type Mdi =
  static member inline icon props = createElement (importDefault "@mdi/react/Icon") props
  static member mouseLeftClickPath = "M539,203.646c-117.475,0-213,95.572-213,213.047v246.613c0,117.475,95.525,213.047,213,213.047s213-95.572,213-213.047  V416.693C752,299.219,656.475,203.646,539,203.646z M730,663.307c0,105.388-85.613,191.126-191,191.126s-191-85.738-191-191.126V533  h382V663.307z M730,511H549V225.846c101,5.357,181,88.908,181,190.848V511z"

[<Erase>]
// fsharplint:disable-next-line
type icon =
  static member inline path (value: string) =  Interop.mkAttr "path" value
  static member inline size (value: string) =  Interop.mkAttr "size" value
  static member inline title (value: string) =  Interop.mkAttr "title" value
  static member inline flipHorizontal (value: bool) =  Interop.mkAttr "horizontal" value
  static member inline flipVertical (value: bool) =  Interop.mkAttr "vertical" value

let iconMouseLeftClick = Mdi.icon [
  icon.path Mdi.mouseLeftClickPath
  icon.size "25px"
  prop.style [style.margin (-2, 0, -8, 0)]
  prop.viewBox (0, 0, 1080, 1080)
]

let iconMouseRightClick = Mdi.icon [
  icon.path Mdi.mouseLeftClickPath
  icon.size "29px"
  icon.flipHorizontal true
  prop.style [style.margin (-2, 0, -10, 0)]
  prop.viewBox (0, 0, 1080, 1080)
]

[<RequireQualifiedAccess>]
type Key =
  | A
  | B
  | C
  | D
  | E
  | F
  | G
  | H
  | I
  | J
  | K
  | L
  | M
  | N
  | O
  | P
  | Q
  | R
  | S
  | T
  | U
  | V
  | W
  | X
  | Y
  | Z
  | LeftArrow
  | RightArrow
  | UpArrow
  | DownArrow
  | Shift
  | Ctrl
  | Alt
  | CtrlMac
  | OptMac
  | CmdMac

let inline asSymbol (key: Key) =
  match key with
  | Key.A -> "a"
  | Key.B -> "b"
  | Key.C -> "c"
  | Key.D -> "d"
  | Key.E -> "e"
  | Key.F -> "f"
  | Key.G -> "g"
  | Key.H -> "h"
  | Key.I -> "i"
  | Key.J -> "j"
  | Key.K -> "k"
  | Key.L -> "l"
  | Key.M -> "m"
  | Key.N -> "n"
  | Key.O -> "o"
  | Key.P -> "p"
  | Key.Q -> "q"
  | Key.R -> "r"
  | Key.S -> "s"
  | Key.T -> "t"
  | Key.U -> "u"
  | Key.V -> "v"
  | Key.W -> "w"
  | Key.X -> "x"
  | Key.Y -> "y"
  | Key.Z -> "z"
  | Key.LeftArrow -> "←"
  | Key.RightArrow -> "→"
  | Key.UpArrow -> "↑"
  | Key.DownArrow -> "↓"
  | Key.Shift -> "⇧"
  | Key.Ctrl -> "Ctrl"
  | Key.Alt -> "Alt"
  | Key.CtrlMac -> "⌃"
  | Key.OptMac -> "⌥"
  | Key.CmdMac -> "⌘"

let inline asText (key: Key) =
  match key with
  | Key.A -> "a"
  | Key.B -> "b"
  | Key.C -> "c"
  | Key.D -> "d"
  | Key.E -> "e"
  | Key.F -> "f"
  | Key.G -> "g"
  | Key.H -> "h"
  | Key.I -> "i"
  | Key.J -> "j"
  | Key.K -> "k"
  | Key.L -> "l"
  | Key.M -> "m"
  | Key.N -> "n"
  | Key.O -> "o"
  | Key.P -> "p"
  | Key.Q -> "q"
  | Key.R -> "r"
  | Key.S -> "s"
  | Key.T -> "t"
  | Key.U -> "u"
  | Key.V -> "v"
  | Key.W -> "w"
  | Key.X -> "x"
  | Key.Y -> "y"
  | Key.Z -> "z"
  | Key.LeftArrow -> "LeftArrow"
  | Key.RightArrow -> "RightArrow"
  | Key.UpArrow -> "UpArrow"
  | Key.DownArrow -> "DownArrow"
  | Key.Shift -> "Shift"
  | Key.Ctrl -> "Ctrl"
  | Key.Alt -> "Alt"
  | Key.CtrlMac -> "control"
  | Key.OptMac -> "option"
  | Key.CmdMac -> "command"

// Methods on Kbd
type Kbd =
  static member inline multiKey (k1: Key, k2: Key) =
    Html.span [
        prop.title (asText(k1) + "+" + asText(k2))
        prop.children [
          Html.kbd (asSymbol(k1))
          Html.kbd (asSymbol(k2))
        ]
      ]
  static member inline keyWithMouseLeft (k1: Key) =
    Html.span [
        prop.title (asText(k1) + "+" + "MouseLeftButton")
        prop.children [
          Html.kbd (asSymbol(k1))
          iconMouseLeftClick
        ]
      ]
  static member inline singleKey (k: Key) =
    Html.span [
        prop.title (asText(k))
        prop.children [
          Html.kbd (asSymbol(k))
        ]
      ]
  static member inline ctrlMac () = Kbd.singleKey (Key.CtrlMac)
  static member inline ctrlMac (k: Key) = Kbd.multiKey (Key.CtrlMac, k)
  static member inline ctrl () = Kbd.singleKey (Key.Ctrl)
  static member inline ctrl (k: Key) = Kbd.multiKey (Key.Ctrl, k)
  static member inline alt () = Kbd.singleKey (Key.Alt)
  static member inline alt (k: Key) = Kbd.multiKey (Key.Alt, k)
  static member inline altWithMouseLeft () = Kbd.keyWithMouseLeft (Key.Alt)
  static member inline optMac () = Kbd.singleKey (Key.OptMac)
  static member inline optMac (k: Key) = Kbd.multiKey (Key.OptMac, k)
  static member inline cmdMac () = Kbd.singleKey (Key.CmdMac)
  static member inline cmdMac (k: Key) = Kbd.multiKey (Key.CmdMac, k)
  static member inline shift () = Kbd.singleKey (Key.Shift)
  static member inline shift (k: Key) = Kbd.multiKey (Key.Shift, k)

type HtmlEx =
  static member inline redText (value: string) =
    Html.span [
      prop.style [style.color.red]
      prop.children [Html.text value]
    ]

type StyleEx =
  static member inline boxShadowInset(horizontalOffset: int, verticalOffset: int, blur: int, spread: int, color: string) =
    Interop.mkStyle "boxShadow" (
        "inset " +
        (unbox<string> horizontalOffset) + "px " +
        (unbox<string> verticalOffset) + "px " +
        (unbox<string> blur) + "px " +
        (unbox<string> spread) + "px " +
        color
    )

type WrappedElem =
  { ValuesReversed: ReactElement list }
  static member empty() = { ValuesReversed = [] }
  static member inline (++) (we: WrappedElem, el: ReactElement) =
    { we with ValuesReversed = (el :: we.ValuesReversed) }
  static member inline (++) (we: WrappedElem, str: string) =
    { we with ValuesReversed = (Html.text str :: we.ValuesReversed) }
  static member inline (++) (we: WrappedElem, _: unit) =
    List.rev we.ValuesReversed

let wrapper() =
  WrappedElem.empty()

let getElementById id =
  let el = document.getElementById(id)
  if isNull el then 
    None
  else 
    Some el

let click (el: HTMLElement) =
  el.click()