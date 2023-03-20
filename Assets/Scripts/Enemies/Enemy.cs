using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public int enemyId; //Convention : {zoneId(1)}_{MapNumber(2)}_(UniqueIdentifier(2))} => 10101 => Oakwood, map : oakwood_01, 01er ennemi
    public string bossName;
    public bool isBoss;
    public int maxHealth;
    [NonSerialized] public int health;
    public int damage;
    public Vector2 manaGain;
    public Vector2 manaGainOnHit;
    public float stunTime;
    public Vector2 coinsLoot;

    private bool isDead = false;

    private readonly float flashDelay = 0.15f;
    private Animator animator;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    [NonSerialized] public bool isStun;
    private bool isBlinking;


    
    [SerializeField] private Collider2D hitbox;
    private Coroutine stunningRoutine;
    private GameManager gm;
    private UserInterfaceManager ui;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        health = maxHealth;
        gm = FindObjectOfType<GameManager>();
        if (isBoss) ui = FindObjectOfType<UserInterfaceManager>();
        animator.SetBool("Alive", true);

        if (gm.killedEnnemies.Contains(enemyId)) Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collider) {
        if (collider.CompareTag("Player") && health > 0) {
            collider.GetComponent<PlayerMovement>().TakeDamage(damage);
        }
    }

    private void FixedUpdate() {
        if (health <= 0) {
            rb.velocity = Vector3.zero;
        }
    }

    private void Death() {
        isDead = true;
        int moneyToLoot = (int)UnityEngine.Random.Range(coinsLoot.x, coinsLoot.y);
        List<GameObject> coins = gm.MoneyToCoin(moneyToLoot);
        foreach (GameObject coin in coins) {
            Instantiate(coin, transform.position, transform.rotation);
        }

        if (!isBoss) gm.killedEnnemies.Add(enemyId);

        int manaAmount = (int)UnityEngine.Random.Range(manaGain.x, manaGain.y);
        gm.SpawnManaBall(manaAmount, transform);


        animator.SetBool("Alive", false);
        rb.velocity = Vector3.zero;
        hitbox.enabled = false;
        this.enabled = false;
    }

    public void TakeDamage(int damage, Vector2 knockback) {
        if (isDead) return;

        animator.SetTrigger("Hurt");
        health -= damage;
        if (isBoss) {ui.UpdateUI();}

        if (health <= 0) {
            Death();
        } else {
            if (isStun) {
                StopCoroutine(stunningRoutine);
            }

            if (isBoss) {
                int manaAmount = (int)UnityEngine.Random.Range(manaGainOnHit.x, manaGainOnHit.y);
                gm.SpawnManaBall(manaAmount, transform);
            }
            

            rb.velocity = Vector3.zero;
            rb.AddForce(knockback, ForceMode2D.Impulse);
            stunningRoutine = StartCoroutine(Stunning());
            if (!isBlinking) {
                StartCoroutine(Blinking());
            }
        }

        
        
    }
    private IEnumerator Stunning() {
        isStun = true;
        yield return new WaitForSeconds(stunTime);
        isStun = false;
    }

    private IEnumerator Blinking() {
        isBlinking = true;
        while (isStun) {
            sr.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(flashDelay);
            sr.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(flashDelay);
        }
        isBlinking = false;
    }
}
