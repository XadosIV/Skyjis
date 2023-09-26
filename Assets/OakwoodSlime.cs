using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakwoodSlime : MonoBehaviour
{
    [SerializeField] private float speed;

    private Enemy data;
    private Animator anim;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private BoxCollider2D detectorHitbox;


    private Vector3 velocity = Vector3.zero;
    void Start()
    {
        data = GetComponent<Enemy>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Find detector
        BoxCollider2D[] components = GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D c in components) {
            if (c.gameObject.name == "Detector" || c.gameObject.name == "detector") {
                detectorHitbox = c;
                break;
            }
        }
    }

    void Update()
    {
        if (data.purified) {
            rb.gravityScale = 3;
        }
    }

    private void FixedUpdate() {
        if (data.purified) return;
        if (data.isStun) return;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector2(speed*Time.fixedDeltaTime, rb.velocity.y), ref velocity, .05f);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Map")) {
            Flip();
        }
    }

    private void Flip() {
        sr.flipX = !sr.flipX;
        speed *= -1;
        detectorHitbox.offset = new Vector2( - detectorHitbox.offset.x, detectorHitbox.offset.y);
    }

    void Jump() {
        rb.AddForce(new Vector2(0, 5f), ForceMode2D.Impulse);
    }
}
