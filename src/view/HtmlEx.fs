module HtmlEx

// Extensions for both the Feliz.Html module and other DOM helper functions.

open Feliz
open Browser
open Browser.Types

type HtmlEx =
  static member inline redText (value: string) =
    Html.span [
      prop.style [style.color.red]
      prop.children [Html.text value]
    ]

let getElementById id =
  let el = document.getElementById(id)
  if isNull el then 
    None
  else 
    Some el

let click (el: HTMLElement) =
  el.click()