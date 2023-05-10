using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mila_Exile_Cave : MonoBehaviour {
    public Transform[] spawnpoints;
    public Transform stopMoving;

    public int moveSpeed;

    bool move = true;

    Vector3 velocity = Vector3.zero;

    private Animator anim;
    private Rigidbody2D rb;
    void Start() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        transform.position = spawnpoints[0].position;
    }

    void Update() {
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        if (transform.position.x < stopMoving.position.x && move) {
            move = false;
            StartCoroutine(InvocationAnimation());
        }
    }

    IEnumerator InvocationAnimation() {
        yield return new WaitForSeconds(1f);
        anim.SetBool("special", true);
        FindObjectOfType<Seal>().Explode();
        yield return new WaitForSeconds(4f);
        anim.SetBool("special", false);
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