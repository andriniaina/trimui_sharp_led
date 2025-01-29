module Effects

open System
open System.Collections.Generic
open System.IO
open ColorConversion
open System.Text

let NB_LEDS_STICK = 11


let sample(arr: list<'t>, start:int, n: int) =
    let start_f = float start
    let size = float arr.Length
    let step_f = size / float n
    let r = [ for i = 0.0 to n do arr[int((start_f + (i * step_f)) % size)]]
    r

let frames_rainbow(nb_steps) =
    let s = 1.0
    let v = 1.0
    let step = 1./float nb_steps
    let hues_start = [for i in 0.0..step..1.0 do yield i ]
    let colors_rgb = [ for h in hues_start do toRGB(h, s, v)]
    let colors_rgb_hex = [for r, g, b in colors_rgb do toHex(r, g, b) ]
    colors_rgb_hex

let frames_m_rainbow(nb_steps) =
#if WRITE_BINARY
    [frames_rainbow(nb_steps)]
#else
    frames_rainbow(nb_steps)
#endif

let frames_lr_rainbow(nb_steps) =
    let colors_rgb_hex = frames_rainbow(nb_steps)
    let frames_hex = [
        for i in 0..nb_steps do
        (sample (colors_rgb_hex, i, NB_LEDS_STICK)
        #if WRITE_BINARY
        #else
         |> String.concat ""
         #endif
        )
    ]
    frames_hex

