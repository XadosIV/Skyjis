using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLeader : MonoBehaviour
{
    public Transform[] spawnpoints; 
    public Transform[] colliders; 
    public Transform[] stopPoints; 
    public string[] dialogs;

    public int index;

    public float moveSpeed;
    public bool move;

    private NPCDialog npcd;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator anim;

    Vector3 velocity = Vector3.zero;

    void Start()
    {
        npcd = GetComponent<NPCDialog>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        transform.position = spawnpoints[0].position;
        index = 0;
        for (int i = 0; i < colliders.Length; i++) {
            colliders[i].gameObject.SetActive(i == index);
        }
        npcd.dialog = dialogs[0];
    }

    void Update() {
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));


        if (CheckpointContainPlayer()) {
            colliders[index].gameObject.SetActive(false);
            npcd.StartInteraction();
            StartCoroutine(GetStopInteraction());
        }

        if (moveSpeed > 0) { // on se déplace vers la droite
            if (transform.position.x >= stopPoints[index].position.x) {
                move = false;
            }
        } else { // on se déplace vers la gauche
            if (transform.position.x <= stopPoints[index].position.x) {
                move = false;
            }
        }
    }

    void FixedUpdate() {
        if (move) {
            float horizontalMovement = moveSpeed * Time.fixedDeltaTime;
            Vector3 targetVelocity = new Vector2(horizontalMovement, rb.velocity.y);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
        }
        else {
            rb.velocity = Vector3.zero;
        }
    }

    IEnumerator GetStopInteraction() {
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        while (!pm.CanInteract()) {
            yield return 0;
        }

        int id = pm.AddBlockAction(new bool[5] { true, true, true, false, true }); // dont block interaction so cam can zoom out
        sr.flipX = !sr.flipX;
        yield return new WaitForSeconds(0.8f);


        move = true;
        while (move) {
            yield return 0;
        }


        index++;
        if (index < dialogs.Length) {
            npcd.dialog = dialogs[index];
        }

        for (int i = 0; i < colliders.Length; i++) {
            colliders[i].gameObject.SetActive(i == index);
        }

        sr.flipX = !sr.flipX;
        transform.position = spawnpoints[index].position;
        pm.RemoveBlockAction(id);

        if (index == spawnpoints.Length - 1) {
            enabled = false;
        }
    }

    bool CheckpointContainPlayer() {
        Collider2D[] tab = new Collider2D[10];

        int nbCollide = colliders[index].GetComponent<Collider2D>().OverlapCollider(new ContactFilter2D(), tab);
        for (int i = 0; i < nbCollide; i++) {
            if (tab[i].CompareTag("Player")){
                return true;
            }
        }
        return false;
    }
}
