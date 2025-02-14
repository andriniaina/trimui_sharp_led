module Tests

open System
open Xunit
open Effects.Battery
open Effects.Rainbow
open Effects.Dynamic_interpolated
open ColorConversion
open INIWriter
open FS.INIReader
open FS.INIReader
open System.IO

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
    let previous_frame = Array.create NB_LEDS_STICK 0.0 |> List.ofArray
    let frames1 = frames_dynamic_hues 6 previous_frame nb_frames
    Assert.Equal (frames1[0].Length, NB_LEDS_STICK)
    Assert.Equal (frames1.Length, nb_frames)
    
[<Fact>]
let x () =
    let iniPath_input = "some_input.ini"
    let iniPath_output = "some_output.ini"
    try
        use w = new StreamWriter(iniPath_input)
        w.WriteLine("""
[section1]
some_string1=value1
some_string2=value1
some_list=[x y z ]
some_tuple=(a,b,c,)
[section2]
some_string1=value1
some_string2=value1
some_list=[x y z ]
some_tuple=(a,b,c,)
        """)
        w.Close()

        let config = INIParser.readFile iniPath_input |> Option.get
        writeFile iniPath_output config
        let new_config = INIParser.readFile iniPath_output |> Option.get
        Assert.Equivalent(config, new_config)
    finally
        File.Delete(iniPath_input)
        File.Delete(iniPath_output)
    
