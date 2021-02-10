module LocalDB

open Fable.Core
open Fable.Core.JS
open Browser.Types

type ILocalDB =
  abstract saveAsFileSupported: unit -> bool

[<ImportAll("./LocalDB")>]
let DB: ILocalDB = jsNative
