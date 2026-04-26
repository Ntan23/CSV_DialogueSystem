# Unity Google Sheets Dialogue System

Developed and tested on Unity 6000.3.12f1 [LTS]

A robust and dynamic dialogue management system for Unity that integrates directly with Google Sheets via CSV Web Request. This tool is designed to bridge the gap between narrative design and technical implementation, allowing writers to iterate on game content in real-time.

## 🚀 Key Features

- **Live Sync:** Update the local database from Google Sheets with a single click via a Custom Inspector.
- **Robust CSV Parsing:** Implements a character-by-character "Toggle Quote" parser to handle commas `,` within dialogue strings safely, ensuring data integrity.
- **Memory Optimized:** Utilizes `StringBuilder` for all string processing to minimize Heap Allocation and prevent Garbage Collector (GC) spikes.
- **Case-Insensitive ID Lookup:** Dialogue IDs are automatically normalized using `.ToLower()`, reducing synchronization errors between writers and programmers.
- **Developer Tools:** Includes an Editor-only testing suite to play specific dialogue IDs during Play Mode and a manual sync and download button.

## 📊 Sample Spreadsheet & Data Structure

To test the system or use it as a template, you can access the sample spreadsheet below:
- **[Sample Dialogue Spreadsheet](https://docs.google.com/spreadsheets/d/14LJzCiPpHx-s2SS5w9FwA4Tzwztkfb3c38r7wGQR0fQ/edit?usp=sharing)**

**Required Columns:**
| DialogueID | CharacterName | DialogueText | SpriteName | NextID |
|:---|:---|:---|:---|:---|
| Intro_01 | Tristan | "Hello" | Tristan_Hello | Intro_02 |

## 🛠️ Setup & Configuration

### 1. Google Sheets Configuration (Crucial)
For Unity to fetch the raw data, the spreadsheet must be published to the web as a CSV:
1. Open your Google Sheet.
2. Navigate to **File > Share > Publish to web**.
3. Change "Entire Document" to the specific sheet (e.g., **Sheet1**).
4. Change the format from "Web page" to **Comma-separated values (.csv)**.
5. Click **Publish** and copy the generated link.

### 2. Unity Integration
1. Paste the generated CSV link into the **sheetCSVUrl** on the `DialogueManager`.
2. In the Inspector, click **"Sync & Download Data"**. This saves the data as `DialogueDatabase.txt` in your `Resources` folder.
3. To test, enter a ID in the `dialogueTestID` field and click **"Play Dialogue by ID"** while the game is running.

## 💻 Technical Documentation

### The "Toggle Quote" Logic
Standard parsing methods like `string.Split(',')` fail when dialogue contains punctuation. This system uses a manual state-machine approach:
- It tracks the `inQuotes` state to differentiate between a **Delimiter comma** and a **Textual comma**.
- It strips leading/trailing quotation marks added by Google Sheets during export to ensure clean UI display.

### Data Efficiency
- **Dictionary Implementation:** Dialogue data is mapped to a `Dictionary<string, DialogueData>`
- **Persistent Storage:** The system saves a local copy in `Resources`, allowing the game to run offline using the last successfully synced data.

## 🎨 Credits & Assets
- **Sample Character Sprites:** Placeholder assets generated via AI/Gemini 3 Flash.
