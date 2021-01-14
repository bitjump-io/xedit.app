module Msg

open Browser.Types
open Thoth.Elmish
open Model

// Msg is an action that we need to apply to the current state.
type Msg =
  | EditorCreated of Height
  | ToggleWrapText
  | OpenFilePicker
  | FilesAdded of File list
  | WindowWidthChaned of float // Triggered each time a window.resize event is emitted.
  | DebouncerSelfMsg of Debouncer.SelfMessage<Msg> // This is the message used by the Debouncer.
  | EndOfWindowWidthChaned // Message we want to debounce.
  | TabChanged of int
  | AddEmptyTab
  | AddTab of TabItemModel
  | RemoveTab of int
  | EditorLanguageChanged of EditorLanguage
  | ShowTooltipChanged of ControlId
  | OnDragenter
  | OnDragleave
  | OnDrop of File list
  | OnPromiseError of exn