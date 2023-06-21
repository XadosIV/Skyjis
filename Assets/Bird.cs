using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] private float flySpeed;

    [SerializeField] private LayerMask collisionLayer;
    private Collider2D physicsBox;
    private Vector2 min;
    private Vector2 max;

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

        physicsBox = GetComponent<BoxCollider2D>();

        BoxCollider2D[] colliders = GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D collider in colliders) {
            if (collider.name == "PatrolArea") {
                min = collider.bounds.min;
                max = collider.bounds.max;
                collider.gameObject.SetActive(false);
                break;
            }
        }
        targetPoint = GetRandomPoint();
    }

    Vector3 GetRandomPoint() {
        Vector2 point;
        do {
            point = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
        } while (Physics2D.OverlapBox(point, physicsBox.bounds.size, 0, collisionLayer));



        return new Vector3(point.x, point.y, transform.position.z);
    }

    void Update()
    {
        Vector2 toTarget = targetPoint - transform.position;
        if (toTarget.magnitude < 0.1f || rb.velocity.magnitude < 0.1f ) {
            targetPoint = GetRandomPoint();
        } else {
            if (Mathf.Sign(toTarget.x) != direction) {
                Flip();
            }
        }
    }

    private void FixedUpdate() {
        if (data.isStun) return;

        Vector3 forward = targetPoint - transform.position;

        rb.velocity = flySpeed * Time.fixedDeltaTime * forward.normalized;
    }

    private void Flip() {
        sr.flipX = !sr.flipX;
        direction = -direction;
    }
}
