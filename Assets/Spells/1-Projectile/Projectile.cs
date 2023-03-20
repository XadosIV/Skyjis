using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpellData data;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        data = GetComponent<SpellData>();
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();

        if (pm.direction == -1) {
            Flip();
        }
        Debug.Log(data.direction);
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(data.direction * data.speed * Time.fixedDeltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (data.enemyLayers == collider.gameObject.layer) {
            collider.GetComponentInParent<Enemy>().TakeDamage(data.damage, new Vector2(data.knockback * data.direction, 0));
        }

        Destroy(gameObject);
    }

    private void Flip() {
        data.direction *= -1;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.flipX = !sr.flipX;
    }
}