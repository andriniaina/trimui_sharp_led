[<AutoOpen>]
module ArrayTools

let shift (x, array: 'a list) =
    let size = array.Length
    let x = x % size

    [
        for i in 0 .. size - 1 do
            array[(i - x + size) % size]
    ]

let shift_and_sample (arr: list<'t>, start: int, n: int) =
    let n_f = float n
    let start_f = float start
    let size = float arr.Length
    let step_f = size / n_f

    [
        for i = 0.0 to n_f - 1.0 do
            arr[int ((start_f + (i * step_f)) % size)]
    ]
