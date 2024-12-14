open System
open System.Text.RegularExpressions

let cleanText (text: string) =
    let cleaned = Regex.Replace(text, "[^\w\s#]", "") 
    cleaned


[<EntryPoint>]
let main argv =
    let inputText = "Hello World!"

    let cleanedText = cleanText inputText

    printfn "Original text: %s" inputText
    printfn "Cleaned text: %s" cleanedText

    0
