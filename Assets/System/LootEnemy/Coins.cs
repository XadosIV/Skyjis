using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public int value;
    private GameManager gm;
    private bool collected = false;
    private Animator animator;
    public Collider2D hitbox;
    private Rigidbody2D rb;
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
        animator.SetInteger("Value", value);

        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(Random.Range(-2, 2), Random.Range(0, 6)), ForceMode2D.Impulse);
        StartCoroutine(HandleHitbox());
    }

    IEnumerator HandleHitbox() {
        hitbox.enabled = false;
        yield return new WaitForSeconds(.5f);
        hitbox.enabled = true;
        yield return new WaitForSeconds(.3f);
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player") && !collected) {
            collected = true;
            gm.Coins += value;
            Destroy(gameObject);
        }
    }
}
