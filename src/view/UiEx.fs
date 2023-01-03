module UiEx

open Fable.Core
open Feliz
open Browser.Types
open Feliz.prop
open Feliz.svg

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


// [<JSX.Component>]
// let IconButtonX () =

//   JSX.jsx
//     $"""
//       import {{ IconButton }} from "@mui/material"
//       import {{ WrapTextIcon }} from "@mui/icons-material/WrapText"

//       <IconButton>
//         <WrapTextIcon />
//       </IconButton>
//     """

[<JSX.Component>]
let IconButtonX () =

  JSX.jsx
    $"""
      <span>hello iconbuttonx</span>
    """

// Material UI extensions.
// type UiEx =
//   static member inline buttonOutlined (value: string, ?onClick: MouseEvent -> unit) =
//     Mui.button [
//       button.variant.outlined
//       button.children value
//       if onClick.IsSome
//         then prop.onClick onClick.Value
//     ]
  // static member inline withTooltip (title: string, showTooltip: bool, element: ReactElement) =
  //   Mui.tooltip [
  //     tooltip.title title
  //     tooltip.arrow true
  //     tooltip.placement.bottom
  //     tooltip.children(element)
  //     tooltip.open' showTooltip
  //   ]