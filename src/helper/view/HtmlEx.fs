module HtmlEx

// Extensions for both the Feliz.Html module and other DOM helper functions.

open Feliz
open Browser
open Browser.Types
open Icons

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
  | Home
  | End

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
  | Key.Home -> "Home"
  | Key.End -> "End"

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
  | Key.Home -> "Home"
  | Key.End -> "End"

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
  static member inline multiKey (k1: Key, k2: Key, k3: Key) =
    Html.span [
        prop.title (asText(k1) + "+" + asText(k2) + "+" + asText(k3))
        prop.children [
          Html.kbd (asSymbol(k1))
          Html.kbd (asSymbol(k2))
          Html.kbd (asSymbol(k3))
        ]
      ]
  static member inline keyWithMouseLeft (k1: Key) =
    Html.span [
        prop.title (asText(k1) + "+" + "MouseLeftButton")
        prop.children [
          Html.kbd (asSymbol(k1))
          mouseLeftClickIcon()
        ]
      ]
  static member inline keyWithMouseScroll (k1: Key) =
    Html.span [
        prop.title (asText(k1) + "+" + "MouseScroll")
        prop.children [
          Html.kbd (asSymbol(k1))
          mouseScrollIcon()
        ]
      ]
  static member inline keysWithMouseLeft (k1: Key, k2: Key) =
    Html.span [
        prop.title (asText(k1) + "+" + asText(k2) + "+" + "MouseLeftButton")
        prop.children [
          Html.kbd (asSymbol(k1))
          Html.kbd (asSymbol(k2))
          mouseLeftClickIcon()
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
  static member inline ctrlShift (k: Key) = Kbd.multiKey (Key.Ctrl, Key.Shift, k)
  static member inline cmdMacShift (k: Key) = Kbd.multiKey (Key.CmdMac, Key.Shift, k)
  static member inline cmdMacOpt (k: Key) = Kbd.multiKey (Key.CmdMac, Key.OptMac, k)
  static member inline alt () = Kbd.singleKey (Key.Alt)
  static member inline alt (k: Key) = Kbd.multiKey (Key.Alt, k)
  static member inline altWithMouseLeft () = Kbd.keyWithMouseLeft (Key.Alt)
  static member inline optMacWithMouseLeft () = Kbd.keyWithMouseLeft (Key.OptMac)
  static member inline altWithMouseScroll () = Kbd.keyWithMouseScroll (Key.Alt)
  static member inline optMacWithMouseScroll () = Kbd.keyWithMouseScroll (Key.OptMac)
  static member inline optMac () = Kbd.singleKey (Key.OptMac)
  static member inline optMac (k: Key) = Kbd.multiKey (Key.OptMac, k)
  static member inline cmdMac () = Kbd.singleKey (Key.CmdMac)
  static member inline cmdMac (k: Key) = Kbd.multiKey (Key.CmdMac, k)
  static member inline shift () = Kbd.singleKey (Key.Shift)
  static member inline shift (k: Key) = Kbd.multiKey (Key.Shift, k)
  static member inline shiftAlt (k: Key) = Kbd.multiKey (Key.Shift, Key.Alt, k)
  static member inline shiftOptMac (k: Key) = Kbd.multiKey (Key.Shift, Key.OptMac, k)
  static member inline shiftAltWithMouseLeft () = Kbd.keysWithMouseLeft (Key.Shift, Key.Alt)
  static member inline shiftOptMacWithMouseLeft () = Kbd.keysWithMouseLeft (Key.Shift, Key.OptMac)

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