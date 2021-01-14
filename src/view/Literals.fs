module Literals

open Fable.Core

[<Literal>]
let FileInputElementId = "file-input"

[<Literal>]
let MainContainerElementId = "main-container"

[<Emit("debugger")>]
let debugger () : unit = jsNative