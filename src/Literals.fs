module Literals

open Fable.Core

[<Literal>]
let MainContainerElementId = "main-container"

[<Emit("debugger")>]
let debugger () : unit = jsNative