module Tabbar

open Feliz
open Feliz.MaterialUI
open Model
open Msg
open Editor

let tabBarElement model dispatch (classes: CssClasses) =
  Mui.tabContext [
    tabContext.value (string model.SelectedTabId)
    tabContext.children [
      Mui.paper [
        prop.style [style.flexGrow 1; style.marginTop 5]
        paper.children [
          Html.div [
            prop.style [style.display.flex]
            prop.children [
              Mui.tabs [
                tabs.variant.scrollable
                tabs.value model.SelectedTabId
                tabs.onChange (TabChanged >> dispatch)
                tabs.indicatorColor.primary
                tabs.textColor.primary
                tabs.children [
                  for t in model.TabItems ->
                    Mui.tab [
                      tab.label t.Name
                    ]
                ]
              ]
              Icons.VerticalBar [
              ]
              Mui.iconButton [ 
                prop.style [style.height 38; style.width 38; style.margin.auto; style.marginLeft 1; style.marginRight 5]
                prop.onClick (fun _ -> dispatch AddEmptyTab)
                iconButton.children (Icons.AddIcon []) 
              ]
            ]
          ]
        ]
      ]
      Html.div [
        EditorComponent(model, dispatch)
      ]
    ]
  ]