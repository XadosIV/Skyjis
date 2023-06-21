using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private int id;  // {zoneId(1)}_{MapNumber(2)}_(UniqueIdentifier(2))} => 10101 => Oakwood, map : oakwood_01, 01er ennemi
    // exclude id = -1
    [SerializeField] private int maxHp;
    public int hp;
    [SerializeField] private int onCollisionDamage;
    [SerializeField] private Vector2 manaGain;
    [SerializeField] private Vector2 manaGainOnHit;
    [SerializeField] private Vector2 coinsLoot;
    [SerializeField] private float knockResist;

    [SerializeField] private float stunTime = .4f;
    [SerializeField] private float flashDelay = .13f;
    public bool isStun;

    private Collider2D hitbox;
    private Rigidbody2D rb;
    private GameManager gm;
    private SpriteRenderer sr;

    private int RangeToInt(Vector2 range) {
        return (int)UnityEngine.Random.Range(range.x, range.y);
    }

    void Start() {
        gm = FindObjectOfType<GameManager>();

        //Check if enemy already killed
        if (gm.killedEnnemies.Contains(id)) Destroy(gameObject);

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        // Set HP
        hp = maxHp;

        // Find and define Hitbox
        BoxCollider2D[] components = GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D c in components) {
            if (c.gameObject.name == "Hitbox" || c.gameObject.name == "hitbox") {
                hitbox = c;
                break;
            }
        }

    }

    void Update() {
    }

    private void Death() {
        // Add id to ennemies killed, to not respawn if the scene reload
        gm.killedEnnemies.Add(id);

        gm.SpawnCoins(RangeToInt(coinsLoot), transform.position, transform.rotation);
        gm.SpawnManaBall(RangeToInt(manaGain), transform);


        // Corpse would move without friction so we make sure there is friction at death
        rb.velocity = Vector3.zero;
        rb.bodyType = RigidbodyType2D.Dynamic; 
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts) {
            if (script.name != name) {
                script.enabled = false;
            }
        }
        hitbox.enabled = false;
        rb.sharedMaterial = null;
        enabled = false;
    }

    private void OnTriggerStay2D(Collider2D collider) {
        if (collider.CompareTag("Player") && hp > 0) {
            collider.GetComponent<PlayerMovement>().TakeDamage(onCollisionDamage);
        }
    }
    public bool TakeDamage(int damage, Vector2 knockback) {
        if (hp <= 0) return false;
        if (isStun) return false;
        
        hp -= damage;
        if (hp <= 0) {
            Death();
        } else {
            StartCoroutine(Stunning());
            gm.SpawnManaBall(RangeToInt(manaGainOnHit), transform);

            rb.AddForce(knockback / knockResist, ForceMode2D.Impulse);
        }

        return true;
    }

    

    private IEnumerator Stunning() {
        isStun = true;
        StartCoroutine(Blinking());
        yield return new WaitForSeconds(stunTime);
        isStun = false;
    }

    private IEnumerator Blinking() {
        while (isStun) {
            sr.color = new Color(1f, 1f, 1f, 0f);
            yield return new WaitForSeconds(flashDelay);
            sr.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(flashDelay);
        }
    }
}
