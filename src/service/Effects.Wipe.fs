module Effects.Wipe

open System
open System.IO
open ColorConversion
open Memoizer
open ArrayTools


let private _frames_lr_wipe (nb_steps, ttl_hash: float) =
    let s = 1
    let v = 1
    let hue1 = Random.Shared.NextDouble()
    let hue2 = (hue1 + 0.5) % 1.0

    let rgb1 = toRGB (hue1, s, v)
    let rgb2 = toRGB (hue2, s, v)

    let halfway = nb_steps / 2

    let frame = [
        for i in 0 .. nb_steps - 1 do
            let r = if i < halfway then NB_LEDS_STICK * i / halfway else NB_LEDS_STICK - NB_LEDS_STICK * (i - halfway) / halfway

            let frame = [
                COLOR_WHITE

                for j in 1 .. NB_LEDS_STICK - 1 do
                    if j < r then rgb1 else rgb2
            ]

            let frame = shift (-2, frame) // because my LEDs are shifted.... Trimui QA...
            String.Join("", frame |> List.map toHex)
    ]

    frame

let private frames_lr_wipe_memoized = memoize 2 _frames_lr_wipe

let frames_lr_wipe (nb_steps) = frames_lr_wipe_memoized (nb_steps, get_ttl_hash (30)) // refresh every 30s
