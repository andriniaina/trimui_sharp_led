module Effects.Rainbow

open System
open System.IO
open ColorConversion
open Memoizer
open ArrayTools

let frames_rainbow (nb_steps) =
    let s = 1.0
    let v = 1.0
    let step = 1. / float nb_steps

    [
        for hue_start in 0.0 .. step .. 1.0 - step do
            (hue_start, s, v) |> toRGB |> toHex
    ]

let frames_m_rainbow (nb_steps) =
#if WRITE_BINARY
    [ frames_rainbow (nb_steps) ]
#else
    frames_rainbow (nb_steps)
#endif

let frames_lr_rainbow (nb_steps) =
    let colors_rgb_hex = frames_rainbow (nb_steps)

    let frames_hex =
        [
            for i in 0 .. nb_steps - 1 do
                shift_and_sample (colors_rgb_hex, i, NB_LEDS_STICK)
#if WRITE_BINARY
#else
                |> String.concat ""
#endif

        ]

    frames_hex
