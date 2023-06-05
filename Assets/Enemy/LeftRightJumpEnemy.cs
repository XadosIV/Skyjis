using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftRightJumpEnemy : MonoBehaviour
{
    public bool changeDirectionJumping;
    public float moveSpeed;
    public float jumpForce;
    [SerializeField] private Transform detectionCenter;
    public float detectionRadius;
    [SerializeField] private Transform groundCheck;
    public float groundCheckRadius;
    private Transform player;
    private bool detected = false;
    private Rigidbody2D rb;
    private Enemy data;
    private float lastSpeed;

    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private LayerMask playerLayer;

    private Vector3 reference = Vector3.zero;

    private void Start() {
        player = FindObjectOfType<PlayerMovement>().transform;
        rb = GetComponent<Rigidbody2D>();
        data = GetComponent<Enemy>();
    }

    void FixedUpdate() {
        /*if (data.isStun) return;
        if (data.health <= 0) {
            return;
        }*/

        float speed;

        detected = Physics2D.OverlapCircle(detectionCenter.position, detectionRadius, playerLayer);
        bool onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayer);

        if (!detected) {
            if (IsFacingRight()) {
                speed = moveSpeed * Time.fixedDeltaTime;
            }
            else {
                speed = -moveSpeed * Time.fixedDeltaTime;
            }
        } else {
            Vector3 toPlayer = player.position - transform.position;
            if (changeDirectionJumping || onGround) {
                if (toPlayer.x > 0) { //faut aller à droite pour aller vers le joueur
                    speed = moveSpeed * Time.fixedDeltaTime;
                    transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
                }
                else {
                    speed = -moveSpeed * Time.fixedDeltaTime;
                    transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                }
                lastSpeed = speed;
            }
            else {
                speed = moveSpeed * Time.fixedDeltaTime * Mathf.Sign(lastSpeed);
            }
        }


        rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector2(speed, rb.velocity.y), ref reference, .05f);
        
        if (onGround && detected) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce* Time.fixedDeltaTime);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Map") && !detected) {
            transform.localScale = new Vector2(-(Mathf.Sign(rb.velocity.x)) * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private bool IsFacingRight() {
        return transform.localScale.x > Mathf.Epsilon;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionCenter.position, detectionRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
