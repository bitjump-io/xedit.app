module Header

open Feliz
open Feliz.MaterialUI
open HtmlEx
open Literals
open MuiEx
open Msg

let headerElement model dispatch =
  Html.div [
    Html.input [
      prop.id FileInputElementId
      prop.type' "file"
      prop.multiple true
      prop.style [style.display.none]
      prop.onChange (FilesAdded >> dispatch)
    ]
    Mui.typography [
      typography.variant.h2
      typography.children [
        Html.text "xedit.app - Client side "
        HtmlEx.redText "x"
        Html.text "tra large file "
        HtmlEx.redText "edit"
        Html.text "or app"
      ]
    ]
    Mui.typography [
      typography.variant.body1
      prop.style [style.marginBottom 10; style.marginTop 10]
      typography.children [
        Html.text "Drag & drop anywhere to open files or use the "
        MuiEx.buttonOutlined ("file picker", fun _ -> dispatch OpenFilePicker)
      ]
    ]
  ]