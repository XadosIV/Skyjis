using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakwoodSpider : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    private Enemy data;
    private Animator anim;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private BoxCollider2D detectorHitbox;

    [SerializeField] private Transform detectionCenter;
    [SerializeField] private float detectionRadius;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;

    private Transform player;
    private bool playerInRange;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask collisionLayer;

    private bool onGround;

    private Vector3 velocity = Vector3.zero;
    void Start() {
        data = GetComponent<Enemy>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        player = FindObjectOfType<PlayerMovement>().transform;

        // Find detector
        BoxCollider2D[] components = GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D c in components) {
            if (c.gameObject.name == "Detector" || c.gameObject.name == "detector") {
                detectorHitbox = c;
                break;
            }
        }
    }

    void Update() {
        if (data.hp <= 0) {
            anim.SetTrigger("death");
            enabled = false;
        } else {
            playerInRange = Physics2D.OverlapCircle(detectionCenter.position, detectionRadius, playerLayer);

            if (Mathf.Abs(rb.velocity.x) < 0.1f) Flip();
        }
    }

    private void FixedUpdate() {
        if (data.isStun) return;
        if (playerInRange) {
            

            onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayer);
            if (onGround) {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);

                Vector3 toPlayer = player.position - transform.position;
                if (Mathf.Sign(toPlayer.x) != Mathf.Sign(speed)) { // Si on ne va pas dans la bonne direction
                    Flip();
                }
            }
        }
        rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector2(speed * Time.fixedDeltaTime, rb.velocity.y), ref velocity, .05f);

    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Map") && !playerInRange) {
            Flip();
        }
    }

    private void Flip() {
        sr.flipX = !sr.flipX;
        speed *= -1;
        detectorHitbox.offset = new Vector2(-detectorHitbox.offset.x, detectorHitbox.offset.y);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionCenter.position, detectionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
