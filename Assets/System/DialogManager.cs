using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    private Image image;
    private PlayerMovement player;
    private GameManager gm;
    private LanguageManager lm;

    int blockActionId;

    public float textSpeed;

    private int index;
    private List<string> dialog;

    public bool isUsed = false;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        lm = FindObjectOfType<LanguageManager>();
        image = GetComponent<Image>();
        Show(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("MeleeHit") && image.enabled) {
            if (textComponent.text == dialog[index]) {
                NextLine();
            } else {
                StopAllCoroutines();
                textComponent.text = dialog[index];
            }
        }
    }
    void Show(bool showing) {
        image.enabled = showing;
        textComponent.enabled = showing;
        isUsed = showing;
    }

    public void StartDialogue(string tag) {
        player = FindObjectOfType<PlayerMovement>();
        blockActionId = player.StartCinematic();

        string text = lm.LoadDialog(tag);


        dialog = new List<string>();
        foreach (string t in text.Split("/")) {
            string tt = t;
            if (t.StartsWith('\n')) {
                tt = t[1..];
            }
            if (t.EndsWith('\n')) {
                tt = tt.Remove(tt.Length - 1, 1);
            }
            dialog.Add(tt);
        }

        Show(true);

        index = -1; // first line of nextline is index++ so we want to start to zero
        NextLine();
    }

    IEnumerator TypeLine() {
        textComponent.text = string.Empty;
        foreach (char c in dialog[index]) {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }



    void NextLine() {
        if (index < dialog.Count -1) {
            index++;
            StartCoroutine(TypeLine());
        } else {
            player.ExitCinematic(blockActionId);
            Show(false);
        }
    }
}
