module Effects.Rainbow

open System
open System.Text
open ColorConversion
open Memoizer
open ArrayTools

let frames_rainbow (nb_steps) =
    let s = 1.0
    let v = 1.0
    let step = 1. / float nb_steps

    [
        for hue_start in 0.0 .. step .. 1.0 - step do
            (hue_start, s, v) |> toRGB
    ]

let _frames_m_rainbow nb_steps = 
    frames_rainbow nb_steps |> List.map toHex

let _frames_lr_rainbow (nb_steps) =
    let colors_rgb_hex = frames_rainbow (nb_steps)

    let frames_hex = [
        for i in 0 .. nb_steps - 1 do
            shift_and_sample (colors_rgb_hex, i, NB_LEDS_STICK) |> Seq.map toHex |> String.concat ""
    ]

    frames_hex

let frames_lr_rainbow = memoize 2 (_frames_lr_rainbow)
let frames_m_rainbow = memoize 2 (_frames_m_rainbow)
