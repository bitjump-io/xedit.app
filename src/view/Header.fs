module Header

open Feliz
open Feliz.MaterialUI
open HtmlEx
open Literals
open MuiEx
open Msg

let headerElement model dispatch =
    Mui.typography [
      typography.variant.h2
      typography.children [
        HtmlEx.redText "x"
        Html.text "edit.app - where "
        HtmlEx.redText "x"
        Html.text " = every file"
      ]
    ]
  ]