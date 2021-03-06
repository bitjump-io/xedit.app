module Editor

open Feliz
open Elmish
open Browser
open Model
open Msg
open Fable.Core.JsInterop
open DomEx

// The MonacoEditor as a react component.
[<ReactComponent>]
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
    prop.style [style.display.block; style.width length.auto; style.height length.auto; style.minHeight 100; style.border (1, borderStyle.solid, "#858585")]
    prop.ref divEl
  ]

exportDefault EditorComponent