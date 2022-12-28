module App

open Fable.Core
open Browser
open Fable.React
open Feliz
open Elmish
open Browser.Types
open Model
open Msg
open System
open DomEx
open RootDiv
open Fable.Core.JsInterop
open HtmlEx
open EventHelper

let addDragAndDropListener (dispatch: Msg -> unit) =
  document.addEventListener("dragover", fun e ->
    e.stopPropagation()
     // Without preventDefault, the dragged file is opened by the browser.
    e.preventDefault()
    (e :?> DragEvent).dataTransfer.dropEffect <- "copy"
  )

  document.addEventListener("dragenter", fun e ->
    e.stopPropagation()
    e.preventDefault()
    dispatch OnDragenter
  )

  document.addEventListener("dragleave", fun e ->
    e.stopPropagation()
    e.preventDefault()
    dispatch OnDragleave
  )

  document.addEventListener("drop", fun e ->
    e.stopPropagation()
    e.preventDefault()
    let fileList = (e :?> DragEvent).dataTransfer.files
    let files = [for i in 0..(fileList.length - 1) -> fileList.item(i)]
    OnDrop files |> dispatch
  )

// Website markup definition.
[<JSX.Component>]
let App2 (model: Model, dispatch) =
  React.useEffectOnce(fun () ->
    (window :?> IWindow).performance.mark("AppStart")
    window.addEventListener("resize", fun _ -> WindowWidthChaned window.innerWidth |> dispatch)
    addDragAndDropListener dispatch

    importDynamic "../../src/editor/MonacoEditor.ts"
    |> Promise.map (fun _ -> dispatch MonacoEditorModulePromiseResolved)
    |> Promise.catch(fun ex -> console.log(ex))
    |> ignore

    window.addEventListener("load", fun _ -> 
      console.log("domContentLoaded", (window :?> IWindow).performance.timing.domContentLoadedEventStart - (window :?> IWindow).performance.timing.navigationStart)
      console.log("domComplete", (window :?> IWindow).performance.timing.domComplete - (window :?> IWindow).performance.timing.navigationStart)
      (window :?> IWindow).performance.getEntriesByType("mark") |> Seq.iter (fun entry -> console.log(entry.name, entry.startTime))
      
      setTimeout (fun _ -> (getElementValueById("underneathEditorElem") :?> IHTMLElement).style.opacity <- "0") 3000
    )
  )
  React.useEffect(
    (fun () -> 
      let editAreaElem = (document.querySelector(".monaco-editor-background") :?> IHTMLElement)
      let marginElem = (document.querySelector(".monaco-editor .margin") :?> IHTMLElement)
      if isNull (editAreaElem :> obj) || isNull (marginElem :> obj) then
        ()
      elif model.DragModel.isDragging then
        editAreaElem.style.backgroundColor <- "#737373"
        marginElem.style.backgroundColor <- "#737373"
      else
        editAreaElem.style.backgroundColor <- ""
        marginElem.style.backgroundColor <- ""
    ),
    [|model.DragModel.isDragging :> obj|])
  React.useEffect(
    (fun () ->
      let newColor = 
        match model.ThemeKind with
        | ThemeKind.Dark -> "#1e1e1e"
        | ThemeKind.Light -> ""
      (document.body :?> IHTMLElement).style.backgroundColor <- newColor
    ),
    [|model.ThemeKind :> obj|])
  RootDivComponent(model, dispatch)
  // Html.div [
  //   Html.text "piep"
  // ]
  // Mui.themeProvider [
  //   themeProvider.theme Themes.darkTheme
  //   themeProvider.children [
  //     RootDivComponent(model, dispatch)
  //   ]
  // ]

// let render (state: Model) (dispatch: Msg -> unit) =
//   App (state, dispatch)

// module private Elmish =
//     type State = int

//     let init (count: int) = 1, Cmd.none

//     let update (msg: Msg) (state: State) =
//       2, Cmd.none

open Elmish
open Elmish.React
open Elmish.Debug
open Elmish.HMR

[<JSX.Component>]
let App () =
    let program () =
        Program.mkProgram Update.init Update.update (fun _ _ -> ())
        #if DEBUG
        |> Program.withDebuggerAt (Debugger.Remote("localhost",5173))
        #endif

    let model, dispatch = React.useElmish (program, ()) //(init, update, arg = 2)

    JSX.jsx
        $"""
    <div className="container mx-5 mt-5 is-max-desktop">
        <p className="title">My Todos</p>
        {App2 (model, dispatch)}
    </div>
    """