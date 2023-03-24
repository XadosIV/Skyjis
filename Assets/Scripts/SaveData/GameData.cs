using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public int nbUpgradeHealth; // 5 + nbUpgradeHealth; = maxHealth
    public int nbUpgradeMana; // 100 + nbUpgradeMana*50; = manaMax
    public int nbUpgradeDamage; // 8 + nbUpgradeDamage*5; = attackdamage

    public int coinsCount;
    public bool hasDoubleJump;
    public bool hasDash;
    public bool hasTeleport;
    
    public int[] spellIndex;
    public int spellsOwned;
    public List<int> inventory;
    public List<int> itemsCollected;
    public int[] flags;
    public string lastSceneSave;
    public int lastWarpSave;

    public Vector3 posGoldDeathBag; // 3
    public int amountGoldDeathBag; // la moitié du goldcount à la mort
    public string sceneDeath; //pour savoir dans quel scène faire spawn le GoldDeathBag


    public GameData() {
        coinsCount = 0;
        hasDoubleJump = false;
        hasDash = false;
        hasTeleport = false;
        spellIndex = new int[]{ -1, -1, -1};
        spellsOwned = 0;
        inventory = new List<int>();
        itemsCollected = new List<int>();
        flags = new int[10];

        posGoldDeathBag = new Vector3();
        amountGoldDeathBag = -1;
        sceneDeath = null;

        lastSceneSave = "oakwood_01";
    }
}
