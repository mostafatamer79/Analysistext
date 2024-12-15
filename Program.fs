open System
open System.IO
open System.Text.RegularExpressions
open System.Windows.Forms
open System.Drawing
open System.Threading.Tasks
open System.Threading

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

    type MainForm() as this =
    inherit Form()

    do
        this.Text <- "Text Analysis"
        this.Width <- 600
        this.Height <- 400
        
    
        try
            let backgroundImage = Image.FromFile("_HzwXzIFTtqR_QuVl7hp5w.jpeg") 
            this.BackgroundImage <- backgroundImage
            this.BackgroundImageLayout <- ImageLayout.Stretch 
        with
        | :? FileNotFoundException -> 
            MessageBox.Show("Background image not found.") |> ignore
            this.BackColor <- Color.WhiteSmoke  

        this.Font <- new Font("Arial", 10.0f)

       

        let buttonWidth = 150
        let buttonHeight = 50
        let horizontalSpacing = 20
        
        let totalButtonWidth = buttonWidth * 2 + horizontalSpacing

       
        let startX = (this.ClientSize.Width - totalButtonWidth) / 2

        let chooseFileButton = new Button(Text = "Choose File", Width = buttonWidth, Height = buttonHeight)
        chooseFileButton.BackColor <- Color.Black
        chooseFileButton.ForeColor <- Color.White
        chooseFileButton.FlatStyle <- FlatStyle.Flat
        chooseFileButton.FlatAppearance.BorderSize <- 0
        chooseFileButton.Location <- Point(startX, 100) 
        let enterTextButton = new Button(Text = "Enter Text", Width = buttonWidth, Height = buttonHeight)
        enterTextButton.BackColor <- Color.Black
        enterTextButton.ForeColor <- Color.White
        enterTextButton.FlatStyle <- FlatStyle.Flat
        enterTextButton.FlatAppearance.BorderSize <- 0
        enterTextButton.Location <- Point(chooseFileButton.Right + horizontalSpacing, 100) 

      
        let inputChoiceLabel = new Label(Text = "Choose input method:", Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter)
        inputChoiceLabel.ForeColor <- Color.White
        inputChoiceLabel.BackColor <- Color.FromArgb(0, 122, 204) 

        let inputTextBox = new TextBox(Dock = DockStyle.Fill, Multiline = true, Visible = false)
        inputTextBox.BackColor <- Color.White
        inputTextBox.ForeColor <- Color.Black
        inputTextBox.Font <- new Font("Arial", 12.0f)
        inputTextBox.BorderStyle <- BorderStyle.None

        let resultsButton = new Button(Text = "Analyze Text", Dock = DockStyle.Bottom, Height = 40, Visible = false)
        resultsButton.BackColor <- Color.FromArgb(100, 149, 237) 
        resultsButton.ForeColor <- Color.White
        resultsButton.FlatStyle <- FlatStyle.Flat
        resultsButton.FlatAppearance.BorderSize <- 0

        let backButton = new Button(Text = "Back", Dock = DockStyle.Bottom, Height = 40, Visible = false)
        backButton.BackColor <- Color.IndianRed
        backButton.ForeColor <- Color.White
        backButton.FlatStyle <- FlatStyle.Flat
        backButton.FlatAppearance.BorderSize <- 0

        let showResultsForm (analysisResults) =
            let wordCount, sentenceCount, paragraphCount, wordFrequency, avgSentenceLength = analysisResults
            let resultsForm = new Form(Text = "Analysis Results", Width = 600, Height = 500)
            resultsForm.BackColor <- Color.WhiteSmoke

            let resultRichTextBox = new RichTextBox(Dock = DockStyle.Fill, ReadOnly = true)
            resultRichTextBox.BackColor <- Color.FromArgb(245, 245, 245)
            resultRichTextBox.ForeColor <- Color.Black
            resultRichTextBox.Font <- new Font("Arial", 12.0f)
            resultRichTextBox.Clear()  

            resultRichTextBox.AppendText(sprintf "Total Words: %d\n" wordCount)
            resultRichTextBox.SelectionColor <- Color.FromArgb(25, 25, 112) 
            resultRichTextBox.AppendText(sprintf "Total Sentences: %d\n" sentenceCount)
            resultRichTextBox.SelectionColor <- Color.ForestGreen
            resultRichTextBox.AppendText(sprintf "Total Paragraphs: %d\n" paragraphCount)
            resultRichTextBox.SelectionColor <- Color.DarkOrange
            resultRichTextBox.AppendText(sprintf "Average Sentence Length: %.2f words\n" avgSentenceLength)
            resultRichTextBox.SelectionColor <- Color.Indigo
            resultRichTextBox.AppendText("\nTop 5 Words Frequency:\n")
            resultRichTextBox.SelectionColor <- Color.Purple

            wordFrequency |> Seq.take 5 |> Seq.iter (fun (word, count) ->
                resultRichTextBox.AppendText(sprintf "%s: %d\n" word count)
                resultRichTextBox.SelectionColor <- Color.Black
            )

            resultsForm.Controls.Add(resultRichTextBox)
            resultsForm.Show()



[<EntryPoint>]
let main argv =
    let filePath = "test.txt"
    let fileContent = getTextInput filePath
    if fileContent <> "" then
        printfn "File Content: %s" fileContent
    else
        printfn "No content or error in reading file."
    0 // Return an integer exit code