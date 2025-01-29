module ColorConversion

open System
open System.Drawing

#if WRITE_BINARY
let toHex (r:byte, g:byte, b:byte) =
    ((int r) <<< 16) + ((int g) <<< 8) + int b
#else
let toHex (r:byte, g:byte, b:byte) = r.ToString("X2") + g.ToString("X2") + b.ToString("X2") + " "
#endif

let toHSV (color: Color) : (double * double * double) =
    let max = Math.Max(color.R, Math.Max(color.G, color.B))
    let min = Math.Min(color.R, Math.Min(color.G, color.B))

    let hue = double (color.GetHue())
    let saturation = if max = 0uy then 0.0 else 1.0 - (double min / double max)
    let value = float max / 255.0

    (hue, saturation, value)

let toRGB (hF: double, saturation: double, value: double) =
    let hue = hF*360.
    let hi = int (Math.Floor(hue / 60.0)) % 6
    let f = hue / 60.0 - Math.Floor(hue / 60.0)

    let scaledValue = value * 255.0
    let v = byte scaledValue
    let p = byte (scaledValue * (1.0 - saturation))
    let q = byte (scaledValue * (1.0 - f * saturation))
    let t = byte (scaledValue * (1.0 - (1.0 - f) * saturation))

    match hi with
    | 0 -> v, t, p // Color.FromArgb(255, v, t, p)
    | 1 -> q, v, p //Color.FromArgb(255, q, v, p)
    | 2 -> p, v, t // Color.FromArgb(255, p, v, t)
    | 3 -> p, q, v // Color.FromArgb(255, p, q, v)
    | 4 -> t, p, v //Color.FromArgb(255, t, p, v)
    | _ -> v, p, q //Color.FromArgb(255, v, p, q)
