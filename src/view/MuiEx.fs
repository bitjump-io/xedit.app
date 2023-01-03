module MuiEx

open Feliz
open Feliz.MaterialUI
open Browser.Types

// Material UI extensions.
type MuiEx =
  static member inline buttonOutlined (value: string, ?onClick: MouseEvent -> unit) =
    Mui.button [
      button.variant.outlined
      button.children value
      if onClick.IsSome
        then prop.onClick onClick.Value
    ]
  static member inline withTooltip (title: string, showTooltip: bool, element: ReactElement) =
    Mui.tooltip [
      tooltip.title title
      tooltip.arrow true
      tooltip.placement.bottom
      tooltip.children(element)
      tooltip.open' showTooltip
    ]