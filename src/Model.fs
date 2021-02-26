module Model

open Feliz
open Thoth.Elmish
open Fable.Core
open MonacoEditorTypes

[<StringEnum>]
type EditorLanguage =
  | [<CompiledName("plaintext")>] PlainText
  | [<CompiledName("markdown")>] Markdown
  | [<CompiledName("javascript")>] JavaScript
  | [<CompiledName("typescript")>] TypeScript
  | [<CompiledName("json")>] Json
  | [<CompiledName("xml")>] Xml
  | [<CompiledName("yaml")>] Yaml
  | [<CompiledName("sql")>] Sql
  member x.displayText =
    match x with
    | PlainText -> "Plain text"
    | Markdown -> "Markdown"
    | JavaScript -> "JavaScript"
    | TypeScript -> "TypeScript"
    | Json -> "JSON"
    | Xml -> "XML"
    | Yaml -> "YAML"
    | Sql -> "SQL"
  static member all = [PlainText; Markdown; JavaScript; TypeScript; Json; Xml; Yaml; Sql]

type Height = | Height of int

type EditorOptions = { WrapText: bool }
  with 
    static member initial = { WrapText = false }

type DragModel = { DragenterCount: int; DragleaveCount: int }
  with 
    static member initial = { DragenterCount = 0; DragleaveCount = 0 }
    member x.isDragging = x.DragenterCount > x.DragleaveCount

type TabItemModel = { Name: string; ModelIndex: int; Language: EditorLanguage; UntitledIndex: int; ContentSize: int }
  with
    static member initial = { Name = "Untitled 1"; ModelIndex = 0; Language = PlainText; UntitledIndex = 1; ContentSize = 0 }

[<RequireQualifiedAccess>]
type ControlId =
  | None
  | WrapText
  | EditorLanguage
  | IncreaseFontSize
  | DecreaseFontSize

[<RequireQualifiedAccess>]
type ThemeKind =
  | Dark
  | Light

[<StringEnum>]
[<RequireQualifiedAccess>]
type OS =
  | [<CompiledName("Unknown")>] Unknown
  | [<CompiledName("Windows")>] Windows
  | [<CompiledName("Mac")>] Mac
  | [<CompiledName("Linux")>] Linux
  static member all = [Windows; Mac; Linux]

type CssClasses = { RootDiv: string }

type XIcon = { Element: ReactElement; Name: string}

// Model holds the current state.
type Model = 
  { SelectedTabId: int
    TabItems: TabItemModel list
    EditorHeight: int
    EditorOptions: EditorOptions
    EditorLanguage: EditorLanguage
    EditorDomElementId: string option
    IsMonacoEditorModulePromiseResolved: bool
    ShowTooltipControlId: ControlId
    WindowInnerWidth: float
    DevicePixelRatio: float
    Debouncer: Debouncer.State
    DragModel: DragModel
    ThemeKind: ThemeKind
    OS: OS
    ShowKeyBindingsFor: OS }

 // Not included in Model type because quite large.
let mutable monacoEditor: IMonacoEditor option = None