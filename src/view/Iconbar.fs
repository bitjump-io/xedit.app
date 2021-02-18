module Iconbar

open Feliz
open Feliz.MaterialUI
open MuiEx
open Model
open Msg
open EventHelper

let toolbarElement model dispatch (classes: CssClasses) =
  Html.div [
    prop.style [style.marginTop 14]
    prop.children [
      MuiEx.withTooltip (
        "Wrap text",
        model.ShowTooltipControlId = ControlId.WrapText,
        Mui.iconButton [ 
          prop.style [style.verticalAlign.bottom; style.height 38; style.width 38; style.marginRight 5; style.borderRadius 5]
          prop.onClick (fun _ -> dispatch ToggleWrapText; (ShowTooltipChanged ControlId.None) |> dispatch)
          prop.onMouseEnter (fun _ -> (ShowTooltipChanged ControlId.WrapText) |> dispatch)
          prop.onTouchStart (fun _ -> (ShowTooltipChanged ControlId.WrapText) |> dispatch)
          prop.onMouseLeave (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
          prop.onTouchEnd (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
          iconButton.children (Icons.wrapTextIcon [ if model.EditorOptions.WrapText then icon.color.primary else () ]) 
        ])
      MuiEx.withTooltip (
        "Language",
        model.ShowTooltipControlId = ControlId.EditorLanguage,
        Mui.formControl [
          formControl.size.small
          formControl.variant.outlined
          formControl.children [
            Mui.select [
              // Add class for hover effect.
              select.classes.root "MuiButton-root"
              select.value model.EditorLanguage
              select.onChange (EditorLanguageChanged >> dispatch)
              select.onOpen (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
              select.onClose (fun _ -> 
                  let f = fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch
                  // Defer hiding tooltip because Firefox fires onMouseEnter event after the select element is closed if clicked out of the list.
                  setTimeout f 20)
              prop.onMouseEnter (fun _ -> (ShowTooltipChanged ControlId.EditorLanguage) |> dispatch)
              prop.onTouchStart (fun _ -> (ShowTooltipChanged ControlId.EditorLanguage) |> dispatch)
              prop.onMouseLeave (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
              prop.onTouchEnd (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
              select.input (
                Mui.inputBase []
              )
              select.children [
                for el in EditorLanguage.all -> 
                  Mui.menuItem [
                    prop.value (unbox<string> el)
                    menuItem.children [
                      Html.text el.displayText
                    ]
                  ]
              ]
            ]
          ]
        ]
      )
      MuiEx.withTooltip (
        "Increase Font Size",
        model.ShowTooltipControlId = ControlId.IncreaseFontSize,
        Mui.iconButton [ 
          prop.style [style.verticalAlign.bottom; style.height 38; style.width 38; style.marginLeft 5; style.marginRight 0; style.padding 2; style.borderRadius 5]
          prop.onClick (fun _ -> dispatch IncreaseFontSize; (ShowTooltipChanged ControlId.None) |> dispatch)
          prop.onMouseEnter (fun _ -> (ShowTooltipChanged ControlId.IncreaseFontSize) |> dispatch)
          prop.onTouchStart (fun _ -> (ShowTooltipChanged ControlId.IncreaseFontSize) |> dispatch)
          prop.onMouseLeave (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
          prop.onTouchEnd (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
          iconButton.children (Icons.fontSizeIncreaseIcon()) 
        ])
      MuiEx.withTooltip (
        "Decrease Font Size",
        model.ShowTooltipControlId = ControlId.DecreaseFontSize,
        Mui.iconButton [ 
          prop.style [style.verticalAlign.bottom; style.height 38; style.width 38; style.marginLeft 0; style.marginRight 5; style.padding 2; style.paddingTop 6; style.borderRadius 5]
          prop.onClick (fun _ -> dispatch DecreaseFontSize; (ShowTooltipChanged ControlId.None) |> dispatch)
          prop.onMouseEnter (fun _ -> (ShowTooltipChanged ControlId.DecreaseFontSize) |> dispatch)
          prop.onTouchStart (fun _ -> (ShowTooltipChanged ControlId.DecreaseFontSize) |> dispatch)
          prop.onMouseLeave (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
          prop.onTouchEnd (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
          iconButton.children (Icons.fontSizeDecreaseIcon()) 
        ])
    ]
  ]