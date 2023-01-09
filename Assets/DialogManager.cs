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

    public float textSpeed;

    private string[] dialog;

    private int index;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
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

    public void StartDialogue(string[] lines) {
        player.StartCinematic();
        Show(true);
        index = 0;
        dialog = lines;
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
        if (index < dialog.Length -1) {
            index++;
            StartCoroutine(TypeLine());
        } else {
            player.ExitCinematic();
            Show(false);
        }
    }
}
