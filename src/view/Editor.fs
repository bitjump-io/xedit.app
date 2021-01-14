module Editor

open Feliz
open Elmish
open Browser
open Model
open Msg
open MonacoEditor

// Not included in model because quite large.
let mutable monacoEditor: IMonacoEditor option = None

// The MonacoEditor as a react component.
[<ReactComponent>]
let EditorComponent (model, dispatch) =
  let divEl = React.useElementRef()
  React.useEffectOnce(fun () ->
    let editor = 
      match divEl.current with
      | Some x -> 
        let width = int x.clientWidth
        let height = 600
        monacoEditor <- Some(Editor.create(x, { width = width; height = height }))
        EditorCreated (Height height) |> dispatch
        monacoEditor
      | None -> None
    React.createDisposable(fun () -> console.error("Should never dispose EditorComponent"))
  )
  Html.div [
    prop.style [style.display.block; style.width length.auto; style.height length.auto; style.minHeight 100; style.border (1, borderStyle.solid, "#858585")]
    prop.ref divEl
  ]