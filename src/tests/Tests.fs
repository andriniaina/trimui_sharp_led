module Tests

open System
open Xunit
open Effects.Battery
open Effects.Rainbow
open Effects.Dynamic
open ColorConversion

[<Fact>]
let ``test Effects.Battery frames`` () =
    let green = toRGB(COLOR_GREEN_HUE,1,1) |> toHex
    Assert.Equal(green, 100 |> __frames_battery  (1,0.0) |> List.head)
    
    Assert.Equal("FF0000 ", 0 |> __frames_battery  (1,0.0) |> List.head)

[<Fact>]
let ``test Effects.Rainbow frames`` () =
    let frames = _frames_m_rainbow 10
    Assert.Equal("FF0000 ", frames.[0])
    Assert.True(frames.[frames.Length-1].StartsWith("FF"))

[<Fact>]
let ``test Effects.Dynamic frames`` () =
    let nb_frames = 10
    let previous_frame = Array.create NB_LEDS_STICK COLOR_BLACK_HEX |> List.ofArray
    let frames1 = frames_dynamic  previous_frame nb_frames
    Assert.Equal (frames1[0].Length, NB_LEDS_STICK)
    Assert.Equal (frames1.Length, nb_frames)
    
    (*
    let first_frame = frames1 |> List.head
    Assert.Equivalent (previous_frame, first_frame)

    let previous_frame = List.last frames1
    let frames2 = frames_dynamic  previous_frame 10 
    Assert.Equal (frames2.Length, NB_LEDS_STICK)
    Assert.Equivalent (previous_frame, frames2 |> List.head)
    *)