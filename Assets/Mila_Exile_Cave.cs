using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mila_Exile_Cave : MonoBehaviour {
    public Transform[] stopMoving;
    public Transform spawnPoint;
    public Transform endPoint;
    public string introductionTag;
    public string exitTag;
    public Transform colliderInteract;
    
    private int stopIndex;
    public float moveSpeed;
    private bool move;
    Vector3 velocity = Vector3.zero;

    private Animator anim;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private NPCDialog npcd;
    private NPCFollower npcf;

    private bool ended = false;

    private void Awake() {
        npcf = GetComponent<NPCFollower>();
        npcf.isFollowing = false;
    }

    void Start() {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        npcd = GetComponent<NPCDialog>();

        npcd.dialog = introductionTag;
        transform.position = spawnPoint.position;
        stopIndex = 0;
        move = true; // déplace jusqu'au premier stoppoint
    }

    



    void Update() {
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        if (stopIndex == 0) {
            if (transform.position.x < stopMoving[stopIndex].position.x && move) {
                move = false;
                StartCoroutine(InvocationAnimation());
            }
        } else if (transform.position.x > stopMoving[stopIndex].position.x && move) {
            move = false;
        }
        

        if (npcf.spawnpointsIndex == npcf.spawnpoints.Length - 1 && !ended) {
            Collider2D[] tab = new Collider2D[10];

            int index = colliderInteract.GetComponent<Collider2D>().OverlapCollider(new ContactFilter2D(), tab);
            for (int i = 0; i < index; i++) {
                if (tab[i].CompareTag("Player")) {
                    StartCoroutine(ExitCave());
                    break;
                }
            }
        }

    }

    IEnumerator InvocationAnimation() {
        yield return new WaitForSeconds(1f);
        anim.SetBool("special", true);
        FindObjectOfType<Seal>().Explode();
        yield return new WaitForSeconds(4f);
        anim.SetBool("special", false);
        yield return new WaitForSeconds(1.7f);
        npcd.dialog = introductionTag;
        npcd.StartInteraction();
        StartCoroutine(GetStopInteraction());
    }

    IEnumerator ExitCave() {
        ended = true;
        npcd.dialog = exitTag;
        npcd.StartInteraction();

        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        while (!pm.CanInteract()) {
            yield return 0;
        }

        int id = pm.AddBlockAction(new bool[5] { true, true, true, false, true }); // dont block interaction so cam can zoom out
        sr.flipX = !sr.flipX;
        yield return new WaitForSeconds(0.2f);
        stopIndex++;
        move = true;
        while (move) {
            yield return 0;
        }
        pm.RemoveBlockAction(id);
        transform.position = endPoint.position;
    }

 

    IEnumerator GetStopInteraction() {
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        while (!pm.CanInteract()) {
            yield return 0;
        }

        int id = pm.AddBlockAction(new bool[5] { true, true, true, false, true }); // dont block interaction so cam can zoom out
        sr.flipX = !sr.flipX;
        yield return new WaitForSeconds(0.8f);
        stopIndex++;
        moveSpeed *= -1.5f;
        move = true;
        while (move) {
            yield return 0;
        }
        sr.flipX = !sr.flipX;

        npcf.isFollowing = true;
        npcf.UpdatePos();

        pm.RemoveBlockAction(id);
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