module RootDiv

open Feliz
open Feliz.MaterialUI
open Literals
open Styles
open Header
open Iconbar
open Tabbar
open Helptext

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
              headerElement model dispatch
              toolbarElement model dispatch classes
              tabBarElement model dispatch classes
              helptextElement
            ]
          ]
        ]
      ]
    ]
  ]