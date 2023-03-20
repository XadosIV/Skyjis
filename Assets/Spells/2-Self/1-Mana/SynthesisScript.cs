using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthesisScript : MonoBehaviour
{
    private GameManager gm;
    private SpellData data;
    private PlayerMovement pm;
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        data = GetComponent<SpellData>();
        pm = FindObjectOfType<PlayerMovement>();
        gm.Health += data.damage;
    }

    void Update() {
        transform.position = pm.transform.position;
    }

    void EndAnimation() {
        Destroy(gameObject);
    }
}
