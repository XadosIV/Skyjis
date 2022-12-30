using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public float knockback;
    public float speed;
    public int direction;
    public float cooldown;
    private Rigidbody2D rb;

    private int enemyLayers = 8;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        PlayerMovement pm = FindObjectOfType<PlayerMovement>();

        if (pm.direction == -1) {
            Flip();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(direction * speed * Time.fixedDeltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (enemyLayers == collider.gameObject.layer) {
            collider.GetComponentInParent<Enemy>().TakeDamage(damage, new Vector2(knockback*direction, 0));
        }

        Destroy(gameObject);
    }

    private void Flip() {
        direction *= -1;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.flipX = !sr.flipX;
    }
}
