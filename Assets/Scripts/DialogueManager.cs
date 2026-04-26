using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;





#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueManager : MonoBehaviour
{
    #region Singleton
    public static DialogueManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public const string sheetCSVUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vR35sWdBZWW_yNOSmShlHvHLa7zZVV2iafJyEM6OubabnW05Z1UPNbeHx4ITYHFtZExqLfVS15tgUpg/pub?gid=0&single=true&output=csv";
    private const string fileName = "DialogueDatabase.txt";

    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image characterImage;
    [SerializeField] private Button nextDialogueButton;

    private string currentDialogueID;
    [Header("For Testing")]
    public string dialogueTestID;
    private Dictionary<string, DialogueData> dialogueDatabase;

    public void SyncAndDownloadData()
    {
        StartCoroutine(DownloadAndSaveCSV());
    }

    IEnumerator DownloadAndSaveCSV()
    {
        Debug.Log("Downloading latest data...");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(sheetCSVUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string downloadedText = webRequest.downloadHandler.text;

                #if UNITY_EDITOR
                string path = Application.dataPath + "/Resources/" + fileName;
                File.WriteAllText(path, downloadedText);
                AssetDatabase.Refresh();
                
                Debug.Log($"Success! {fileName} has been updated at {path}");
                #endif
            }
            else
            {
                Debug.LogError("Download Failed: " + webRequest.error);
            }
        }
    }

    void Start()
    {
        LoadDatabase();
        nextDialogueButton.onClick.AddListener(NextDialogue);
    }   

    public void LoadDatabase()
    {
        TextAsset dialogueDatabaseTextAsset = Resources.Load<TextAsset>("DialogueDatabase");
        
        if (dialogueDatabaseTextAsset != null)
        {
            dialogueDatabase = DialogueCSVParser.ParseFile(dialogueDatabaseTextAsset.text);
        }
    }

    public void ShowDialogue(string id)
    {
        if (dialogueDatabase.ContainsKey(id))
        {
            DialogueData data = dialogueDatabase[id];
            characterNameText.text = data.characterName;
            dialogueText.text = data.dialogueText;
            characterImage.sprite = Resources.Load<Sprite>(data.spriteName);

            currentDialogueID = id;
        }
    }
    
    public void NextDialogue()
    {
        if (dialogueDatabase.ContainsKey(currentDialogueID))
        {
            DialogueData data = dialogueDatabase[currentDialogueID];

            if (data.nextDialogueID != "END")
            {
                ShowDialogue(data.nextDialogueID);
            }
            else
            {
                Debug.Log("Dialogue Finished!");
            }
        }
    }

    public string GetFirstDialogueID()
    {
        return dialogueDatabase.Keys.First();
    }

    public bool IsDialogueIDAvailable(string id)
    {
        return dialogueDatabase.ContainsKey(id);
    }
}

[CustomEditor(typeof(DialogueManager))]
public class DialogueManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DialogueManager dialogueManager = (DialogueManager)target;

        GUILayout.Space(10);
        
        if (GUILayout.Button("Sync & Download Data", GUILayout.Height(30)))
        {
            dialogueManager.SyncAndDownloadData();
        }

        GUILayout.Space(5);

        GUI.backgroundColor = Color.green; 
        if (GUILayout.Button("Play Dialogue by ID (Dialogue Test ID)", GUILayout.Height(30)))
        {
            if (!Application.isPlaying) 
            {
                Debug.LogWarning("You Need To Play The Game First !");
                return;
            }
    
            string dialogueTestID = dialogueManager.dialogueTestID.ToLower();
            
            if (string.IsNullOrEmpty(dialogueTestID))
            {
                Debug.Log("Dialogue Test ID is Empty. Start From The First Dialogue");
                dialogueTestID = dialogueManager.GetFirstDialogueID();
            }
            else if (!dialogueManager.IsDialogueIDAvailable(dialogueTestID))
            {
                Debug.LogWarning("Dialogue ID Not Available !");
                return;
            }

            dialogueManager.ShowDialogue(dialogueTestID);
        }
        GUI.backgroundColor = Color.white; 
    }
}