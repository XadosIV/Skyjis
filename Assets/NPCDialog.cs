using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialog : MonoBehaviour
{
    public SpriteRenderer key;
    public string dialog;
    bool isSpeaking = false;
    PlayerMovement player;
    DialogManager dm;
    SpriteRenderer sr;
    Rigidbody2D rb;
    void Start()
    {
        key.enabled = false;
        player = FindObjectOfType<PlayerMovement>();
        dm = FindObjectOfType<DialogManager>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (key.enabled && Input.GetButtonDown("Interaction") && !isSpeaking && player.CanInteract()) {
            key.enabled = false;
            StartInteraction();
        }
        /*
        if (rb.velocity.x < 0.001f && rb.velocity.x > -0.001f) {
            rb.velocity = new Vector2(0,rb.velocity.y);
        }*/
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
        cam.SetSize(4f, between);
        dm.StartDialogue(dialog);
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
