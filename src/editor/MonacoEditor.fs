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
  abstract increaseFontSize: editor: IMonacoEditor -> unit
  abstract decreaseFontSize: editor: IMonacoEditor -> unit
  abstract setLanguage: languageId: string -> editor: IMonacoEditor -> unit
  abstract focus: editor: IMonacoEditor -> unit
  abstract getValue: modelIndex: int -> editor: IMonacoEditor -> string
  abstract getLinesContent: modelIndex: int -> editor: IMonacoEditor -> string[]
  abstract onDidChangeContent: modelIndex: int -> listener: (IModelContentChangedEvent -> unit) -> editor: IMonacoEditor -> unit

[<ImportAll("./MonacoEditor")>]
let Editor: IExports = jsNative

type IMonacoEditor =
  abstract currentTextModelId: int

type IModelContentChangedEvent =
  abstract changes: IModelContentChange[] with get
  abstract eol: string with get
  abstract versionId: int with get
  abstract isUndoing: bool with get
  abstract isRedoing: bool with get
  abstract isFlush: bool with get

type IModelContentChange =
  abstract range: IRange with get
  abstract rangeOffset: int with get
  abstract rangeLength: int with get
  abstract text: string with get

type IRange =
  abstract startLineNumber: int with get
  abstract startColumn: int with get
  abstract endLineNumber: int with get
  abstract endColumn: int with get

// fsharplint:disable-next-line
type Dimension = { width: int; height: int }
