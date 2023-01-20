using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcScript : MonoBehaviour
{
    public SpriteRenderer key;

    private bool isSpeaking = false;

    private DialogManager dm;

    public string[] dialogs;
    public int readFlag;

    void Start()
    {
        dm = FindObjectOfType<DialogManager>();
        key.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (key.enabled && Input.GetButtonDown("Interaction") && !isSpeaking) {
            isSpeaking = true;
            dm.StartDialogue(dialogs, readFlag);
            StartCoroutine(CatchEndDialog());
        }
    }

    public IEnumerator CatchEndDialog() {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        while (player.IsInCinematic()) {
            yield return new WaitForEndOfFrame();
        }
        isSpeaking = false;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            key.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            key.enabled = true;
        }
    }
}
