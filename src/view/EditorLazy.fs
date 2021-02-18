module EditorLazy

open Feliz
open Fable.Core
open Fable.Core.JsInterop

let private asyncComponent: JS.Promise<unit -> ReactElement> = importDynamic "./Editor.fs"

[<ReactComponent>]
let EditorComponentLazy (dispatch) =
  // Without React.useMemo, the Editor component is created and disposed on every new rendering of the parent (tabBarElement).
  React.useMemo (
    (fun () -> 
      React.suspense (
        [Html.div [React.lazy' (asyncComponent, ({| dispatch = dispatch |}))]], 
        Html.text "loading")), 
    [||])
