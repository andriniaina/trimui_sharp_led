[<AutoOpen>]
module Constants

open System
open System.IO
open ColorConversion
open Memoizer
open ArrayTools

let NB_LEDS_MIDDLE = 1
let NB_LEDS_STICK = 11

let INDEX_M = 0
let INDEX_L = RGB_WIDTH*NB_LEDS_MIDDLE
let INDEX_R = RGB_WIDTH*(NB_LEDS_MIDDLE+NB_LEDS_STICK)

let COLOR_BLACK = (0uy, 0uy, 0uy)
let COLOR_BLACK_HEX = toHex COLOR_BLACK
let COLOR_WHITE = (255uy, 255uy, 255uy)
let COLOR_WHITE_HEX = toHex COLOR_WHITE 
let COLOR_RED_HEX = toHex (255uy, 0uy, 0uy)
