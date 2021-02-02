module JavaScriptHelper

open Fable.Core

[<Emit("!!$0")>]
let forceBool(value: obj): bool = jsNative