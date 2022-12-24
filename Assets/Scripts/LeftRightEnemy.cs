using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftRightEnemy : Enemy
{
    
    public float sideCollisionRadius;
    public float groundCollisionRadius;

    private float direction;

    [SerializeField] private Transform sideCollisionLeft;
    [SerializeField] private Transform sideCollisionRight;
    [SerializeField] private Transform groundCollisionLeft;
    [SerializeField] private Transform groundCollisionRight;
    [SerializeField] private LayerMask collisionLayers;
    private Vector3 velocity = Vector3.zero;

    new void Start() {
        base.Start();
        direction = Random.Range(0, 2) == 0 ? 1 : -1;
        if (direction == 1) {
            direction *= -1;
            Flip();
        }
    }

    void Update()
    {
        
    }

    new void FixedUpdate() {
        if (isStun) return;
        if (health <= 0) {
            base.FixedUpdate();
            return;
        }
        bool groundCollide;
        bool sideCollide;
        if (direction == 1) {
            sideCollide = Physics2D.OverlapCircle(sideCollisionRight.position, sideCollisionRadius, collisionLayers);
            groundCollide = Physics2D.OverlapCircle(groundCollisionRight.position, groundCollisionRadius, collisionLayers);
        }
        else {
            sideCollide = Physics2D.OverlapCircle(sideCollisionLeft.position, sideCollisionRadius, collisionLayers);
            groundCollide = Physics2D.OverlapCircle(groundCollisionLeft.position, groundCollisionRadius, collisionLayers);
        }


        if (sideCollide || !groundCollide) {
            Flip();
        }

        float horizontalMovement = direction * moveSpeed * Time.fixedDeltaTime;
        Vector3 targetVelocity = new Vector2(horizontalMovement, rb.velocity.y);

        

        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, .05f);
    }

    private void Flip() {
        sr.flipX = !sr.flipX;
        direction *= -1;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(sideCollisionLeft.position, sideCollisionRadius);
        Gizmos.DrawWireSphere(sideCollisionRight.position, sideCollisionRadius);
        Gizmos.DrawWireSphere(groundCollisionLeft.position, groundCollisionRadius);
        Gizmos.DrawWireSphere(groundCollisionRight.position, groundCollisionRadius);
    }

}
