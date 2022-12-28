module Iconbar

open Feliz
open UiEx
open Model
open Msg
open EventHelper
open System
open Fable.Core
open Fable.React
open Feliz
open Browser.Types


let inline toJsx (el: ReactElement) : JSX.Element = unbox el
let inline toReact (el: JSX.Element) : ReactElement = unbox el

[<JSX.Component>]
let TodoView () =

  // JSX.jsx
  //   $"""
  //     import {{ IconButton }} from "@mui/material"
  //     import {{ WrapTextIcon }} from "@mui/icons-material/WrapText"

  //     <IconButton>
  //       <WrapTextIcon />
  //     </IconButton>
  //   """
  JSX.jsx
    $"""
      <span>TODOVIEW SPAN</span>
    """

let toolbarElement model dispatch =
  Html.div [
    prop.children [
      UiEx.TooltipX (
        "Wrap text",
        model.ShowTooltipControlId = ControlId.WrapText,
        toReact (TodoView ())) |> toReact
        // Mui.iconButton [ 
        //   prop.style [style.verticalAlign.bottom; style.height 38; style.width 38; style.marginRight 5; style.borderRadius 5]
        //   prop.onClick (fun _ -> dispatch ToggleWrapText; (ShowTooltipChanged ControlId.None) |> dispatch)
        //   prop.onMouseEnter (fun _ -> (ShowTooltipChanged ControlId.WrapText) |> dispatch)
        //   prop.onTouchStart (fun _ -> (ShowTooltipChanged ControlId.WrapText) |> dispatch)
        //   prop.onMouseLeave (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
        //   prop.onTouchEnd (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
        //   iconButton.children (Icons.wrapTextIcon [ if model.EditorOptions.WrapText then icon.color.primary else () ]) 
        // ])
      // UiEx.TooltipX (
      //   "Language",
      //   model.ShowTooltipControlId = ControlId.EditorLanguage,
        // Mui.formControl [
        //   formControl.size.small
        //   formControl.variant.outlined
        //   formControl.children [
            // Mui.select [
            //   // Add class for hover effect.
            //   select.classes.root "MuiButton-root"
            //   select.value model.EditorLanguage
            //   select.onChange (EditorLanguageChanged >> dispatch)
            //   select.onOpen (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
            //   select.onClose (fun _ -> 
            //       let f = fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch
            //       // Defer hiding tooltip because Firefox fires onMouseEnter event after the select element is closed if clicked out of the list.
            //       setTimeout f 20)
            //   prop.onMouseEnter (fun _ -> (ShowTooltipChanged ControlId.EditorLanguage) |> dispatch)
            //   prop.onTouchStart (fun _ -> (ShowTooltipChanged ControlId.EditorLanguage) |> dispatch)
            //   prop.onMouseLeave (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
            //   prop.onTouchEnd (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
            //   select.input (
            //     Mui.inputBase []
            //   )
            //   select.children [
            //     for el in EditorLanguage.all -> 
            //       Mui.menuItem [
            //         prop.value (unbox<string> el)
            //         menuItem.children [
            //           Html.text el.displayText
            //         ]
            //       ]
            //   ]
            // ]
          //]
        //]
      //)
      UiEx.TooltipX (
        "Increase Font Size",
        model.ShowTooltipControlId = ControlId.IncreaseFontSize,
        UiEx.IconButtonX () |> toReact
        // [ 
        //   prop.style [style.verticalAlign.bottom; style.height 38; style.width 38; style.marginLeft 5; style.marginRight 0; style.padding 2; style.borderRadius 5]
        //   prop.onClick (fun _ -> dispatch IncreaseFontSize; (ShowTooltipChanged ControlId.None) |> dispatch)
        //   prop.onMouseEnter (fun _ -> (ShowTooltipChanged ControlId.IncreaseFontSize) |> dispatch)
        //   prop.onTouchStart (fun _ -> (ShowTooltipChanged ControlId.IncreaseFontSize) |> dispatch)
        //   prop.onMouseLeave (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
        //   prop.onTouchEnd (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
        //   iconButton.children (Icons.fontSizeIncreaseIcon()) 
        // ]
        ) |> toReact
      UiEx.TooltipX (
        "Decrease Font Size",
        model.ShowTooltipControlId = ControlId.DecreaseFontSize,
        UiEx.IconButtonX () |> toReact
        // [ 
        //   prop.style [style.verticalAlign.bottom; style.height 38; style.width 38; style.marginLeft 0; style.marginRight 5; style.padding 2; style.paddingTop 6; style.borderRadius 5]
        //   prop.onClick (fun _ -> dispatch DecreaseFontSize; (ShowTooltipChanged ControlId.None) |> dispatch)
        //   prop.onMouseEnter (fun _ -> (ShowTooltipChanged ControlId.DecreaseFontSize) |> dispatch)
        //   prop.onTouchStart (fun _ -> (ShowTooltipChanged ControlId.DecreaseFontSize) |> dispatch)
        //   prop.onMouseLeave (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
        //   prop.onTouchEnd (fun _ -> (ShowTooltipChanged ControlId.None) |> dispatch)
        //   iconButton.children (Icons.fontSizeDecreaseIcon()) 
        // ]
        ) |> toReact
    ]
  ]