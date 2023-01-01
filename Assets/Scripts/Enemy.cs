using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    [System.NonSerialized] public int health;
    public int damage;
    public Vector2 manaGain;
    public float stunTime;
    public Vector2 coinsLoot;

    private readonly float flashDelay = 0.15f;
    private Animator animator;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    [System.NonSerialized] public bool isStun;
    private bool isBlinking;


    
    [SerializeField] private Collider2D hitbox;
    private Coroutine stunningRoutine;

    private GameManagerScript gm;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        health = maxHealth;
        gm = FindObjectOfType<GameManagerScript>();
        animator.SetBool("Alive", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player") && health > 0) {
            collider.GetComponent<PlayerMovement>().TakeDamage(damage);
        }
    }

    protected void FixedUpdate() {
        if (health <= 0) {
            rb.velocity = Vector3.zero;
        }
    }

    private void Death() {
        int moneyToLoot = (int)Random.Range(coinsLoot.x, coinsLoot.y);
        List<GameObject> coins = gm.MoneyToCoin(moneyToLoot);
        foreach (GameObject coin in coins) {
            Instantiate(coin, transform.position, transform.rotation);
        }

        int manaAmount = (int)Random.Range(manaGain.x, manaGain.y);
        gm.SpawnManaBall(manaAmount, transform);


        animator.SetBool("Alive", false);
        rb.velocity = Vector3.zero;
        hitbox.enabled = false;
        this.enabled = false;
    }

    public void TakeDamage(int damage, Vector2 knockback) {
        if (health <= 0) return;

        animator.SetTrigger("Hurt");
        health -= damage;
        if (health <= 0) {
            Death();
        }
        else {
            if (isStun) {
                StopCoroutine(stunningRoutine);
            }
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
