open System
open System.Collections.Generic
open System.IO
open FS.INIReader
open System.Text
open Effects.Battery
open Effects.Nexus
open Effects.Rainbow
open Effects.Wipe
open Memoizer


let get_effect_function (config: INIAst.INIData, led_type) =
    let effect_name = INIExtr.fieldString led_type "effect_name" config |> Option.defaultValue "rainbow"

    match led_type with
    | "left"
    | "right" ->
        match effect_name with
        | "rainbow" -> memoize 1 frames_lr_rainbow
        | "battery" -> frames_lr_battery
        | "wipe" -> frames_lr_wipe
        | "nexus" -> frames_lr_nexus
        | _ -> raise (KeyNotFoundException(effect_name))
    | "middle" ->
        match effect_name with
        | "rainbow" -> memoize 1 frames_m_rainbow
        | "battery" -> frames_m_battery
        | _ -> raise (KeyNotFoundException(effect_name))
    | _ -> raise (KeyNotFoundException(led_type))


let build_frames (config: INIAst.INIData, nb_steps) =
    let frames_hex_l = get_effect_function (config, "left") (nb_steps)
    let frames_hex_r = get_effect_function (config, "right") (nb_steps)
    let frames_hex_m = get_effect_function (config, "middle") (nb_steps)

    let frames_hex =
        [
            for frame_m, frame_l, frame_r in Seq.zip3 frames_hex_m frames_hex_l frames_hex_r do
#if WRITE_BINARY
                List.concat [ frame_m; frame_l; frame_r ]
#else
                frame_m + frame_l + frame_r
#endif
        ]

    frames_hex

[<EntryPoint>]
let main args =
    printfn "----- START in %s -----" (AppContext.BaseDirectory)
    let iniFile = sprintf "%s/trimui_sharp_led.ini" AppContext.BaseDirectory
    let config = INIParser.readFile iniFile |> Option.get

    let NB_STEPS = INIExtr.fieldInt "settings" "NB_STEPS" config |> Option.defaultValue 60
    let FPS = INIExtr.fieldInt "settings" "FPS" config |> Option.defaultValue 30
    let SLEEP_ms = 1000 / FPS

#if WRITE_BINARY
    let frame_hex_filename = "/sys/class/led_anim/frame"
#else
    let frame_hex_filename = "/sys/class/led_anim/frame_hex"
#endif
    let filename = if File.Exists(frame_hex_filename) then frame_hex_filename else "led_simulator.txt"
    use fp = new System.IO.FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write)

    while true do
        let frames_hex = build_frames (config, NB_STEPS)

        for _ = 1 to 20 * FPS / NB_STEPS do
            for frame in frames_hex do
                fp.Seek(0, SeekOrigin.Begin) |> ignore
#if WRITE_BINARY
                for i in frame do
                    let bytes = BitConverter.GetBytes(i)
                    fp.Write(bytes, 0, bytes.Length)
#else
                fp.Write(Encoding.ASCII.GetBytes(frame))
#endif
                System.Threading.Thread.Sleep(SLEEP_ms)

    0
