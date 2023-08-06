using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] private float flySpeed;

    [SerializeField] private Transform detectionCenter;
    [SerializeField] private float detectionRadius;
    private bool playerInRange;
    [SerializeField] private LayerMask playerLayer;
    private Transform player;


    private Enemy data;
    private Rigidbody2D rb;

    private Vector3 targetPoint;

    private Vector3 velocity = Vector3.zero;

    private int direction = 1;
    private SpriteRenderer sr;

    void Start()
    {
        data = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<PlayerMovement>().transform;
    }

    void Update()
    {
        if (data.purified) return;
        if (data.hp <= 0) {
            enabled = false;
        } else {
            playerInRange = Physics2D.OverlapCircle(detectionCenter.position, detectionRadius, playerLayer);
        }
    }

    private void FixedUpdate() {
        if (data.isStun) return;
        if (data.purified) return;
        if (!playerInRange) return;

        Vector3 forward = player.position - transform.position;

        if (Mathf.Sign(forward.x) != direction) {
            Flip();
        }

        rb.velocity = flySpeed * Time.fixedDeltaTime * forward.normalized;
    }

    private void Flip() {
        sr.flipX = !sr.flipX;
        direction = -direction;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionCenter.position, detectionRadius);
    }
}
