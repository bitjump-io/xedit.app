module RootDiv

open Feliz
// open Literals
open Iconbar
open Tabbar
// open Keybindings
// open LocalDB
// open MuiEx

open Fable.Core
open Model

[<JSX.Component>]
let RootDivComponent (model, dispatch) =
  Html.div [
    prop.style Styles.rootDiv
    prop.children [
      // Mui.container [
      //   prop.id MainContainerElementId
      //   container.component' "main"
      //   container.disableGutters true
      //   prop.children [
      //     Html.div [
      //       prop.children [
      toolbarElement model dispatch
      tabBarElement model dispatch
      //         keybindingsTable model dispatch
      //         Html.br []
      //         Html.text (string (DB.saveAsFileSupported ()))
      //         // The button is only included because without it the css class .MuiButton-root and .MuiButton-root:hover are not generated.
      //         // However these are also needed for the select element.
      //         Mui.button [
      //           button.variant.outlined
      //           prop.style [style.visibility.collapse]
      //         ]
      //       ]
      //     ]
      //   ]
      // ]
    ]
  ]