using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int damage;
    [SerializeField] protected float stunTime;
    [SerializeField] protected float flashDelay;
    [SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer sr;
    [SerializeField] protected Rigidbody2D rb;
    public int health;
    protected bool isStun;
    private bool isBlinking;
    private Coroutine stunningRoutine;
    public float moveSpeed;

    // Start is called before the first frame update
    protected void Start()
    {
        health = maxHealth;
        animator.SetBool("Alive", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && health > 0) {
            collision.GetComponent<PlayerMovement>().TakeDamage(damage);
        }
    }

    protected void FixedUpdate() {
        if (health <= 0) {
            rb.velocity = Vector3.zero;
        }
    }

    public void TakeDamage(int damage, Vector2 knockback) {
        if (health <= 0) return;

        animator.SetTrigger("Hurt");
        health -= damage;
        if (health <= 0) {
            animator.SetBool("Alive", false);
            rb.velocity = Vector3.zero;
            this.enabled = false;
        }
        else {
            if (isStun) {
                StopCoroutine(stunningRoutine);
            }
            rb.velocity = knockback;
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
