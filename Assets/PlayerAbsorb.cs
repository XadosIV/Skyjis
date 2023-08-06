using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbsorb : MonoBehaviour
{
    private PlayerMovement pm;
    private Animator anim;

    public bool absorbing;

    [SerializeField] private Transform centerAbsorb;
    [SerializeField] private float radiusAbsorb;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private Transform leftAbsorb;
    [SerializeField] private Transform rightAbsorb;

    [SerializeField] private ParticleSystemForceField psff;

    void Start() {
        pm = GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();

        psff = centerAbsorb.GetComponent<ParticleSystemForceField>();
        psff.enabled = false;
    }

    public bool IsAvailable() {
        return !absorbing;
    }

    void Update() {
        if (!absorbing) return;

        Collider2D[] colliders = new Collider2D[5];

        colliders = Physics2D.OverlapCircleAll(centerAbsorb.position, radiusAbsorb, enemyLayer);

        foreach (Collider2D collider in colliders) {
            Enemy e = collider.GetComponentInParent<Enemy>();
            if (e) {
                if (e.canAbsorb) {
                    e.Absorb();
                }
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerAbsorb.position, radiusAbsorb);
    }

    public void Execute() {
        StartCoroutine(Absorb());
    }

    public IEnumerator Absorb() {
        int id = pm.AddBlockAction(new bool[] { true, true, true, true });
        absorbing = true;
        anim.SetBool("Absorb", true);
        psff.enabled = true;
        centerAbsorb.localPosition = pm.direction == 1 ? leftAbsorb.localPosition : rightAbsorb.localPosition;
        while (!Input.GetButtonUp("Focus")) {
            yield return 0;
        }
        absorbing = false;
        psff.enabled = false;
        anim.SetBool("Absorb", false);
        pm.RemoveBlockAction(id);
    }
}
