module Effects.Dynamic
open ColorConversion
open System

let string_join = String.concat ""
let frames_dynamic S (previous_frame:string list) (nb_steps:int) =
    let nb_leds =  previous_frame.Length
    let random_color (_) = toHex (toRGB (Random.Shared.NextDouble(), S, 1))
    let indexes_infinite = seq {
        while true do
            let indexes = [0..nb_leds-1] |> List.randomShuffle
            for i in indexes do
             yield i
    }

    let add_random_color frame index  = List.updateAt index (random_color()) frame
    let next_frames =
        previous_frame |> Seq.scan add_random_color 

    let frames =
        next_frames indexes_infinite
        |> Seq.skip 1 // ignore preivous frame
        |> Seq.collect (fun e -> [e;e;e;e;e;e]) // slow down
        |> Seq.take nb_steps
        |> List.ofSeq
    frames
    
let frames_dynamic_lr (args:Map<string,string>) =
    let S= match Map.tryFind "saturation" args with | Some(v) -> double v | _ -> 1.0
    let mutable previous_frame = Array.create NB_LEDS_STICK COLOR_BLACK_HEX |> List.ofArray
    fun nb_steps ->
        let frames = frames_dynamic S previous_frame nb_steps
        previous_frame <- List.last frames
        frames |> (List.map string_join)

