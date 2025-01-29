module Effects.Nexus

open System
open System.IO
open ColorConversion
open Memoizer
open ArrayTools
open System.Text

let _frames_lr_nexus (nb_steps, ttl_hash: float) =
    let randomColor () = toHex (toRGB (Random.Shared.NextDouble(), 1, 1))
    let randomIndex (_) = Random.Shared.Next(NB_LEDS_STICK - 1)
    let i, j = 0, randomIndex ()
    let k = Seq.initInfinite (randomIndex) |> Seq.find (fun x -> x <> j)
    let leds_index_on = dict [ i, COLOR_RED_HEX; j, randomColor (); k, randomColor () ]
    let halfway = nb_steps / 2

    let frame_initial = [
        for i in 0 .. NB_LEDS_STICK - 1 do
            if leds_index_on.ContainsKey(i) then leds_index_on.[i] else COLOR_BLACK_HEX
    ]

    let frames = [
        for i in 0 .. nb_steps - 1 do
            let frame = if i < halfway then shift (i / 4, frame_initial) else shift ((nb_steps - i) / 4, frame_initial) // FIXME
            String.Join("", frame)
    ]

    frames

let frames_lr_nexus_memoized = memoize 2 (_frames_lr_nexus)

let frames_lr_nexus (nb_steps) = frames_lr_nexus_memoized (nb_steps, get_ttl_hash (10))
