// Currently the style property is not implemented on the fsharp binding of HTMLElement.
// see "type [<AllowNullLiteral>] HTMLElement" on https://github.com/fable-compiler/fable-browser/blob/master/src/Dom/Browser.Dom.fs
module rec DomEx

open Browser.Types
open Fable.Core
open Fable.Core.JsInterop
open Browser

type IWindow =
  abstract member performance: IPerformance

type IPerformance =
  abstract getEntries: unit -> IPerformanceEntry[]
  abstract getEntriesByName: name: string -> IPerformanceEntry[] // second optional arg is type
  abstract getEntriesByType: type': string -> IPerformanceEntry[]
  abstract mark: markName: string -> unit
  abstract measure: measureName: string * startMark: string * endMark: string -> unit
  abstract now: unit -> float
  abstract timing: IPerformanceTiming

type IPerformanceEntry =
  abstract duration: float with get
  abstract entryType: string with get
  abstract name: string with get
  abstract startTime: float with get
  abstract toJSON: unit -> string // any

type IPerformanceTiming =
  abstract connectEnd: float with get
  abstract connectStart: float with get
  abstract domComplete:  float with get
  abstract domContentLoadedEventEnd: float with get
  abstract domContentLoadedEventStart: float with get
  abstract domInteractive: float with get
  abstract domLoading: float with get
  abstract domainLookupEnd: float with get
  abstract domainLookupStart: float with get
  abstract fetchStart: float with get
  abstract loadEventEnd: float with get
  abstract loadEventStart: float with get
  abstract navigationStart: float with get
  abstract redirectEnd: float with get
  abstract redirectStart: float with get
  abstract requestStart: float with get
  abstract responseEnd: float with get
  abstract responseStart: float with get
  abstract secureConnectionStart: float with get
  abstract unloadEventEnd: float with get
  abstract unloadEventStart: float with get
  abstract toJSON: unit -> string // any

// See CSSStyleDeclaration in https://github.com/microsoft/TypeScript/blob/master/lib/lib.dom.d.ts
type ICSSStyleDeclaration =
  abstract member backgroundColor: string with get, set
  abstract member width: string with get, set
  abstract member height: string with get, set
  abstract member opacity: string with get, set

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