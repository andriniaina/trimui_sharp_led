module Effects

open System
open System.IO
open ColorConversion
open Memoizer

let NB_LEDS_STICK = 11
let COLOR_BLACK = toHex (0uy, 0uy, 0uy)
let COLOR_RED = toHex (255uy, 0uy, 0uy)

let shift (x, array: 'a list) =
    let size = array.Length
    let x = x % size

    [
        for i in 0 .. size - 1 do
            array[(i - x + size) % size]
    ]

let shift_and_sample (arr: list<'t>, start: int, n: int) =
    let n_f = float n
    let start_f = float start
    let size = float arr.Length
    let step_f = size / n_f

    [
        for i = 0.0 to n_f - 1.0 do
            arr[int ((start_f + (i * step_f)) % size)]
    ]

let private read_battery_capacity () =
    let CAPACITY_FILENAME = "/sys/class/power_supply/axp2202-battery/capacity"
    if File.Exists(CAPACITY_FILENAME) then File.ReadAllText(CAPACITY_FILENAME).Trim() |> int else 0 // fake

let get_ttl_hash (seconds) =
    let t = DateTime.UtcNow - new DateTime(1970, 1, 1)
    Math.Round(t.TotalSeconds / seconds)

let _frames_battery (nb_steps, ttl_hash: float) =
    let capacity = read_battery_capacity () |> double
    let h_GREEN = 130.0
    let h = h_GREEN * capacity / 100.0
    let hex = toHex (toRGB (h, 1, 1))
    List.init nb_steps (fun _ -> hex)

let private frames_battery_memoized = memoize 2 _frames_battery

let frames_m_battery (nb_steps) = frames_battery_memoized (nb_steps, get_ttl_hash (60. * 10.))

let _frames_lr_battery (nb_steps, ttl_hash: float) =
    let capacity = read_battery_capacity () |> double
    // -1 for a more dramatic effect
    let capacity_index = Math.Floor(capacity / 100.0 * double NB_LEDS_STICK) |> int
    let h_GREEN = 130.0
    let hex = toRGB (h_GREEN / 360.0, 1, 1) |> toHex

    let frame = [
        for _ in 0 .. capacity_index - 1 do
            hex

        for _ in 0 .. (NB_LEDS_STICK - capacity_index - 1) do
            "550000 "
    ]

    let frame_adjusted = String.Join("", shift (1, frame))
    List.init nb_steps (fun _ -> frame_adjusted)

let private frames_lr_battery_memoized = memoize 2 _frames_lr_battery

let frames_lr_battery (nb_steps) = frames_lr_battery_memoized (nb_steps, get_ttl_hash (60. * 10.))

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
                for j in 0 .. NB_LEDS_STICK - 1 do
                    if j < r then rgb1 else rgb2
            ]

            let frame = shift (1, frame) // because my LEDs are shifted.... Trimui QA...
            String.Join("", frame |> List.map toHex)
    ]

    frame

let private frames_lr_wipe_memoized = memoize 2 _frames_lr_wipe

let frames_lr_wipe (nb_steps) = frames_lr_wipe_memoized (nb_steps, get_ttl_hash (30)) // refresh every 30s

let _frames_lr_nexus (nb_steps, ttl_hash: float) =
    let randomColor () = toHex (toRGB (Random.Shared.NextDouble(), 1, 1))
    let randomIndex (_) = Random.Shared.Next(NB_LEDS_STICK - 1)
    let i, j = 0, randomIndex ()
    let k = Seq.initInfinite (randomIndex) |> Seq.find (fun x -> x <> j)
    let leds_index_on = dict [ i, COLOR_RED; j, randomColor (); k, randomColor () ]
    let halfway = nb_steps / 2

    let frame_initial = [
        for i in 0 .. NB_LEDS_STICK - 1 do
            if leds_index_on.ContainsKey(i) then leds_index_on.[i] else COLOR_BLACK
    ]

    let frames = [
        for i in 0 .. nb_steps - 1 do
            let frame = if i < halfway then shift (i / 4, frame_initial) else shift ((nb_steps - i) / 4, frame_initial) // FIXME
            String.Join("", frame)
    ]

    frames

let frames_lr_nexus_memoized = memoize 2 _frames_lr_nexus

let frames_lr_nexus (nb_steps) = frames_lr_nexus_memoized (nb_steps, get_ttl_hash (10))
