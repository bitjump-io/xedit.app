module App

open Feliz
open Elmish
open Feliz.MaterialUI
open Browser
open Browser.Types
open Model
open Msg
open DomEx
open RootDiv

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
[<ReactComponent>]
let App (model: Model, dispatch) =
  React.useEffectOnce(fun () ->
    window.addEventListener("resize", fun _ -> WindowWidthChaned window.innerWidth |> dispatch)
    addDragAndDropListener dispatch
  )
  React.useEffect(
    (fun () -> 
      let editAreaElem = (document.querySelector(".monaco-editor-background") :?> IHTMLElement)
      let marginElem = (document.querySelector(".monaco-editor .margin") :?> IHTMLElement)
      if model.DragModel.isDragging then
        editAreaElem.style.backgroundColor <- "#737373"
        marginElem.style.backgroundColor <- "#737373"
      else
        editAreaElem.style.backgroundColor <- ""
        marginElem.style.backgroundColor <- ""
    ),
    [|model.DragModel.isDragging :> obj|])
  Mui.themeProvider [
    themeProvider.theme Themes.darkTheme
    themeProvider.children [
      RootDivComponent(model, dispatch)
    ]
  ]

let render (state: Model) (dispatch: Msg -> unit) =
  App (state, dispatch)
