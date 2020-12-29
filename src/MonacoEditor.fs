/// This module contais F# declarations for the MonacoEditor TypeScript file.
/// See https://fable.io/docs/communicate/js-from-fable.html
module rec ME // MonacoEditor
open Fable.Core
open Fable.Core.JS
open Browser.Types

type IExports =
  abstract create: elem: HTMLElement * dimension: Dimension -> IMonacoEditor

[<ImportAll("./MonacoEditor")>]
let Editor: IExports = jsNative

type IMonacoEditor =
  abstract dispose: unit -> unit
  abstract layout: dimension: Dimension -> unit
  abstract setWordWrap: value: string -> unit

// fsharplint:disable-next-line
type Dimension = { width: int; height: int }
