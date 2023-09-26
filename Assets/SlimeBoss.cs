using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBoss : MonoBehaviour {

    /*
     100% - 70% vie : charge
     70% - 40% vie : flip / rebond
     40% - 0% vie : bonus slime qui spawn
     */

    public float moveSpeed;
    private float initMoveSpeed;
    private Rigidbody2D rb;
    private Enemy data;
    private Animator anim;
    private SpriteRenderer sr;

    private int nbCollisions = 0;
    private bool jumping;
    private float maxHp;
    private bool attacking;

    private Vector3 reference = Vector3.zero;

    private int onTop = 0;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        data = GetComponent<Enemy>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        maxHp = data.hp;
        initMoveSpeed = moveSpeed;
    }

    private int GetPhase() {
        float percentHealth = (data.hp / maxHp) * 100;
        float percentAbsorbed = (data.absorbedCount / data.requireAbsorb) * 100;
        float percent = Mathf.Min(percentHealth, percentAbsorbed);

        if (percent > 70) return 1;
        if (percent > 40) return 2;
        if (percent > 0) return 3;
        return 0;
    }

    private void Update() {
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
    }

    void FixedUpdate() {
        /*if (data.isStun) return;
        if (data.health <= 0) {
            return;
        }*/

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

    private void Call(string attack) {
        switch (attack) {
            case "charge":
                break;
            case "bounce":
                break;
            case "spawn":
                break;
        }
    }

    private void Attack() {
        int phase = GetPhase();
        switch (phase) {
            case 0:
                return;
            case 1:
                StartCoroutine(Jump());
                break;
            case 2:
                return;
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
        moveSpeed = 0;
        anim.SetTrigger("jump");
        nbCollisions = 0;
        while (nbCollisions != 3) {
            yield return 0;
        }
        moveSpeed = 0;
        anim.SetTrigger("jump");
        attacking = false;
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