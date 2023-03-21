using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellData : MonoBehaviour
{
    public int id;
    public string spellName;
    public float damage;
    public Vector2 knockback;
    public float speed;
    public float cooldown;
    public int manaCost;
    [System.NonSerialized] public int direction = 1;

    public bool canCastMidAir;

    public readonly int enemyLayers = 8;

    public Vector2 Knockback(int _direction) {
        return new Vector2(knockback.x * _direction, knockback.y);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
