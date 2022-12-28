// Partly copied from https://github.com/Shmew/Feliz.MaterialUI/blob/master/src/Feliz.MaterialUI/Bindings.fs

namespace MUI

open Fable.Core
open Browser.Types
open Feliz
open Feliz.Styles

type ButtonBaseActions =
  abstract focusVisible: unit -> bool