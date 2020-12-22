/// This module contais F# declarations for the MonacoEditor TypeScript file.
/// See https://fable.io/docs/communicate/js-from-fable.html
module rec MonacoEditor
open Fable.Core
open Fable.Core.JS
open Browser.Types

type IExports =
    abstract create: elem: HTMLElement -> IMonacoEditor

[<ImportAll("./MonacoEditor")>]
let Editor: IExports = jsNative

type IMonacoEditor =
    abstract dispose: unit -> unit
