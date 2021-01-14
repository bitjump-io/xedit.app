module Model

open Thoth.Elmish
open Fable.Core

[<StringEnum>]
type EditorLanguage =
  | [<CompiledName("plaintext")>] PlainText
  | [<CompiledName("javascript")>] JavaScript
  | [<CompiledName("typescript")>] TypeScript
  member x.displayText =
    match x with
    | PlainText -> "Plain text"
    | JavaScript -> "JavaScript"
    | TypeScript -> "TypeScript"
  static member all = [PlainText; JavaScript; TypeScript]

type Height = | Height of int

type EditorOptions = { WrapText: bool }
  with 
    static member initial = { WrapText = false }

type DragModel = { DragenterCount: int; DragleaveCount: int }
  with 
    static member initial = { DragenterCount = 0; DragleaveCount = 0 }
    member x.isDragging = x.DragenterCount > x.DragleaveCount

type TabItemModel = { ModelIndex: int; Name: string; Language: EditorLanguage; IsUntitled: bool }
  with
    static member initial = { ModelIndex = 0; Name = "Untitled 1"; Language = PlainText; IsUntitled = true }

[<RequireQualifiedAccess>]
type ControlId =
  | None
  | WrapText
  | EditorLanguage

type CssClasses = { RootDiv: string }

// Model holds the current state.
type Model = 
  { SelectedTabId: int
    TabItems: TabItemModel list
    EditorHeight: int
    EditorOptions: EditorOptions
    EditorLanguage: EditorLanguage
    ShowTooltipControlId: ControlId
    WindowInnerWidth: float
    DevicePixelRatio: float
    Debouncer: Debouncer.State
    DragModel: DragModel }