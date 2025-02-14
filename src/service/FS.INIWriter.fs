module INIWriter

open FS.INIReader.INIAst
open System
open System.IO


let rec writeValue (writer: TextWriter) v =
    let writeValues (writer: TextWriter) (separator:string) vs =
        for v in vs do
            writeValue writer v  
            writer.Write(separator)
    match v with
    | INIEmpty -> writer.Write("")
    | INIString(e) -> writer.Write(e)
    | INIList(l) ->
        writer.Write("[")
        writeValues writer " " l
        writer.Write("]")
    | INITuple(l) ->
        writer.Write("(")
        writeValues writer "," l
        writer.Write(")")

let writeSection (writer: TextWriter) (k: string) (values: Map<INIKey, INIValue>) =
    writer.WriteLine($"[{k}]")

    for v in values do
        writer.Write(v.Key)
        writer.Write("=")
        writeValue writer v.Value
        writer.WriteLine("")



let writeFile (iniPath: string) (data: INIData) =
    use writer = new StreamWriter(iniPath)

    for kv in data do
        writeSection writer kv.Key kv.Value
