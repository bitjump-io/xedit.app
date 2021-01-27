module Tabbar

open Feliz
open Feliz.MaterialUI
open Model
open Msg
open Editor
open Browser

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
                tabs.children (
                  model.TabItems
                  |> List.mapi (fun tabIndex t ->
                    Mui.tab [
                      // Add class for hover effect.
                      tab.classes.root "MuiButton-root"
                      tab.disableRipple true
                      tab.label [
                        Html.span [
                          prop.key "1"
                          prop.style [style.flexGrow 1]
                          prop.children [ Html.text t.Name ]
                        ]
                        Mui.iconButton [ 
                          prop.key "2"
                          iconButton.component' "div"
                          prop.style [style.height 20; style.width 20]
                          prop.onClick (fun e -> RemoveTab tabIndex |> dispatch; e.stopPropagation())
                          iconButton.children (Icons.closeIcon [
                            prop.style [style.height 16; style.width 18]
                          ]) 
                        ]
                      ]
                    ]
                    ))
              ]
              Icons.verticalBar [
              ]
              Mui.iconButton [ 
                prop.style [style.height 38; style.width 38; style.margin.auto; style.marginLeft 1; style.marginRight 5]
                prop.onClick (fun _ -> dispatch AddEmptyTab)
                iconButton.children (Icons.addIcon []) 
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