module Editor

open Fable.Core
open Feliz
open Elmish
open Browser
open Model
open Msg
open Fable.Core.JsInterop
open DomEx

// The MonacoEditor as a react component.
[<JSX.Component>]
let EditorComponent (props: {| dispatch: Msg -> unit|}) =
  let dispatch = props.dispatch
  let divEl = React.useElementRef()
  React.useEffectOnce(fun () ->
    match divEl.current with
    | Some x -> 
      x.id <- "editorElem"
      let heightTillBottomScreen = int (window.innerHeight - x.getBoundingClientRect().top - 2.0)
      let height = if heightTillBottomScreen < 300 then 300 else heightTillBottomScreen
      (x :?> IHTMLElement).style.height <- ((string height) + "px")
      EditorDomElementCreated x.id |> dispatch
    | None -> ()
    React.createDisposable(fun () -> console.error("Should never dispose EditorComponent"))
  )
  Html.div [
    prop.style [style.position.relative]
    prop.children [
      // opacity of this div will be set to 0 shortly after page is loaded.
      Html.div [
        prop.id "underneathEditorElem"
        prop.style [style.position.absolute; style.margin 30; style.fontSize 32; style.color "#CCCCCC66"; style.transitionProperty "opacity"; style.transitionTimingFunction.easeIn; style.transitionDurationSeconds 2]
        prop.children [
          Html.text "Write here or drop files."
        ]
      ]
      Html.div [
        prop.style [style.display.block; style.width length.auto; style.height length.auto; style.minHeight 100; style.border (1, borderStyle.solid, "#858585"); style.zIndex 6]
        prop.ref divEl
      ]
    ]
  ]


exportDefault EditorComponent