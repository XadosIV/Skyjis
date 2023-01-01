using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public int value;
    private GameManagerScript gm;
    private bool collected = false;
    private Animator animator;
    void Start()
    {
        gm = FindObjectOfType<GameManagerScript>();
        animator = GetComponent<Animator>();
        animator.SetInteger("Value", value);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(Random.Range(-5, 5), Random.Range(-5, 5)), ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player") && !collected) {
            collected = true;
            gm.coinsCount += value;
            gm.UpdateUI();
            Destroy(gameObject);
        }
    }
}
