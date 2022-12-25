using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftRightJumpEnemy : Enemy
{

    public float jumpForce;
    public float sideCollisionRadius;
    public float groundCollisionRadius;
    public float detectionRadius;
    public float groundCheckRadius;

    private float direction;

    [SerializeField] private Transform detectionCenter;
    [SerializeField] private Transform sideCollisionLeft;
    [SerializeField] private Transform sideCollisionRight;
    [SerializeField] private Transform groundCollisionLeft;
    [SerializeField] private Transform groundCollisionRight;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private LayerMask playerLayer;
    private Vector3 velocity = Vector3.zero;

    private Transform player;
    private bool detected = false;
    private bool groundCollide;
    private bool sideCollide;
    private bool isJumping;
    private bool isGrounded;

    new void Start() {
        base.Start();
        direction = Random.Range(0, 2) == 0 ? 1 : -1;
        if (direction == 1) {
            direction *= -1;
            Flip();
        }
        player = FindObjectOfType<PlayerMovement>().transform;
    }

    void Update() {
        if (!detected) {
            if (sideCollide || !groundCollide) {
                Flip();
            }
        } else {
            Vector3 toPlayer = player.position - transform.position;
            if ((toPlayer.x > 0 && direction == -1) || (toPlayer.x < 0 && direction == 1)) {
                Flip();
            }


            if (isGrounded) {
                if (sideCollide || groundCollide) {
                    isJumping = true;
                }
            }
        }
    }
        new void FixedUpdate() {
        if (isStun) return;
        if (health <= 0) {
            base.FixedUpdate();
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, collisionLayers);

        detected = Physics2D.OverlapCircle(detectionCenter.position, detectionRadius, playerLayer);

        if (direction == 1) {
            sideCollide = Physics2D.OverlapCircle(sideCollisionRight.position, sideCollisionRadius, collisionLayers);
            groundCollide = Physics2D.OverlapCircle(groundCollisionRight.position, groundCollisionRadius, collisionLayers);
        }
        else {
            sideCollide = Physics2D.OverlapCircle(sideCollisionLeft.position, sideCollisionRadius, collisionLayers);
            groundCollide = Physics2D.OverlapCircle(groundCollisionLeft.position, groundCollisionRadius, collisionLayers);
        }
        

        float horizontalMovement = direction * moveSpeed * Time.fixedDeltaTime;
        Vector3 targetVelocity = new Vector2(horizontalMovement, rb.velocity.y);

        

        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);

        if (isJumping) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
            isJumping = false;
        }
    }

    private void Flip() {
        sr.flipX = !sr.flipX;
        direction *= -1;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionCenter.position, detectionRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}
