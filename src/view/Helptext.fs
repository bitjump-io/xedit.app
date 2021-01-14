module Helptext

open Feliz

let helptextElement =
  Html.div [
    prop.style [style.marginTop 5]
    prop.children [
      Html.text "Tipps"
      Html.ul [
        // Better remap default keybindings to CodeMirror keybindings.
        // https://github.com/microsoft/monaco-editor/issues/102
        // https://github.com/microsoft/monaco-editor/issues/1350
        Html.li "To see the context menu, right-MouseClick (Crl-MouseClick on Mac)."
        Html.li "Hold down Alt (option on Mac) and navigate with the left and right arrows to move word-by-word. Additionally hold down Shift to select while moving."
        Html.li "Hold down Ctrl (command on Mac) and navigate with the left and right arrows to move to the start and end of the line. Additionally hold down Shift to select while moving."
        Html.li "Hold down Alt (option on Mac) and navigate with the up and down arrows to move the current row up or down."
        Html.li "Add additional cursors by holding down Alt (option on Mac) then MouseClick where you want the cursors then release Alt."
        Html.li "To add multiple cursors in the same column at once, MouseClick in the row and column where the firt cursor should be then holding down Shift + Alt (Shift + option on Mac) and MouseClick the last row of the same column."
        Html.li "To select text in column mode, MouseClick at the upper left start of the text that you want to select, then hold down Shift + Alt (Shift + option on Mac) and MouseClick or MousePress-and-Drag to select the bottom right of the selection."
        Html.li "You can drag selected text with the mouse."
        Html.li "Ctrl-a selects everything."
        Html.li "Ctrl-c is copy."
        Html.li "Ctrl-v is paste."
        Html.li "Ctrl-x is cut."
        Html.li "Ctrl-z is undo."
        Html.li "Ctrl-Shift-z is redo."
        Html.li "Ctrl-MouseClick to open links."
        Html.li "Hold down Shift and move the cursor to select text. Esc to undo the selection."
        Html.li "Press F1 to open the command palette that shows all available commands and keyboard shortcuts."
      ]
    ]
  ]