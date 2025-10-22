using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBoss : MonoBehaviour {

    public float moveSpeed;
    private float initMoveSpeed;
    private Rigidbody2D rb;
    private Enemy data;
    private Animator anim;
    private SpriteRenderer sr;

    private int nbCollisions = 0;
    private bool jumping;
    private bool attacking;

    private Vector3 reference = Vector3.zero;

    private int onTop = 0;

    [SerializeField] private GameObject[] enemiesSpawnable;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        data = GetComponent<Enemy>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        initMoveSpeed = moveSpeed;
    }

    private void Update() {
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
    }

    void FixedUpdate() {
        if (data.purified) return;
        if (data.isStun) return;
        if (data.hp <= 0) {
            return;
        }

        float speed;
        if (IsFacingRight()) {
            speed = moveSpeed * Time.fixedDeltaTime;
        }
        else {
            speed = -moveSpeed * Time.fixedDeltaTime;
        }

        rb.velocity = Vector3.SmoothDamp(rb.velocity, new Vector2(speed, rb.velocity.y), ref reference, .05f);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Map")) {
            if (!jumping) transform.localScale = new Vector2(-(Mathf.Sign(rb.velocity.x)) * Mathf.Abs(transform.localScale.x), transform.localScale.y);
            nbCollisions++;
            if (!attacking) Attack();
        }
    }

    private void Attack() {
        int attack = Random.Range(0,3);

        if (attack == 0)
        {
            StartCoroutine(Jump());
        }else if (attack == 1)
        {
            StartCoroutine(Charge());
        }else if (attack == 2)
        {
            StartCoroutine(Spawn());
        }
    }

    IEnumerator Charge() {
        anim.SetBool("charge", true);
        attacking = true;
        moveSpeed *= 1.5f;
        nbCollisions = 0;
        while (nbCollisions != 3) {
            yield return 0;
        }
        anim.SetBool("charge", false);
        attacking = false;
        moveSpeed = initMoveSpeed;
    }

    IEnumerator Jump() {
        attacking = true;
        anim.SetTrigger("jump");
        yield return new WaitForSeconds(.5f);
        moveSpeed = 0;
        nbCollisions = 0;
        yield return new WaitForSeconds(3f);
        while (nbCollisions != 3) {
            if (Random.Range(0,500) == 1)
            {
                break;
            }
            yield return 0;
        }
        anim.SetTrigger("jump");
        yield return new WaitForSeconds(.5f);
        moveSpeed = 0;
        attacking = false;
    }

    IEnumerator Spawn()
    {
        attacking = true;
        yield return new WaitForSeconds(1.5f);
        moveSpeed = 0;
        yield return new WaitForSeconds(0.2f);
        anim.SetTrigger("spawn");
        yield return new WaitForSeconds(1.5f);
        moveSpeed = initMoveSpeed;
        attacking = false;
    }

    void CreateEnemies()
    {

        int indexEnemyOne = Random.Range(0, enemiesSpawnable.Length -1);
        int indexEnemyTwo = Random.Range(0, enemiesSpawnable.Length -1);

        // elite enemy = enemies.Spawnable.Length -1 -> Bigger, harder, 10% spawn
        if (Random.Range(0,10) == 1)
        {
            indexEnemyOne = enemiesSpawnable.Length - 1;
        }
        if (Random.Range(0, 10) == 1)
        {
            indexEnemyTwo = enemiesSpawnable.Length - 1;
        }

        GameObject one = Instantiate(enemiesSpawnable[indexEnemyOne], transform.position, transform.rotation);
        one.GetComponent<Rigidbody2D>().AddForce(new Vector2(-4, Random.Range(0, 6)), ForceMode2D.Impulse);

        GameObject two = Instantiate(enemiesSpawnable[indexEnemyTwo], transform.position, transform.rotation);
        two.GetComponent<Rigidbody2D>().AddForce(new Vector2(4, Random.Range(0, 6)), ForceMode2D.Impulse);

    }

    IEnumerator FlipY() {
        jumping = true;
        int sign = onTop == 0 ? -1 : 1;
        yield return new WaitForSeconds(0.2f);
        sr.flipY = !sr.flipY;
        rb.gravityScale = sign * 30;
        yield return new WaitForSeconds(0.3f);
        rb.gravityScale = sign * 1;
        moveSpeed = initMoveSpeed;
        onTop = (onTop + 1) % 2;
        jumping = false;
    }

    private bool IsFacingRight() {
        return transform.localScale.x > Mathf.Epsilon;
    }
}