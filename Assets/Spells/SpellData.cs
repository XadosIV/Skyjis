using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellData : MonoBehaviour
{
    public int id;
    public string spellName;
    public int damage;
    public float knockback;
    public float speed;
    public float cooldown;
    public int manaCost;
    [System.NonSerialized] public int direction = 1;

    public bool canCastMidAir;

    public readonly int enemyLayers = 8;


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
