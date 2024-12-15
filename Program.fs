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
let analyzeText (text: string) =
    let cleanedText = cleanText text
    let words = cleanedText.Split([| ' '; '\n'; '\t' |], StringSplitOptions.RemoveEmptyEntries)
    

    let sentences = Regex.Split(text, "(?<=[.!?])\s+")
    let filteredSentences = sentences |> Array.filter (fun s -> s.Trim().Length > 0)

    let paragraphs = text.Split([| '\n' |], StringSplitOptions.RemoveEmptyEntries)

    let wordCount = words.Length
    let sentenceCount = filteredSentences.Length
    let paragraphCount = paragraphs.Length

    let wordFrequency =
        words
        |> Seq.map (fun word -> word.ToLowerInvariant())
        |> Seq.countBy id
        |> Seq.sortByDescending snd

    let averageSentenceLength =
        if sentenceCount > 0 then
            float wordCount / float sentenceCount
        else
            0.0

   
    wordCount, sentenceCount, paragraphCount, wordFrequency, averageSentenceLength
[<EntryPoint>]
let main argv =
    let filePath = "test.txt"
    let fileContent = getTextInput filePath
    if fileContent <> "" then
        printfn "File Content: %s" fileContent
    else
        printfn "No content or error in reading file."
    0 // Return an integer exit code