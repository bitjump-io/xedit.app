module MonacoEditorTypes

open Browser.Types

type IRange =
  abstract startLineNumber: int with get
  abstract startColumn: int with get
  abstract endLineNumber: int with get
  abstract endColumn: int with get

type IModelContentChange =
  abstract range: IRange with get
  abstract rangeOffset: int with get
  abstract rangeLength: int with get
  abstract text: string with get

type IModelContentChangedEvent =
  abstract changes: IModelContentChange[] with get
  abstract eol: string with get
  abstract versionId: int with get
  abstract isUndoing: bool with get
  abstract isRedoing: bool with get
  abstract isFlush: bool with get

// fsharplint:disable-next-line
type Dimension = { width: int; height: int }

type IMonacoEditor =
  abstract currentTextModelId: int
  abstract increaseFontSize: unit -> unit
  abstract decreaseFontSize: unit -> unit
  abstract focus: unit -> unit
  abstract setWordWrap: value: bool -> unit
  abstract addTextModel: value: string * language: string -> int
  abstract layout: dimension: Dimension -> unit
  abstract setTextModelIndex: modelIndex: int -> unit
  abstract setLanguage: languageId: string -> unit
  abstract removeTextModel: modelIndex: int -> unit
  abstract getValue: modelIndex: int -> string
  abstract getLinesContent: modelIndex: int -> string[]
  abstract onDidChangeContent: modelIndex: int -> listener: (IModelContentChangedEvent -> unit) -> unit

type IExports =
  abstract create: elem: HTMLElement * dimension: Dimension -> IMonacoEditor
