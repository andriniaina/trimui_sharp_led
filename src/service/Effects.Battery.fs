module Effects.Battery

open System
open System.IO
open System.Text
open ColorConversion
open Memoizer
open ArrayTools

let private read_battery_capacity () =
    let CAPACITY_FILENAME = "/sys/class/power_supply/axp2202-battery/capacity"
    if File.Exists(CAPACITY_FILENAME) then File.ReadAllText(CAPACITY_FILENAME).Trim() |> int else 0 // fake

let _frames_battery (nb_steps, ttl_hash: float) =
    let capacity = read_battery_capacity () |> double
    let h_GREEN = 130.0
    let h = h_GREEN * capacity / 100.0
    let hex = toHex (toRGB (h, 1, 1))
    List.init nb_steps (fun _ -> hex)

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

let private frames_battery_memoized = memoize 2 (_frames_battery)

let frames_m_battery (nb_steps) = frames_battery_memoized (nb_steps, get_ttl_hash (60. * 10.))

let private frames_lr_battery_memoized = memoize 2 (_frames_lr_battery)

let frames_lr_battery (nb_steps) = frames_lr_battery_memoized (nb_steps, get_ttl_hash (60. * 10.))
