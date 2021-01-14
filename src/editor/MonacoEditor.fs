/// This module contais F# declarations for the MonacoEditor TypeScript file.
/// See https://fable.io/docs/communicate/js-from-fable.html
module rec MonacoEditor
open Fable.Core
open Fable.Core.JS
open Browser.Types

type IExports =
  abstract create: elem: HTMLElement * dimension: Dimension -> IMonacoEditor
  abstract addTextModel: value: string * language: string -> editor: IMonacoEditor -> int
  abstract removeTextModel: modelIndex: int -> editor: IMonacoEditor -> unit
  abstract setTextModelIndex: modelIndex: int -> editor: IMonacoEditor -> unit
  abstract layout: dimension: Dimension -> editor: IMonacoEditor -> unit
  abstract setWordWrap: value: bool -> editor: IMonacoEditor -> unit
  abstract setLanguage: languageId: string -> editor: IMonacoEditor -> unit

[<ImportAll("./MonacoEditor")>]
let Editor: IExports = jsNative

type IMonacoEditor =
  abstract currentTextModelId: int

// fsharplint:disable-next-line
type Dimension = { width: int; height: int }
