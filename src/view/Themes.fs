module Themes
open Feliz
open Feliz.MaterialUI

// Themes documentation links
// - https://cmeeren.github.io/Feliz.MaterialUI/#usage/themes
// - https://material-ui.com/customization/theming/

let defaultTheme = Styles.createTheme()

let darkTheme = Styles.createTheme([
  theme.palette.mode.dark
  theme.palette.primary Colors.blue
  theme.palette.secondary Colors.teal
  theme.palette.background.default' defaultTheme.palette.grey.``900``
  theme.palette.background.paper "#1e1e1e"
  theme.typography.h1.fontSize "3rem"
  theme.typography.h2.fontSize "2rem"
  theme.typography.h3.fontSize "1.5rem"

  theme.styleOverrides.muiAppBar.colorDefault [
    style.backgroundColor "#303030"
  ]
  theme.styleOverrides.muiPopover.root [
    // Need to set color directly because body is the parent.
    style.color "#ccc"
  ]
  theme.styleOverrides.muiPaper.root [
    style.color "inherit"
    style.backgroundColor "#303030"
  ]
  theme.styleOverrides.muiDrawer.paper [
    style.backgroundColor defaultTheme.palette.grey.``900``
  ]
  theme.defaultProps.muiAppBar [
    appBar.color.default'
  ]
  theme.styleOverrides.muiButtonBase.root [
    style.color "inherit"
    //style.custom ("&:hover", [style.borderRadius 4; style.backgroundColor.red; style.borderColor "#ff0000"; style.boxShadow (0, 0, 0, 20, "rgba(0,123,255,.25)")] )
  ]
  theme.styleOverrides.muiButton.root [
    style.color "inherit"
    style.textTransform.none
    // Remove fontSize because it overwrites MuiInputBase-input fontSize.
    style.custom ("fontSize", "unset")
  ]
  // theme.styleOverrides.muiInputBase.root [
  //   style.color (color.rgb(255, 133, 0))
  // ]
  theme.styleOverrides.muiInputBase.input [
    style.borderRadius 4
    style.position.relative
    //style.backgroundColor.transparent
    style.color (color.rgb(133, 133, 133))
    style.borderStyle borderStyle.none
    style.fontSize 16
    style.padding (8, 24, 8, 10)
   ]
   // theme.styleOverrides.muiInputBase.inputMarginDense [
  //   // Clear 3px paddingTop.
  //   style.paddingTop length.auto
  // ]
  theme.styleOverrides.muiIconButton.root [
    style.color (color.rgb(133, 133, 133))
  ]
  theme.styleOverrides.muiTabs.root [
    //style.display.flex
  ]
  theme.styleOverrides.muiTab.root [
    // Let tab width expand to tab label length.
    style.maxWidth length.auto
    // No uppercase.
    style.textTransform.none
    // Make label area wider to show close button at right.
    style.padding (0, 0, 0, 5)
  ]
  // theme.styleOverrides.muiSelect.root [
  //   // No uppercase.
  //   style.textTransform.none
  // ]
  theme.styleOverrides.muiSelect.icon [
    // Instead of white show specific color.
    style.color (color.rgb(133, 133, 133))
  ]
  theme.styleOverrides.muiFormControl.root [
    // undo vertical-align top
    style.verticalAlign.inheritFromParent
  ]
])
