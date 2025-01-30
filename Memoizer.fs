[<AutoOpen>]
module Memoizer
open System.Collections.Generic
open System

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

let get_ttl_hash (seconds) =
    let t = DateTime.UtcNow - new DateTime(1970, 1, 1)
    Math.Round(t.TotalSeconds / seconds)
