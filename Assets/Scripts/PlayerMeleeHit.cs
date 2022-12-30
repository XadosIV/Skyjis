using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeHit : MonoBehaviour
{
    private GameManagerScript playerData;
    private PlayerMovement pm;
    private Animator animator;
    private Rigidbody2D rb;
    public Transform attackPointLeft;
    public Transform attackPointRight;
    public LayerMask enemyLayers;

    private bool canHit = true;
    void Start()
    {
        animator = GetComponent<Animator>();
        playerData = FindObjectOfType<GameManagerScript>();
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    public bool IsAvailable() {
        return canHit;
    }

    public void Execute() {
        animator.SetTrigger("MeleeHit");
        StartCoroutine(HandleMeleeHit());
    }

    // Call durant l'animation de melee hit
    private void MeleeHit() {
        Transform attackPoint = pm.direction == 1 ? attackPointRight : attackPointLeft;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, playerData.attackRange, enemyLayers);
        bool somethingHit = false;
        foreach (Collider2D collider in hitColliders) {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy.health > 0) {
                enemy.TakeDamage(playerData.attackDamage, new Vector2(playerData.knockback * pm.direction, 0));
                somethingHit = true;
            }
        }
        if (somethingHit) {
            rb.AddForce(new Vector2(playerData.knockback * pm.direction * -1 * playerData.selfKnockback, 0));
        }
    }

    //Cooldown
    private IEnumerator HandleMeleeHit() {
        canHit = false;
        yield return new WaitForSeconds(playerData.attackSpeed);
        canHit = true;
    }


}
