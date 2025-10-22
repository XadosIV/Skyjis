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
    [SerializeField] private float disarmedTime = 3f;
    [SerializeField] private float flashDelay = .13f;
    [SerializeField] private float absorbTime = 2f;
    public bool isStun;

    public bool purified;
    public bool manaGiven;
    public bool canAbsorb;
    public bool absorbed;
    public int absorbedCount;
    public int requireAbsorb = 1;

    private Collider2D hitbox;
    private Rigidbody2D rb;
    private GameManager gm;
    private SpriteRenderer sr;
    private Animator anim;
    private ParticleSystem particle;
    private ParticleSystemRenderer psr;
    private ParticleSystem.EmissionModule pem;
    private ParticleSystem.ExternalForcesModule pefm;
    private ParticleSystem.CollisionModule pcm;


    [SerializeField] private Material manaParticle;
    [SerializeField] private Material purifiedParticle;

    private int RangeToInt(Vector2 range) {
        return (int)UnityEngine.Random.Range(range.x, range.y);
    }

    void Start() {
        gm = FindObjectOfType<GameManager>();

        //Check if enemy already killed
        if (gm.killedEnnemies.Contains(id) && id != -1) Destroy(gameObject);

        //Check purified
        if (gm.purifiedEnnemies.Contains(id)) purified = true;
        if (gm.ennemiesManaGiven.Contains(id)) manaGiven = true;

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        particle = GetComponentInChildren<ParticleSystem>();
        pem = particle.emission;
        pefm = particle.externalForces;
        pcm = particle.collision;
        psr = GetComponentInChildren<ParticleSystemRenderer>();

        UpdateParticleType();


        // Set HP
        hp = maxHp;

        // Set absorbed
        absorbedCount = requireAbsorb;

        // Find and define Hitbox
        BoxCollider2D[] components = GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D c in components) {
            if (c.gameObject.name == "Hitbox" || c.gameObject.name == "hitbox") {
                hitbox = c;
                break;
            }
        }

    }

    void UpdateParticleType() {
        if (purified) {
            psr.material = purifiedParticle;
            pem.rateOverTime = 10;
            particle.Play();
            pefm.enabled = false;
            pcm.enabled = false;
        }
        else {
            psr.material = manaParticle;
            pem.rateOverTime = 30;
            particle.Stop();
            pefm.enabled = true;
            pcm.enabled = true;
        }
    }

    void Update() {
    }

    public void Death() {
        // Add id to ennemies killed, to not respawn if the scene reload
        gm.killedEnnemies.Add(id);

        gm.SpawnCoins(RangeToInt(coinsLoot), transform.position, transform.rotation);
        gm.SpawnManaBall(RangeToInt(manaGain), transform);

        if (anim) anim.SetTrigger("death");

        // Corpse would move without friction so we make sure there is friction at death
        rb.velocity = Vector3.zero;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;
        rb.sharedMaterial = null;

        hitbox.enabled = false;

        enabled = false;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts) {
            script.enabled = false;
        }
    }

    private void Hide() {
        sr.enabled = false;
    }

    private void Show() {
        sr.enabled = true;
    }

    private void Delete() {
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collider) {
        if (collider.CompareTag("Player") && hp > 0) {
            collider.GetComponent<PlayerMovement>().TakeDamage(onCollisionDamage, this);
        }
    }
    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player") && purified && !manaGiven) {
            anim.SetTrigger("happy");
            gm.SpawnManaBall(RangeToInt(manaGain), transform);
            manaGiven = true;
            gm.ennemiesManaGiven.Add(id);
        }
    }

    public bool TakeDamage(int damage, Vector2 knockback) {
        if (purified) return false;
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

    public void GetParried(Vector2 knockback) {
        if (hp > 0) {
            rb.AddForce(knockback / knockResist, ForceMode2D.Impulse);
            StartCoroutine(Disarmed());
        }
    }

    public void Absorb() {
        canAbsorb = false; // empeche de call plusieurs fois
        StartCoroutine(Absorbing());
    }

    private IEnumerator Absorbing() {
        PlayerAbsorb pa = FindObjectOfType<PlayerAbsorb>();
        absorbed = true;
        float time = 0f;
        while (pa.absorbing && time <= absorbTime) {
            time += Time.deltaTime;
            yield return 0;
        }
        if (time > absorbTime) {
            absorbedCount--;
            if (absorbedCount <= 0) {
                purified = true;
                manaGiven = true;
                gm.purifiedEnnemies.Add(id);
                gm.ennemiesManaGiven.Add(id);
                anim.SetTrigger("happy");
                gm.SpawnManaBall(RangeToInt(manaGain), transform);
            }
            UpdateParticleType();
        }
        absorbed = false;
    }

    private IEnumerator Disarmed() {
        isStun = true;
        canAbsorb = true;
        particle.Play();
        yield return new WaitForSeconds(disarmedTime);
        while (absorbed) {
            yield return 0;
        }
        if (!purified) particle.Stop();
        canAbsorb = false;
        isStun = false;
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
