module UiEx

// open Fable.Core
// open Feliz
// open Browser.Types
// open Feliz.prop
// open Feliz.svg
// open Fable.React

open System.ComponentModel
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Feliz
open Browser.Types
open MUI

[<JSX.Component>]
let TooltipX (title: string, showTooltip: bool, element: ReactElement) =
  JSX.jsx
    $"""
      import  {{ Tooltip }} from "@primer/react"

      <Tooltip direction="s">
        <div>
        {title}
        {element}
        </div>
      </Tooltip>
    """
 
[<JSX.Component>]
let ButtonOutlined (value: string, onClick: MouseEvent -> unit) =
  if value = null then failwith "value is null"

  JSX.jsx
    $"""
      import  {{ Button }} from "@primer/react"

      <Button variant="outline">
        {value}
      </Button>
    """

[<JSX.Component>]
let IconButtonX (style: #IStyleAttribute list, children: #ReactElement list, onClick: MouseEvent -> unit, onMouseEnter: MouseEvent -> unit, onMouseLeave: MouseEvent -> unit, onTouchStart: TouchEvent -> unit, onTouchEnd: TouchEvent -> unit) =

  JSX.jsx
    $"""
      import {{ IconButton }} from "@mui/material"
      import {{ WrapText as WrapTextIcon }} from "@mui/icons-material"


      <IconButton style={style} onClick={onClick} onMouseEnter={onMouseEnter} onMouseLeave={onMouseLeave} onTouchStart={onTouchStart} onTouchEnd={onTouchEnd}>
        {children}
      </IconButton>
    """

type UiEx =
  static member inline buttonOutlined (value: string, ?onClick: MouseEvent -> unit) =
    Mui.button [
      button.variant.outlined
      button.children value
      if onClick.IsSome
        then prop.onClick onClick.Value
    ]
  static member inline withTooltip (title: string, showTooltip: bool, element: ReactElement) =
    Mui.tooltip [
      MUI.tooltip.title title
      tooltip.arrow true
      tooltip.placement.bottom
      tooltip.children(element)
      tooltip.open' showTooltip
    ]