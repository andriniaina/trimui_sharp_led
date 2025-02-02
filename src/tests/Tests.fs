module Tests

open System
open Xunit
open Effects.Battery
open Effects.Rainbow
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
