module Editor

open Feliz
open Elmish
open Browser
open Model
open Msg
open MonacoEditor
open Fable.Core.JsInterop

// The MonacoEditor as a react component.
[<ReactComponent>]
let EditorComponent (props: {| dispatch: Msg -> unit|}) =
  let dispatch = props.dispatch
  let divEl = React.useElementRef()
  React.useEffectOnce(fun () ->
    let editor = 
      match divEl.current with
      | Some x -> 
        let width = int x.clientWidth
        let heightTillBottomScreen = int (window.innerHeight - x.getBoundingClientRect().top - 2.0)
        let height = if heightTillBottomScreen < 300 then 300 else heightTillBottomScreen
        monacoEditor <- Some(Editor.create(x, { width = width; height = height }))
        EditorCreated (Height height) |> dispatch
        //Editor.onDidChangeContent 0 (fun x -> throttle 3000 (fun _ -> console.log("editor changed save content."))) monacoEditor.Value
        //Editor.onDidChangeContent 0 (fun e -> console.log("editor content changed. ", e.versionId, e.changes.[0])) monacoEditor.Value
        //Editor.onDidChangeContent 0 (fun e -> console.log("size change: ", e.changes |> Seq.sumBy (fun c -> c.text.Length - c.rangeLength))) monacoEditor.Value
        Editor.onDidChangeContent 0 (fun e -> ModelContentChange e.changes |> dispatch) monacoEditor.Value
        monacoEditor
      | None -> None
    React.createDisposable(fun () -> console.error("Should never dispose EditorComponent"))
  )
  Html.div [
    prop.style [style.display.block; style.width length.auto; style.height length.auto; style.minHeight 100; style.border (1, borderStyle.solid, "#858585")]
    prop.ref divEl
  ]

exportDefault EditorComponent