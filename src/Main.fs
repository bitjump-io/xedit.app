module Main

// open Elmish
// open Elmish.React
// open Elmish.Debug
// open Elmish.HMR
open Fable.Core
open Browser
open Fable.React

// // App
// Program.mkProgram Update.init Update.update App.render
// #if DEBUG
// |> Program.withDebuggerAt (Debugger.Remote("localhost",5173))
// #endif
// //|> Program.withReactSynchronous "feliz-app"
// |> Program.run


let inline toReact (el: JSX.Element) : ReactElement = unbox el

// Entry point must be in a separate file
// for Vite Hot Reload to work

[<JSX.Component>]
let App () = App.App()

let root = ReactDomClient.createRoot (document.getElementById ("app-container"))
root.render (App() |> toReact)