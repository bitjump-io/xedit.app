// Currently the style property is not implemented on the fsharp binding of HTMLElement.
// see "type [<AllowNullLiteral>] HTMLElement" on https://github.com/fable-compiler/fable-browser/blob/master/src/Dom/Browser.Dom.fs
module rec DomEx

open Browser.Types

open Fable.Core
open Fable.Core.JsInterop
open Browser

// See CSSStyleDeclaration in https://github.com/microsoft/TypeScript/blob/master/lib/lib.dom.d.ts
type ICSSStyleDeclaration =
  abstract member backgroundColor: string with get, set

type IDOMTokenList =
  abstract member length: int with get
  abstract member value: string with get, set
  abstract add: tokens: string[] -> unit
  abstract member contains: token: string -> bool
  abstract member item: index: int -> string // may return null
  abstract member remove: tokens: string[] -> unit
  //replace(oldToken: string, newToken: string): void;
  //toggle(token: string, force?: boolean): boolean;
  //[index: number]: string;

type IHTMLElement =
  inherit HTMLElement
  abstract member style: ICSSStyleDeclaration
  abstract member classList: IDOMTokenList with get
  abstract member className: string with get, set

type INavigator =
  abstract member userAgent: string with get
  abstract member maxTouchPoints: int with get

[<Emit("typeof navigator === 'object' ? navigator : null")>]
let private navigatorExpr: INavigator option = jsNative

let navigatorObj = navigatorExpr