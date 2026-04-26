using System.Collections.Generic;
using System.Text;

public class DialogueCSVParser
{
    public static Dictionary<string, DialogueData> ParseFile(string csvRawText)
    {
        Dictionary<string, DialogueData> dialogueDatabase = new Dictionary<string, DialogueData>();
     
        string[] rows = csvRawText.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        //Start dari baris ke 2
        for (int i = 1; i < rows.Length; i++)
        {
            string[] cells = SplitCSVRow(rows[i]);

            int col = 0;

            DialogueData data = new DialogueData();
            data.dialogueID = cells[col++].ToLower();
            data.characterName = cells[col++];
            data.dialogueText = cells[col++].Trim('"');
            data.spriteName = cells[col++];
            data.nextDialogueID = cells[col++].ToLower();

            if (!dialogueDatabase.ContainsKey(data.dialogueID))
            {
                dialogueDatabase.Add(data.dialogueID, data);
            }
        }
        
        return dialogueDatabase;
    }

    private static string[] SplitCSVRow(string row)
    {
        List<string> cellStrings = new List<string>();
        bool inQuotes = false;
        StringBuilder currentCellString = new StringBuilder();

        foreach (char c in row)
        {
            if (c == '"') 
            {
                inQuotes = !inQuotes; 
            }
            else if (c == ',' && !inQuotes) 
            {
                cellStrings.Add(currentCellString.ToString());
                currentCellString.Clear();
            }
            else 
            {
                currentCellString.Append(c);
            }
        }

        cellStrings.Add(currentCellString.ToString());
        return cellStrings.ToArray();
    }
}
