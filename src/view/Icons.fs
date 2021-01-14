module Icons

open Fable.Core
open Fable.Core.JsInterop
open Fable.React

let inline gitHubIcon b : ReactElement = 
  ofImport "default" "@material-ui/icons/GitHub" (keyValueList CaseRules.LowerFirst b) []

let inline wrapTextIcon b : ReactElement = 
  ofImport "default" "@material-ui/icons/WrapText" (keyValueList CaseRules.LowerFirst b) []

let inline AddIcon b : ReactElement = 
  ofImport "default" "@material-ui/icons/Add" (keyValueList CaseRules.LowerFirst b) []