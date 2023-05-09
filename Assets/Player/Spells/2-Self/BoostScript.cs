using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostScript : MonoBehaviour
{
    public float boostSpeed;
    public float boostDamage;
    public float effectDuration;

    private PlayerMovement pm;
    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        StartCoroutine(HandleEffect());
    }

    void Update() {
        transform.position = pm.transform.position;
    }

    IEnumerator HandleEffect() {
        pm.AddSpeedFactor(this, boostSpeed);
        pm.AddAttackFactor(this, boostDamage);
        yield return new WaitForSeconds(effectDuration);
        pm.RemoveAttackFactor(this);
        pm.RemoveSpeedFactor(this);
        Destroy(gameObject);
    }
}
