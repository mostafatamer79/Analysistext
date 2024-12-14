open System
open System.IO
open System.Text.RegularExpressions
open System.Windows.Forms

let cleanText (text: string) =
    let cleaned = Regex.Replace(text, "[^\w\s#]", "") 
    cleaned

let getTextInput (filePath: string) =
    try
        if File.Exists filePath then
            use reader = new StreamReader(filePath, true) // Auto-detect file encoding
            let fileText = reader.ReadToEnd()
            if not (String.IsNullOrWhiteSpace(fileText)) then
                fileText
            else
                MessageBox.Show("File is empty.") |> ignore
                ""
        else
            MessageBox.Show("File not found.") |> ignore
            ""
    with
    | :? IOException as ex -> 
        MessageBox.Show($"Error reading file: {ex.Message}") |> ignore
        ""
    | ex -> 
        MessageBox.Show($"Unknown error: {ex.Message}") |> ignore
        ""

[<EntryPoint>]
let main argv =
    let filePath = "test.txt"
    let fileContent = getTextInput filePath
    if fileContent <> "" then
        printfn "File Content: %s" fileContent
    else
        printfn "No content or error in reading file."
    0 // Return an integer exit code