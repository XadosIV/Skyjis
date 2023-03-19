using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSpell : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpellData data;
    private int direction = 1;
    private readonly int enemyLayers = 8;
    private PlayerMovement pm;
    private PlayerMeleeHit pmh;

    public float width;
    public float height;
    public float offsetX;
    public float offsetY;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        data = GetComponent<SpellData>();
        pm = FindObjectOfType<PlayerMovement>();
        pmh = FindObjectOfType<PlayerMeleeHit>();
        if (pm.direction == -1) {
            Flip();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pm.direction != direction) {
            Flip();
        }
        if (pm.direction == 1) {
            transform.position = pmh.attackPointRight.position;
        } else {
            transform.position = pmh.attackPointLeft.position;
        }
    }

    void Damage() {
        Vector2 pos = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, new Vector2(width, height), 0f);
        foreach (Collider2D collider in colliders) {
            if (enemyLayers == collider.gameObject.layer) {
                collider.GetComponentInParent<Enemy>().TakeDamage(data.damage, new Vector2(data.knockback * direction, 0));
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Vector2 pos = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);

        Gizmos.DrawWireCube(pos, new Vector2(width, height));
    }


    void Flip() {
        direction *= -1;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.flipX = !sr.flipX;
    }

    void EndAnimation() {
        Destroy(gameObject);
    }
}
