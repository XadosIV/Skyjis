using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour {
    public string language = "fr";

    public TextAsset DialogsFile;
    public TextAsset TextsFile;

    public Dictionary<string, string> dialogs;
    public Dictionary<string, string> names;
    public Dictionary<string, string> texts;

    public class JSONFormat {
        public string[] tags;
        public string[] names;
        public string[] texts;

        public Dictionary<string, string> toDictionaryTexts() {
            Dictionary<string, string> dico = new Dictionary<string, string>();
            
            for (int i = 0; i < tags.Length; i++) {
                dico.Add(tags[i], texts[i]);
            }

            return dico;
        }

        public Dictionary<string, string> toDictionaryNames() {
            Dictionary<string, string> dico = new Dictionary<string, string>();

            for (int i = 0; i < tags.Length; i++) {
                dico.Add(tags[i], names[i]);
            }

            return dico;
        }
    }

    void Start()
    {
        SetLanguage(language);
    }

    public string LoadDialog(string tag) {
        if (dialogs.ContainsKey(tag)) {
            return dialogs[tag];
        } else {
            return "DIALOG NOT FOUND";
        }
    }

    public string GetName(string tag) {
        if (dialogs.ContainsKey(tag)) {
            return names[tag];
        } else {
            return "???";
        }
    }

    public void LoadText(string tag) {

    }

    public void SetLanguage(string lang) {
        JSONFormat t = JsonUtility.FromJson<JSONFormat>(DialogsFile.text);
        dialogs = t.toDictionaryTexts();
        names = t.toDictionaryNames();
    }

}
