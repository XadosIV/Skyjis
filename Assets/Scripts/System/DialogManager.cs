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
    private GameManagerScript gm;

    public float textSpeed;

    private List<string> dialog;

    private int index;

    void Start()
    {
        gm = FindObjectOfType<GameManagerScript>();
        image = GetComponent<Image>();
        Show(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("MeleeHit") && image.enabled) {
            if (textComponent.text == dialog[index]) {
                NextLine();
            }
            else {
                StopAllCoroutines();
                textComponent.text = dialog[index];
            }
        }
    }
    void Show(bool showing) {
        image.enabled = showing;
        textComponent.enabled = showing;
    }

    private List<string> LoadFromFlag(string[] lines, int readFlag) {
        int dialogState = gm.saveData.flags[readFlag];
        bool saveDialog = false;
        List<string> dialog = new List<string>();
        foreach (string line in lines) {
            if (line.StartsWith("ID:")) {
                int id = int.Parse(line.Split(":")[1]);
                if (id == dialogState) {
                    saveDialog = true;
                }
                else if (saveDialog) {
                    break;
                }
            }
            else if (line.StartsWith("TAG:")) {
                int index = int.Parse(line.Split(":")[1]);
                int value = int.Parse(line.Split(":")[2]);
                gm.saveData.flags[index] = value;
            }
            else {
                if (saveDialog) {
                    dialog.Add(line);
                }
            }
        }
        return dialog;
    }

    public void StartDialogue(string[] lines, int? readFlag) {
        if (readFlag == null) {
            readFlag = 0;
        }
        Show(true);
        index = 0;
        dialog = LoadFromFlag(lines, (int)readFlag);
        player = FindObjectOfType<PlayerMovement>();
        player.StartCinematic(true);
        StartCoroutine(TypeLine());
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
            player.ExitCinematic();
            Show(false);
        }
    }
}
