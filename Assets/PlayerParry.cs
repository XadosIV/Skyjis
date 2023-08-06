using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParry : MonoBehaviour
{
    [SerializeField] private bool parrying;
    private bool inCooldown;

    public float knockback;
    public float parryTime;

    public float parryCooldown;

    private PlayerMovement pm;
    private Animator animator;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    public bool IsAvailable() {
        if (parrying || inCooldown) return false;
        return true;
    }

    public bool isParrying() {
        return parrying;
    }

    public void StopParrying() {
        parrying = false;
    }

    public void Execute() {
        StartCoroutine(Parry());
    }

    public float GetKnockback() {
        return knockback;
    }

    private IEnumerator ParryDurationHandler() {
        yield return new WaitForSeconds(parryTime);
        parrying = false;
    }

    private IEnumerator Parry() {
        int id = pm.AddBlockAction(new bool[] { true, true, true, true });
        animator.SetTrigger("Parry");
        parrying = true;
        inCooldown = true;

        StartCoroutine(ParryDurationHandler());
        while (parrying) {
            yield return 0;
        }

        pm.RemoveBlockAction(id);
        yield return new WaitForSeconds(parryCooldown);
        inCooldown = false;
    }

    public void Effects (Enemy enemy) {
        enemy.GetParried(new Vector2(knockback * Mathf.Sign(enemy.transform.position.x - transform.position.x), 0));
    }
}
