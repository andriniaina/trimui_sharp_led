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
open ColorConversion
open Effects.Dynamic

let ASCII = Encoding.ASCII

type EffectFunction = int -> list<string>


module LED_TYPE =
    [<Literal>]
    let LEFT = "left"

    [<Literal>]
    let RIGHT = "right"

    [<Literal>]
    let MIDDLE = "middle"

type Config = {
    fn_left: EffectFunction
    fn_right: EffectFunction
    fn_middle: EffectFunction

    nb_steps: int
    sleep_ms: int
}


let build_effect_function (config: INIAst.INIData, led_type) =
    let led_type_name = led_type.ToString()
    let effect_name = INIExtr.fieldString (led_type.ToString()) "effect_name" config |> Option.defaultValue "rainbow"

    match led_type with
    | LED_TYPE.LEFT
    | LED_TYPE.RIGHT ->
        match effect_name with
        | "rainbow" -> frames_lr_rainbow
        | "battery" -> frames_lr_battery
        | "wipe" -> frames_lr_wipe
        | "nexus" -> frames_lr_nexus
        | "dynamic" -> frames_dynamic_lr
        | _ as v ->
            printfn "Wrong value %s for %s" v led_type_name
            frames_lr_nexus // raise (KeyNotFoundException(effect_name))
    | LED_TYPE.MIDDLE ->
        match effect_name with
        | "rainbow" -> frames_m_rainbow
        | "battery" -> frames_m_battery
        | _ as v ->
            printfn "Wrong value %s for %s" v led_type_name
            frames_m_rainbow // raise (KeyNotFoundException(effect_name))
    | _ -> raise (KeyNotFoundException(led_type))


let build_frames (config: Config) =
    let nb_steps = config.nb_steps
    let frames_hex_m = config.fn_middle (nb_steps)
    let frames_hex_l = config.fn_left (nb_steps)
    let frames_hex_r = config.fn_right (nb_steps)
    
    List.zip3 frames_hex_m frames_hex_l frames_hex_r

let readConfig iniFile =
    printfn "Reading config file %s" iniFile
    let config = INIParser.readFile iniFile |> Option.get
    let fps = INIExtr.fieldInt "settings" "FPS" config |> Option.defaultValue 30

    let config = {
        fn_left = build_effect_function (config, LED_TYPE.LEFT)
        fn_middle = build_effect_function (config, LED_TYPE.MIDDLE)
        fn_right = build_effect_function (config, LED_TYPE.RIGHT)
        nb_steps = INIExtr.fieldInt "settings" "NB_STEPS" config |> Option.defaultValue 60
        sleep_ms = 1000 / fps
    }
    printfn "Config %s" (config.ToString())
    config

let toBuffer (m:string,l:string,r:string) buffer=
    ASCII.GetBytes(m,0,m.Length,buffer,INDEX_M)  |> ignore
    ASCII.GetBytes(l,0,l.Length,buffer,INDEX_L)  |> ignore
    ASCII.GetBytes(r,0,r.Length,buffer,INDEX_R)  |> ignore
    buffer

let reinitializeLeds() =
    if File.Exists "/sys/class/led_anim/max_scale" then
        use device = new FileStream("/sys/class/led_anim/max_scale", FileMode.OpenOrCreate, FileAccess.Write)
        device.Write(ASCII.GetBytes("14"))

[<EntryPoint>]
let main args =
    printfn "----- Started in %s -----" (AppContext.BaseDirectory)
    let iniFile = sprintf "%s/trimui_sharp_led.ini" AppContext.BaseDirectory

    let frame_hex_filename = "/sys/class/led_anim/frame_hex"
    let filename = if File.Exists(frame_hex_filename) then frame_hex_filename else "led_simulator.txt"
    use device = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write)

    let buffer = Array.create (RGB_WIDTH*(NB_LEDS_MIDDLE+NB_LEDS_STICK*2)) 0uy

    let mutable config = readConfig iniFile
    let mutable frames_hex = build_frames(config)
    use watcher = new FileSystemWatcher(Path.GetDirectoryName(iniFile), "*.ini", EnableRaisingEvents = true)
    watcher.Changed.Add(fun _ -> 
        reinitializeLeds ()
        config <- readConfig iniFile
        frames_hex <- build_frames(config)
    )

    reinitializeLeds ()
    while true do
        frames_hex <- build_frames(config)
        for m_l_r in frames_hex do
            device.Seek(0, SeekOrigin.Begin) |> ignore
            device.Write(toBuffer m_l_r buffer)
            System.Threading.Thread.Sleep(config.sleep_ms)

    0
