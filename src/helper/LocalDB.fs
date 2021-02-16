module LocalDB

open Fable.Core
open Fable.Core.JS
open Browser.Types

type ILocalDB =
  abstract saveAsFileSupported: unit -> bool
  abstract saveAsFile: filename: string * data: string -> Promise<unit>

[<ImportAll("./LocalDB")>]
let DB: ILocalDB = jsNative
