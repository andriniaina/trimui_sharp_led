module Memoizer
open System.Collections.Generic

let memoize max_size f =
    let dict = Dictionary<_, _>()
    let keys = Queue<_>()

    fun arg ->
        let exist, value = dict.TryGetValue arg

        match exist with
        | true -> value
        | _ ->
            let value = f arg
            dict.Add(arg, value)
            keys.Enqueue(arg)
            if dict.Count>max_size then                
                dict.Remove(keys.Dequeue()) |> ignore
            value
