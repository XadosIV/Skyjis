using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpellData data;

    int damage;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        data = GetComponent<SpellData>();
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();

        damage = pm.CalculateDamage(data.damage);

        if (pm.direction == -1) {
            Flip();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(data.direction * data.speed * Time.fixedDeltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (data.enemyLayers == collider.gameObject.layer) {
            collider.GetComponentInParent<Enemy>().TakeDamage(damage, data.Knockback(data.direction));
        }

        Destroy(gameObject);
    }

    private void Flip() {
        data.direction *= -1;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.flipX = !sr.flipX;
    }
}