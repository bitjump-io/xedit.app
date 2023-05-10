module Keybindings

open HtmlEx
open Feliz
open Feliz.MaterialUI
open MuiTypes
open Model
open Msg
open Icons

// todo keybinding to transform to uppercase, lowercase, titlecase.

// https://material-ui.com/components/tables/#basic-table
let keybindingsTable (model: Model) (dispatch: Msg -> unit) (classes: CssClasses) =
  let cmdKeyElem = if model.ShowKeyBindingsFor = OS.Mac then Kbd.cmdMac else Kbd.ctrl
  let cmdShiftElem = if model.ShowKeyBindingsFor = OS.Mac then Kbd.cmdMacShift else Kbd.ctrlShift
  let altKeyElem = if model.ShowKeyBindingsFor = OS.Mac then Kbd.optMac else Kbd.alt
  let altWithMouseLeftKeyElem = if model.ShowKeyBindingsFor = OS.Mac then Kbd.optMacWithIcon(mouseLeftClickIcon()) else Kbd.altWithIcon(mouseLeftClickIcon())
  let cmdWithMouseLeftElem = if model.ShowKeyBindingsFor = OS.Mac then Kbd.cmdMacWithIcon(mouseLeftClickIcon()) else Kbd.ctrlWithIcon(mouseLeftClickIcon())
  let altWithMouseScrollElem = if model.ShowKeyBindingsFor = OS.Mac then Kbd.optMacWithIcon(mouseScrollIcon()) else Kbd.altWithIcon(mouseScrollIcon()) 
  let shiftAltElem = if model.ShowKeyBindingsFor = OS.Mac then Kbd.shiftOptMac else Kbd.shiftAlt
  Mui.tableContainer [
    tableContainer.component' MuiTypes.paper
    tableContainer.children [
      Mui.table [
        table.size.small
        table.children [
          Mui.tableHead [
            Mui.tableRow [
              Mui.tableCell "Command"
              Mui.tableCell [
                prop.children [
                  Html.text "Keybinding "
                  Mui.formControl [
                    formControl.size.small
                    formControl.variant.outlined
                    formControl.children [
                      Mui.select [
                        prop.classes [classes.ButtonHover]
                        select.input (
                          Mui.inputBase []
                        )
                        select.value (unbox<string> model.ShowKeyBindingsFor)
                        select.onChange (ShowKeyBindingsForChanged >> dispatch)
                        select.children [
                          for el in OS.all -> 
                            Mui.menuItem [
                              prop.value (unbox<string> el)
                              menuItem.children [
                                Html.text (unbox<string> el)
                              ]
                            ]
                        ]
                      ]
                    ]
                  ]
                ]
              ]
              Mui.tableCell ""
            ]
          ]
          Mui.tableBody [
            Mui.tableRow [
              if model.ShowKeyBindingsFor = OS.Windows then
                Mui.tableCell (wrapper() ++ Kbd.ctrl(Key.LeftArrow) ++ " / " ++ Kbd.ctrl(Key.RightArrow) ++ ())
              elif model.ShowKeyBindingsFor = OS.Mac then
                Mui.tableCell (wrapper() ++ Kbd.optMac(Key.LeftArrow) ++ " / " ++ Kbd.optMac(Key.RightArrow) ++ ())
              else
                Mui.tableCell ""
              Mui.tableCell (wrapper() ++ "Move one word to the left/right. Hold down " ++ Kbd.shift() ++ " to select." ++ ())
              Mui.tableCell ""
            ]
            Mui.tableRow [
              if model.ShowKeyBindingsFor = OS.Windows then
                Mui.tableCell (wrapper() ++ Kbd.singleKey(Key.Home) ++ " / " ++ Kbd.singleKey(Key.End) ++ ())
              elif model.ShowKeyBindingsFor = OS.Mac then
                Mui.tableCell (wrapper() ++ Kbd.cmdMac(Key.LeftArrow) ++ " / " ++ Kbd.cmdMac(Key.RightArrow) ++ ())
              else
                Mui.tableCell ""
              Mui.tableCell (wrapper() ++ "Move to the beginning/end of the line. Hold down " ++ Kbd.shift() ++ " to select." ++ ())
              Mui.tableCell ""
            ]
            Mui.tableRow [
              if model.ShowKeyBindingsFor = OS.Windows then
                Mui.tableCell (wrapper() ++ Kbd.ctrl(Key.Home) ++ " / " ++ Kbd.ctrl(Key.End) ++ ())
              elif model.ShowKeyBindingsFor = OS.Mac then
                Mui.tableCell (wrapper() ++ Kbd.cmdMac(Key.UpArrow) ++ " / " ++ Kbd.cmdMac(Key.DownArrow) ++ ())
              else
                Mui.tableCell ""
              Mui.tableCell (wrapper() ++ "Move to the first/last position in the file. Hold down " ++ Kbd.shift() ++ " to select." ++ ())
              Mui.tableCell ""
            ]
            // Mui.tableRow [
            //   Mui.tableCell (wrapper() ++ Kbd.ctrl(Key.LeftArrow) ++ "/" ++ Kbd.ctrl(Key.RightArrow) ++ ())
            //   Mui.tableCell (wrapper() ++ "Move to the first/last column. Hold down " ++ Kbd.shift() ++ " to select." ++ ())
            //   Mui.tableCell ""
            // ]
            // Mui.tableRow [
            //   Mui.tableCell (wrapper() ++ Kbd.ctrl(Key.UpArrow) ++ "/" ++ Kbd.ctrl(Key.DownArrow) ++ ())
            //   Mui.tableCell (wrapper() ++ "Move to the first/last row. Hold down " ++ Kbd.shift() ++ " to select." ++ ())
            //   Mui.tableCell ""
            // ]
            Mui.tableRow [
              Mui.tableCell [cmdKeyElem(Key.A)]
              Mui.tableCell "Select everything."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              Mui.tableCell [cmdKeyElem(Key.C)]
              Mui.tableCell "Copy selected text, or the line, if no text is selected, to the clipboard."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              Mui.tableCell [cmdKeyElem(Key.V)]
              Mui.tableCell "Insert the clipboard content at the cursor(s) location(s)."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              Mui.tableCell [cmdKeyElem(Key.X)]
              Mui.tableCell "Copy and delete the selected text, or the line, if no text is selected, to the clipboard."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              Mui.tableCell [cmdKeyElem(Key.K)]
              Mui.tableCell "Delete line."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              Mui.tableCell [cmdKeyElem(Key.Z)]
              Mui.tableCell "Undo last change."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              Mui.tableCell [cmdShiftElem(Key.Z)]
              Mui.tableCell "Redo last change."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              Mui.tableCell (wrapper() ++ altKeyElem(Key.UpArrow) ++ " / " ++ altKeyElem(Key.DownArrow) ++ ())
              Mui.tableCell "Move the current line up/down."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              Mui.tableCell [cmdKeyElem(Key.F)]
              Mui.tableCell "Open the find window with the selected text pre-filled."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              if model.ShowKeyBindingsFor = OS.Windows then
                Mui.tableCell [Kbd.ctrl(Key.H)]
              elif model.ShowKeyBindingsFor = OS.Mac then
                Mui.tableCell [Kbd.cmdMacOpt(Key.F)]
              else
                Mui.tableCell ""
              Mui.tableCell "Open the replace window with the selected text pre-filled."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              Mui.tableCell (wrapper() ++ cmdWithMouseLeftElem ++ ())
              Mui.tableCell "Add an additional cursor."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              Mui.tableCell (wrapper() ++ altWithMouseLeftKeyElem ++ ())
              Mui.tableCell "Column mode (box) selection with additional cursors."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              Mui.tableCell (wrapper() ++ altWithMouseScrollElem ++ ())
              Mui.tableCell "5 times faster scrolling."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              Mui.tableCell (wrapper() ++ shiftAltElem(Key.F) ++ ())
              Mui.tableCell "Format document."
              Mui.tableCell ""
            ]
            Mui.tableRow [
              Mui.tableCell (wrapper() ++ cmdKeyElem(Key.K) ++ " " ++ cmdKeyElem(Key.F) ++ ())
              Mui.tableCell "Format selection."
              Mui.tableCell ""
            ]
          ]
        ]
      ]
    ]
  ]

  // ctrl (or command) -g go to line
  // move selected text via drag-drop. -> not working

// let helptextElement =
//   Html.div [
//     prop.style [style.marginTop 5]
//     prop.children [
//       Html.text "Tipps"
//       Html.ul [
//         // Better remap default keybindings to CodeMirror keybindings.
//         // https://github.com/microsoft/monaco-editor/issues/102
//         // https://github.com/microsoft/monaco-editor/issues/1350
//         Html.li (wrapper() ++ "To see the context menu, right-MouseClick (" ++ Kbd.ctrlMac() ++ "-MouseClick on Mac)." ++ ())
//         Html.li "Add additional cursors by holding down Alt (option on Mac) then MouseClick where you want the cursors then release Alt."
//         Html.li "To add multiple cursors in the same column at once, MouseClick in the row and column where the firt cursor should be then holding down Shift + Alt (Shift + option on Mac) and MouseClick the last row of the same column."
//         Html.li "To select text in column mode, MouseClick at the upper left start of the text that you want to select, then hold down Shift + Alt (Shift + option on Mac) and MouseClick or MousePress-and-Drag to select the bottom right of the selection."
//         Html.li "You can drag selected text with the mouse."
//         Html.li (wrapper() ++ (Kbd.ctrlMac (Key.A)) ++ " selects everything." ++ ())
//         Html.li "Ctrl-Shift-z is redo."
//         Html.li "Ctrl-MouseClick to open links."
//         Html.li "Hold down Shift and move the cursor to select text. Esc to undo the selection."
//         Html.li "Press F1 to open the command palette that shows all available commands and keyboard shortcuts."
//       ]
//     ]
//   ]