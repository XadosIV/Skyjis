using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int direction = 1;
    private Rigidbody2D rb;
    private SpellData data;
    private readonly int enemyLayers = 8;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        data = GetComponent<SpellData>();
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();

        if (pm.direction == -1) {
            Flip();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(direction * data.speed * Time.fixedDeltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (enemyLayers == collider.gameObject.layer) {
            collider.GetComponentInParent<Enemy>().TakeDamage(data.damage, new Vector2(data.knockback *direction, 0));
        }

        Destroy(gameObject);
    }

    private void Flip() {
        direction *= -1;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.flipX = !sr.flipX;
    }
}
