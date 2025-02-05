module Effects.Dynamic_interpolated
open ColorConversion
open System
open System.Collections.Generic


let string_join = String.concat ""
let frames_dynamic_hues interpolation_length (previous_frame_hues:double list) (nb_steps:int) =
    let nb_leds =  previous_frame_hues.Length
    let random_color (_) = double (Random.Shared.NextDouble())
    let indexes_infinite = seq {
        while true do
            let indexes = [0..nb_leds-1] |> List.randomShuffle
            for i in indexes do
             yield i
    }

    let add_random_color (frames: double list list) index  =
        let frame = List.last frames
        let old_color = List.item index frame
        let new_color = random_color ()
        let new_hues = interpolate interpolation_length old_color new_color |> Seq.skip 1
        let new_frames = new_hues |> Seq.map (fun h -> List.updateAt index h frame)
        new_frames |> List.ofSeq

    let frames =
        indexes_infinite
        |> Seq.scan add_random_color [previous_frame_hues]
        |> Seq.skip 1 // ignore previous frame
        |> Seq.take (nb_steps/interpolation_length)
        |> Seq.collect id
        |> List.ofSeq
    frames
    
let frames_dynamic_interpolated_lr (args:Map<string,string>) =
    let S= match Map.tryFind "saturation" args with | Some(v) -> double v | _ -> 1.0
    let INTERPOLATION_LENGTH= match Map.tryFind "slow-down" args with | Some(v) -> int v | _ -> 6
    let mutable previous_frame = Array.create NB_LEDS_STICK 0.0 |> List.ofArray
    fun nb_steps ->
        let frames = frames_dynamic_hues INTERPOLATION_LENGTH previous_frame nb_steps
        previous_frame <- List.last frames
        frames
            |> Seq.map (Seq.map (fun hue -> toHex(toRGB(hue,S,1.0))))
            |> Seq.map string_join
            |> List.ofSeq
