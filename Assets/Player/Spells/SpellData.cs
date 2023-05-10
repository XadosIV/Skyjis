using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpellData : MonoBehaviour
{
    PlayerMovement pm;
    Rigidbody2D rb;
    GameManager gm;

    public int id = -1;
    public string spellName;
    [SerializeField] float damage;
    [SerializeField] Vector2 knockback;
    [SerializeField] float speed;
    public float cooldown;
    public int manaCost;
    [System.NonSerialized] public int direction = 1;

    [SerializeField] Vector3 offsetTexture;

    [SerializeField] Vector3 dim;
    [SerializeField] Vector3 offset;

    int blockActionId;
    public bool displayOverGround;
    public bool hasDirection;
    public bool blockPlayer;
    public bool canCastMidAir;
    public bool holding;
    public bool positionToPlayer;
    public bool followPlayer;
    public bool forceIdle = false;

    public readonly int enemyLayers = 8; // check l'editeur, c'est le layer numero 8

    public Vector2 Knockback(int _direction) {
        return new Vector2(knockback.x * _direction, knockback.y);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        gm = FindObjectOfType<GameManager>();
        pm = FindObjectOfType<PlayerMovement>();

        if (displayOverGround) { GetComponent<SpriteRenderer>().sortingOrder = 1000; }
        if (positionToPlayer) {transform.position = pm.transform.position + offsetTexture;}

        if (hasDirection) {
            if (pm.direction != direction) { Flip(); }
        } else {
            direction = 0;
        }
        if (blockPlayer) { blockActionId = pm.StartCinematic(forceIdle); }

        if (holding) { StartCoroutine(Holding()); }

        damage = pm.CalculateDamage(damage);
    }

    void Update()
    {
        if (followPlayer) {
            transform.position = pm.transform.position + offsetTexture;
        }
    }

    void FixedUpdate() {
        if (rb) {
            rb.velocity = new Vector2(direction * speed * Time.fixedDeltaTime, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (enemyLayers == collider.gameObject.layer) {
            collider.GetComponentInParent<Enemy>().TakeDamage((int)damage, Knockback(direction));
        }

        Destroy(gameObject);
    }

    void Damage() {
        Vector2 pos = transform.position + offset;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, dim, 0f);
        foreach (Collider2D collider in colliders) {
            if (enemyLayers == collider.gameObject.layer) {
                if (!hasDirection) {
                    direction = (int)Mathf.Sign(collider.gameObject.transform.position.x - transform.position.x);
                }

                collider.GetComponentInParent<Enemy>().TakeDamage((int)damage, Knockback(direction));
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Vector2 pos = transform.position + offset;

        Gizmos.DrawWireCube(pos, dim);
    }


    void Flip() {
        direction *= -1;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.flipX = !sr.flipX;
        offset.x *= -1;

        Light2D[] lights = GetComponentsInChildren<Light2D>();
        foreach (Light2D l in lights) {
            l.transform.RotateAround(transform.position, transform.up, 180f);
        }
    }

    void FreePlayer() {
        if (blockActionId != -1) {
            pm.ExitCinematic(blockActionId);
            blockActionId = -1;
        }
    }

    void EndAnimation() {
        FreePlayer();
        Destroy(gameObject);
    }

    IEnumerator Holding() {
        string holdingButton = "Spell";
        GameData save = gm.save;
        int i = 0;
        while (i < 3) {
            if (save.spellIndex[i] == id) {
                holdingButton += i + 1;
            }
            i++;
        }

        while (Input.GetButton(holdingButton) && gm.Mana >= manaCost) {
            gm.Mana -= manaCost;
            Damage();
            yield return new WaitForSeconds(.2f);
        }

        FreePlayer();
        Destroy(gameObject);
    }
}
