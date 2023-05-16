using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mila_Exile_Cave : MonoBehaviour {
    public Transform[] spawnpoints;
    public Transform[] stopMoving;
    public Transform[] checkpoints;

    public string[] dialogs_tag;
    int stopMovingIndex;
    int spawnpointsIndex;
    int checkpointsIndex;

    public float moveSpeed;

    bool move = true;

    Vector3 velocity = Vector3.zero;

    private Animator anim;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private NPCDialog npcd;
    void Start() {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        npcd = GetComponent<NPCDialog>();
        stopMovingIndex = 0;
        spawnpointsIndex = 0;
        SetCheckpointIndex(0);
        Teleport();
    }

    public void Teleport(bool incrementCheckpoint = false) {
        sr.flipX = true;
        transform.position = spawnpoints[spawnpointsIndex].position;
        if (dialogs_tag.Length > spawnpointsIndex) {
            npcd.dialog_tag = dialogs_tag[spawnpointsIndex];
        }
        spawnpointsIndex++;
        if (incrementCheckpoint) {
            SetCheckpointIndex(checkpointsIndex+1);
        }
    }

    void SetCheckpointIndex(int index) {
        checkpointsIndex = index;
        for (int i = 0; i < checkpoints.Length; i++) {
            checkpoints[i].gameObject.SetActive(i == checkpointsIndex);
        }
    }

    void Update() {
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        if (stopMovingIndex == 0) {
            if (transform.position.x < stopMoving[stopMovingIndex].position.x && move) {
                move = false;
                StartCoroutine(InvocationAnimation());
            }
        } else {
            if (transform.position.x > stopMoving[stopMovingIndex].position.x && move) {
                move = false;
            }
        }

        CheckpointContainPlayer();
    }

    IEnumerator InvocationAnimation() {
        yield return new WaitForSeconds(1f);
        anim.SetBool("special", true);
        FindObjectOfType<Seal>().Explode();
        yield return new WaitForSeconds(4f);
        anim.SetBool("special", false);
        yield return new WaitForSeconds(1.7f);
        npcd.StartInteraction();
        StartCoroutine(GetStopInteraction());
    }

    IEnumerator ExitCave() {
        npcd.StartInteraction();

        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        while (!pm.CanInteract()) {
            yield return 0;
        }

        int id = pm.AddBlockAction(new bool[5] { true, true, true, false, true }); // dont block interaction so cam can zoom out
        sr.flipX = !sr.flipX;
        yield return new WaitForSeconds(0.2f);
        stopMovingIndex++;
        move = true;
        while (move) {
            yield return 0;
        }
        pm.RemoveBlockAction(id);
        Teleport();
    }

 

    IEnumerator GetStopInteraction() {
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        while (!pm.CanInteract()) {
            yield return 0;
        }

        int id = pm.AddBlockAction(new bool[5] { true, true, true, false, true }); // dont block interaction so cam can zoom out
        sr.flipX = !sr.flipX;
        yield return new WaitForSeconds(0.8f);
        stopMovingIndex++;
        moveSpeed *= -1.5f;
        move = true;
        while (move) {
            yield return 0;
        }
        sr.flipX = !sr.flipX;
        Teleport();
        pm.RemoveBlockAction(id);
    }

    void CheckpointContainPlayer() {
        Collider2D[] tab = new Collider2D[10];

        if (checkpointsIndex < checkpoints.Length) {
            int index = checkpoints[checkpointsIndex].GetComponent<Collider2D>().OverlapCollider(new ContactFilter2D(), tab);
            for (int i = 0; i < index; i++) {
                if (tab[i].CompareTag("Player")) {
                    if (checkpointsIndex == 3) {
                        checkpoints[checkpointsIndex].gameObject.SetActive(false);
                        StartCoroutine(ExitCave());
                        return;
                    } else {
                        Teleport(true);
                        return;
                    }
                    
                    
                }
            }
        }
    }

    void FixedUpdate() {
        if (move) {
            float horizontalMovement = -moveSpeed * Time.fixedDeltaTime;
            Vector3 targetVelocity = new Vector2(horizontalMovement, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
        } else {
            rb.velocity = Vector3.zero;
        }
    }
}