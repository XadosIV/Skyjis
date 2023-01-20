using UnityEngine;

public class LeftRightEnemy : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody2D rb;
    private Enemy data;

    private Vector3 reference = Vector3.zero;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        data = GetComponent<Enemy>();
    }

    void FixedUpdate() {
        if (data.isStun) return;
        if (data.health <= 0) {
            return;
        }

        float speed;
        if (IsFacingRight()) {
            speed = moveSpeed*Time.fixedDeltaTime;
        } else {
            speed = -moveSpeed*Time.fixedDeltaTime;
        }

        rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector2(speed, rb.velocity.y), ref reference, .05f);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Map")) {
            transform.localScale = new Vector2(-(Mathf.Sign(rb.velocity.x)) * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private bool IsFacingRight() {
        return transform.localScale.x > Mathf.Epsilon;
    }
}