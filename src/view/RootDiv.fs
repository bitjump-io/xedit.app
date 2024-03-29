module RootDiv

open Feliz
open Feliz.MaterialUI
open Literals
open Styles
open Iconbar
open Tabbar
open Keybindings
open LocalDB
open MuiEx

[<ReactComponent>]
let RootDivComponent (model, dispatch) =
  let classes = useStyles ()
  Html.div [
    prop.className classes.RootDiv
    prop.children [
      Mui.container [
        prop.id MainContainerElementId
        container.component' "main"
        container.disableGutters true
        prop.children [
          Html.div [
            prop.children [
              toolbarElement model dispatch classes
              tabBarElement model dispatch classes
              keybindingsTable model dispatch classes
              Html.br []
              Html.text (string (DB.saveAsFileSupported ()))
              // The button is only included because without it the css class .MuiButton-root and .MuiButton-root:hover are not generated.
              // However these are also needed for the select element.
              Mui.button [
                button.variant.outlined
                prop.style [style.visibility.collapse]
              ]
            ]
          ]
        ]
      ]
    ]
  ]