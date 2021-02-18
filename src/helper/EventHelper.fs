module EventHelper

open Browser.Dom

// see https://github.com/elmish/elmish/issues/189#issuecomment-530080466
// Example call: Slider.onChange (fun value -> throttle 1000 (fun _ -> dispatch (SetValue value)))
let throttle =
  let timeoutIds = ResizeArray<float>()
  fun (timeout: int) (callback: unit -> unit) ->
    let delayed = fun _ ->
      for timeoutId in timeoutIds do window.clearInterval(timeoutId)
      callback()
    timeoutIds.Add(window.setTimeout(delayed, timeout))

let setTimeout (handler: unit -> unit) (timeout: int) : unit =
  window.setTimeout(handler, timeout) |> ignore