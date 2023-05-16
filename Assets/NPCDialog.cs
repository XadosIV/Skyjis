using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialog : MonoBehaviour
{
    public SpriteRenderer key;
    public string dialog_tag;
    bool isSpeaking = false;
    PlayerMovement player;
    DialogManager dm;
    SpriteRenderer sr;
    void Start()
    {
        key.enabled = false;
        player = FindObjectOfType<PlayerMovement>();
        dm = FindObjectOfType<DialogManager>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (key.enabled && Input.GetButtonDown("Interaction") && !isSpeaking && player.CanInteract()) {
            StartInteraction();
        }
    }

    public void StartInteraction() {
        Vector3 between = (transform.position - player.transform.position)/2;
        between.z = -10;

        if (between.x >= 0) { // positive, NPC look left, P look right
            sr.flipX = true;
            if (player.direction == -1) {
                player.direction = 1;
                player.FlipSprite();
            }
        } else {
            sr.flipX = false;
            if (player.direction == 1) {
                player.direction = -1;
                player.FlipSprite();
            }
        }

        isSpeaking = true;
        CameraFollow cam = FindObjectOfType<CameraFollow>();
        cam.SetSize(3f, between);
        dm.StartDialogue(dialog_tag);
        StartCoroutine(CatchEndDialog());
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            key.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            key.enabled = false;
        }
    }
    public IEnumerator CatchEndDialog() {
        while (!player.CanInteract()) {
            yield return new WaitForEndOfFrame();
        }
        CameraFollow cam = FindObjectOfType<CameraFollow>();
        cam.TransformDefault();
        isSpeaking = false;
    }
}
