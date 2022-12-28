// Partly copied from https://github.com/Shmew/Feliz.MaterialUI/blob/master/src/Feliz.MaterialUI/Props.fs
namespace MUI

open System
open System.ComponentModel
open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Feliz

[<Erase>]
type iconButton =
  /// The icon element.
  static member inline children (element: ReactElement) = prop.children element
  /// The icon element.
  static member inline children (elements: ReactElement seq) = prop.children elements
  /// The icon element.
  static member inline children (value: string) = Interop.mkAttr "children" value
  /// The icon element.
  static member inline children (values: string seq) = Interop.mkAttr "children" values
  /// The icon element.
  static member inline children (value: int) = Interop.mkAttr "children" value
  /// The icon element.
  static member inline children (value: float) = Interop.mkAttr "children" value
  /// If `true`, the button will be disabled.
  static member inline disabled (value: bool) = Interop.mkAttr "disabled" value
  /// If `true`, the keyboard focus ripple will be disabled.
  static member inline disableFocusRipple (value: bool) = Interop.mkAttr "disableFocusRipple" value
  /// If `true`, the ripple effect will be disabled.
  static member inline disableRipple (value: bool) = Interop.mkAttr "disableRipple" value
  /// *Inherited from `buttonBase`*
  ///
  /// A ref for imperative actions. It currently only supports `focusVisible()` action.
  static member inline action (ref: IRefValue<ButtonBaseActions option>) = Interop.mkAttr "action" ref
  /// *Inherited from `buttonBase`*
  ///
  /// A ref for imperative actions. It currently only supports `focusVisible()` action.
  static member inline action (handler: ButtonBaseActions -> unit) = Interop.mkAttr "action" handler
  /// *Inherited from `buttonBase`*
  ///
  /// If `true`, the ripples will be centered. They won't start at the cursor interaction position.
  static member inline centerRipple (value: bool) = Interop.mkAttr "centerRipple" value
  /// *Inherited from `buttonBase`*
  ///
  /// The component used for the root node. Either a string to use a HTML element or a component.
  ///
  /// ⚠️ [Needs to be able to hold a ref](https://material-ui.com/guides/composition/#caveat-with-refs).
  static member inline component' (value: string) = Interop.mkAttr "component" value
  /// *Inherited from `buttonBase`*
  ///
  /// The component used for the root node. Either a string to use a HTML element or a component.
  ///
  /// ⚠️ [Needs to be able to hold a ref](https://material-ui.com/guides/composition/#caveat-with-refs).
  static member inline component' (value: ReactElementType) = Interop.mkAttr "component" value
  /// *Inherited from `buttonBase`*
  ///
  /// If `true`, the touch ripple effect will be disabled.
  static member inline disableTouchRipple (value: bool) = Interop.mkAttr "disableTouchRipple" value
  /// *Inherited from `buttonBase`*
  ///
  /// If `true`, the base button will have a keyboard focus ripple.
  static member inline focusRipple (value: bool) = Interop.mkAttr "focusRipple" value
  /// *Inherited from `buttonBase`*
  ///
  /// This prop can help a person know which element has the keyboard focus. The class name will be applied when the element gain the focus through a keyboard interaction. It's a polyfill for the [CSS :focus-visible selector](https://drafts.csswg.org/selectors-4/#the-focus-visible-pseudo). The rationale for using this feature [is explained here](https://github.com/WICG/focus-visible/blob/master/explainer.md). A [polyfill can be used](https://github.com/WICG/focus-visible) to apply a `focus-visible` class to other components if needed.
  static member inline focusVisibleClassName (value: string) = Interop.mkAttr "focusVisibleClassName" value
  /// *Inherited from `buttonBase`*
  ///
  /// Callback fired when the component is focused with a keyboard. We trigger a `onFocus` callback too.
  static member inline onFocusVisible (handler: Event -> unit) = Interop.mkAttr "onFocusVisible" handler
  /// *Inherited from `buttonBase`*
  ///
  /// Props applied to the `TouchRipple` element.
  static member inline TouchRippleProps (props: IReactProperty list) = Interop.mkAttr "TouchRippleProps" (createObj !!props)

module iconButton =

  /// The color of the component.
  [<Erase>]
  type color =
    static member inline default' = Interop.mkAttr "color" "default"
    static member inline inherit' = Interop.mkAttr "color" "inherit"
    static member inline primary = Interop.mkAttr "color" "primary"
    static member inline secondary = Interop.mkAttr "color" "secondary"

  /// If given, uses a negative margin to counteract the padding on one side (this is often helpful for aligning the left or right side of the icon with content above or below, without ruining the border size and shape).
  [<Erase>]
  type edge =
    static member inline start = Interop.mkAttr "edge" "start"
    static member inline end' = Interop.mkAttr "edge" "end"
    static member inline false' = Interop.mkAttr "edge" false

  /// The size of the button. `small` is equivalent to the dense button styling.
  [<Erase>]
  type size =
    static member inline small = Interop.mkAttr "size" "small"
    static member inline medium = Interop.mkAttr "size" "medium"

[<Erase>]
type tooltip =
  /// If `true`, adds an arrow to the tooltip.
  static member inline arrow (value: bool) = Interop.mkAttr "arrow" value
  /// Tooltip reference element.
  ///
  /// ⚠️ [Needs to be able to hold a ref](https://material-ui.com/guides/composition/#caveat-with-refs).
  static member inline children (value: ReactElement) = Interop.mkAttr "children" value
  /// Do not respond to focus events.
  static member inline disableFocusListener (value: bool) = Interop.mkAttr "disableFocusListener" value
  /// Do not respond to hover events.
  static member inline disableHoverListener (value: bool) = Interop.mkAttr "disableHoverListener" value
  /// Do not respond to long press touch events.
  static member inline disableTouchListener (value: bool) = Interop.mkAttr "disableTouchListener" value
  /// The number of milliseconds to wait before showing the tooltip. This prop won't impact the enter touch delay (`enterTouchDelay`).
  static member inline enterDelay (value: int) = Interop.mkAttr "enterDelay" value
  /// The number of milliseconds to wait before showing the tooltip when one was already recently opened.
  static member inline enterNextDelay (value: int) = Interop.mkAttr "enterNextDelay" value
  /// The number of milliseconds a user must touch the element before showing the tooltip.
  static member inline enterTouchDelay (value: int) = Interop.mkAttr "enterTouchDelay" value
  /// This prop is used to help implement the accessibility logic. If you don't provide this prop. It falls back to a randomly generated id.
  static member inline id (value: string) = Interop.mkAttr "id" value
  /// Makes a tooltip interactive, i.e. will not close when the user hovers over the tooltip before the `leaveDelay` is expired.
  static member inline interactive (value: bool) = Interop.mkAttr "interactive" value
  /// The number of milliseconds to wait before hiding the tooltip. This prop won't impact the leave touch delay (`leaveTouchDelay`).
  static member inline leaveDelay (value: int) = Interop.mkAttr "leaveDelay" value
  /// The number of milliseconds after the user stops touching an element before hiding the tooltip.
  static member inline leaveTouchDelay (value: int) = Interop.mkAttr "leaveTouchDelay" value
  /// Callback fired when the component requests to be closed.
  ///
  /// **Signature:**
  ///
  /// `function(event: object) => void`
  ///
  /// *event:* The event source of the callback.
  static member inline onClose (handler: Event -> unit) = Interop.mkAttr "onClose" handler
  /// Callback fired when the component requests to be open.
  ///
  /// **Signature:**
  ///
  /// `function(event: object) => void`
  ///
  /// *event:* The event source of the callback.
  static member inline onOpen (handler: Event -> unit) = Interop.mkAttr "onOpen" handler
  /// If `true`, the tooltip is shown.
  static member inline open' (value: bool) = Interop.mkAttr "open" value
  /// The component used for the popper.
  static member inline PopperComponent (value: ReactElementType) = Interop.mkAttr "PopperComponent" value
  /// Props applied to the [`Popper`](https://material-ui.com/api/popper/) element.
  static member inline PopperProps (props: IReactProperty list) = Interop.mkAttr "PopperProps" (createObj !!props)
  /// Tooltip title. Zero-length titles string are never displayed.
  static member inline title (value: ReactElement) = Interop.mkAttr "title" value
  /// Tooltip title. Zero-length titles string are never displayed.
  static member inline title (values: ReactElement seq) = Interop.mkAttr "title" values
  /// Tooltip title. Zero-length titles string are never displayed.
  static member inline title (value: string) = Interop.mkAttr "title" value
  /// Tooltip title. Zero-length titles string are never displayed.
  static member inline title (values: string seq) = Interop.mkAttr "title" values
  /// Tooltip title. Zero-length titles string are never displayed.
  static member inline title (value: int) = Interop.mkAttr "title" value
  /// Tooltip title. Zero-length titles string are never displayed.
  static member inline title (value: float) = Interop.mkAttr "title" value
  /// The component used for the transition. [Follow this guide](https://material-ui.com/components/transitions/#transitioncomponent-prop) to learn more about the requirements for this component.
  static member inline TransitionComponent (value: ReactElementType) = Interop.mkAttr "TransitionComponent" value
  /// Props applied to the [`Transition`](http://reactcommunity.org/react-transition-group/transition#Transition-props) element.
  static member inline TransitionProps (props: IReactProperty list) = Interop.mkAttr "TransitionProps" (createObj !!props)

module tooltip =

  /// Tooltip placement.
  [<Erase>]
  type placement =
    static member inline bottomEnd = Interop.mkAttr "placement" "bottom-end"
    static member inline bottomStart = Interop.mkAttr "placement" "bottom-start"
    static member inline bottom = Interop.mkAttr "placement" "bottom"
    static member inline leftEnd = Interop.mkAttr "placement" "left-end"
    static member inline leftStart = Interop.mkAttr "placement" "left-start"
    static member inline left = Interop.mkAttr "placement" "left"
    static member inline rightEnd = Interop.mkAttr "placement" "right-end"
    static member inline rightStart = Interop.mkAttr "placement" "right-start"
    static member inline right = Interop.mkAttr "placement" "right"
    static member inline topEnd = Interop.mkAttr "placement" "top-end"
    static member inline topStart = Interop.mkAttr "placement" "top-start"
    static member inline top = Interop.mkAttr "placement" "top"

[<Erase>]
type button =
  /// The content of the button.
  static member inline children (element: ReactElement) = prop.children element
  /// The content of the button.
  static member inline children (elements: ReactElement seq) = prop.children elements
  /// The content of the button.
  static member inline children (value: string) = Interop.mkAttr "children" value
  /// The content of the button.
  static member inline children (values: string seq) = Interop.mkAttr "children" values
  /// The content of the button.
  static member inline children (value: int) = Interop.mkAttr "children" value
  /// The content of the button.
  static member inline children (value: float) = Interop.mkAttr "children" value
  /// The component used for the root node. Either a string to use a HTML element or a component.
  static member inline component' (value: string) = Interop.mkAttr "component" value
  /// The component used for the root node. Either a string to use a HTML element or a component.
  static member inline component' (value: ReactElementType) = Interop.mkAttr "component" value
  /// If `true`, the button will be disabled.
  static member inline disabled (value: bool) = Interop.mkAttr "disabled" value
  /// If `true`, no elevation is used.
  static member inline disableElevation (value: bool) = Interop.mkAttr "disableElevation" value
  /// If `true`, the keyboard focus ripple will be disabled.
  static member inline disableFocusRipple (value: bool) = Interop.mkAttr "disableFocusRipple" value
  /// If `true`, the ripple effect will be disabled.
  ///
  /// ⚠️ Without a ripple there is no styling for :focus-visible by default. Be sure to highlight the element by applying separate styles with the `focusVisibleClassName`.
  static member inline disableRipple (value: bool) = Interop.mkAttr "disableRipple" value
  /// Element placed after the children.
  static member inline endIcon (element: ReactElement) = Interop.mkAttr "endIcon" element
  /// If `true`, the button will take up the full width of its container.
  static member inline fullWidth (value: bool) = Interop.mkAttr "fullWidth" value
  /// The URL to link to when the button is clicked. If defined, an `a` element will be used as the root node.
  static member inline href (value: string) = Interop.mkAttr "href" value
  /// Element placed before the children.
  static member inline startIcon (element: ReactElement) = Interop.mkAttr "startIcon" element
  /// *Inherited from `buttonBase`*
  ///
  /// A ref for imperative actions. It currently only supports `focusVisible()` action.
  static member inline action (ref: IRefValue<ButtonBaseActions option>) = Interop.mkAttr "action" ref
  /// *Inherited from `buttonBase`*
  ///
  /// A ref for imperative actions. It currently only supports `focusVisible()` action.
  static member inline action (handler: ButtonBaseActions -> unit) = Interop.mkAttr "action" handler
  /// *Inherited from `buttonBase`*
  ///
  /// If `true`, the ripples will be centered. They won't start at the cursor interaction position.
  static member inline centerRipple (value: bool) = Interop.mkAttr "centerRipple" value
  /// *Inherited from `buttonBase`*
  ///
  /// If `true`, the touch ripple effect will be disabled.
  static member inline disableTouchRipple (value: bool) = Interop.mkAttr "disableTouchRipple" value
  /// *Inherited from `buttonBase`*
  ///
  /// If `true`, the base button will have a keyboard focus ripple.
  static member inline focusRipple (value: bool) = Interop.mkAttr "focusRipple" value
  /// *Inherited from `buttonBase`*
  ///
  /// This prop can help a person know which element has the keyboard focus. The class name will be applied when the element gain the focus through a keyboard interaction. It's a polyfill for the [CSS :focus-visible selector](https://drafts.csswg.org/selectors-4/#the-focus-visible-pseudo). The rationale for using this feature [is explained here](https://github.com/WICG/focus-visible/blob/master/explainer.md). A [polyfill can be used](https://github.com/WICG/focus-visible) to apply a `focus-visible` class to other components if needed.
  static member inline focusVisibleClassName (value: string) = Interop.mkAttr "focusVisibleClassName" value
  /// *Inherited from `buttonBase`*
  ///
  /// Callback fired when the component is focused with a keyboard. We trigger a `onFocus` callback too.
  static member inline onFocusVisible (handler: Event -> unit) = Interop.mkAttr "onFocusVisible" handler
  /// *Inherited from `buttonBase`*
  ///
  /// Props applied to the `TouchRipple` element.
  static member inline TouchRippleProps (props: IReactProperty list) = Interop.mkAttr "TouchRippleProps" (createObj !!props)

module button =
  /// The color of the component.
  [<Erase>]
  type color =
    static member inline default' = Interop.mkAttr "color" "default"
    static member inline inherit' = Interop.mkAttr "color" "inherit"
    static member inline primary = Interop.mkAttr "color" "primary"
    static member inline secondary = Interop.mkAttr "color" "secondary"
  /// The size of the button. `small` is equivalent to the dense button styling.
  [<Erase>]
  type size =
    static member inline large = Interop.mkAttr "size" "large"
    static member inline medium = Interop.mkAttr "size" "medium"
    static member inline small = Interop.mkAttr "size" "small"
  /// The variant to use.
  [<Erase>]
  type variant =
    static member inline contained = Interop.mkAttr "variant" "contained"
    static member inline outlined = Interop.mkAttr "variant" "outlined"
    static member inline text = Interop.mkAttr "variant" "text"