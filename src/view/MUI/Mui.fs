// Partly copied from https://github.com/Shmew/Feliz.MaterialUI/blob/master/src/Feliz.MaterialUI/Mui.fs

namespace MUI

open System.ComponentModel
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Feliz

[<AutoOpen; EditorBrowsable(EditorBrowsableState.Never)>]
module MuiHelpers =

  let reactElement (el: ReactElementType) (props: 'a) : ReactElement =
    import "createElement" "react"

  let reactElementTag (tag: string) (props: 'a) : ReactElement =
    import "createElement" "react"

  let createElement (el: ReactElementType) (properties: IReactProperty seq) : ReactElement =
    reactElement el (!!properties |> Object.fromFlatEntries)

  let createElementTag (tag: string) (properties: IReactProperty seq) : ReactElement =
    reactElementTag tag (!!properties |> Object.fromFlatEntries)

[<Erase>]
type Mui =
    static member inline iconButton props = createElement (importDefault "@mui/material/IconButton") props

    static member inline iconButton (children: #seq<ReactElement>) = createElement (importDefault "@mui/material/IconButton") [ MUI.iconButton.children (children :> ReactElement seq) ]

    static member inline tooltip props = createElement (importDefault "@mui/material/Tooltip") props

    static member inline button props = createElement (importDefault "@material-ui/core/Button") props

    static member inline button (children: #seq<ReactElement>) = createElement (importDefault "@material-ui/core/Button") [ MUI.button.children (children :> ReactElement seq) ]