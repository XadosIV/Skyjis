using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthesisScript : MonoBehaviour
{
    private GameManagerScript gm;
    private SpellData data;
    private PlayerMovement pm;
    void Start()
    {
        gm = FindObjectOfType<GameManagerScript>();
        data = GetComponent<SpellData>();
        pm = FindObjectOfType<PlayerMovement>();
        gm.GiveLife(data.damage);
    }

    void Update() {
        transform.position = pm.transform.position;
    }

    void EndAnimation() {
        Destroy(gameObject);
    }
}
