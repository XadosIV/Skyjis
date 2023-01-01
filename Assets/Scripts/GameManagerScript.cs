using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsDontDestroy;

    public int health;
    public int maxHealth;
    public int mana;
    public int maxMana;
    public int coinsCount;
    public bool hasDoubleJump;
    public bool hasDash;
    public bool hasTeleport;
    public int attackDamage;
    public float attackRange;
    public float attackSpeed;
    public float knockback;
    public float selfKnockback;
    public float moveSpeed;
    public float jumpForce;
    public float jumpTime;
    public float invincibleTime;
    public float dashingDistance;
    public float dashingTime;
    public float dashingCooldown;
    public float teleportRange;
    public int[] spellIndex = new int[3];

    public GameObject[] spellList;

    public string spawnName;
    public bool needAwakeAnimation;

    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Slider manaBar;
    [SerializeField] private Text manaText;
    [SerializeField] private Text coinsText;
    [SerializeField] private GameObject[] coinsObjects;
    [SerializeField] private GameObject manaBall;

    public Vector2[] getBoundaries() {
        CameraFollow cam = FindObjectOfType<CameraFollow>();
        return cam.GetBoundaries();
    }

    void Awake()
    {
        foreach (GameObject element in objectsDontDestroy) {
            DontDestroyOnLoad(element);
        }
    }

    public void GiveMana(int amount) {
        mana += amount;
        if (mana > maxMana) {
            mana = maxMana;
        }
        UpdateUI();
    }

    public void GiveLife(int amount) {
        health += amount;
        if (health > maxHealth) {
            health = maxHealth;
        }
        UpdateUI();
    }

    public void SpawnManaBall(int _manaAmount, Transform _position) {
        GameObject manaBallObject = Instantiate(manaBall, _position.position, _position.rotation);
        ManaBall manaBallScript = manaBallObject.GetComponent<ManaBall>();
        manaBallScript.manaGain = _manaAmount;
    }

    public void UpdateUI() {
        UpdateHearts();
        UpdateMana();
        UpdateCoins();
    }

    private void UpdateHearts() {
        if (health > maxHealth) {
            health = maxHealth;
        }

        for (int i = 0; i < hearts.Length; i++) {
            if (i < maxHealth) {
                hearts[i].enabled = true;
                if (i < health) {
                    hearts[i].sprite = fullHeart;
                }
                else {
                    hearts[i].sprite = emptyHeart;
                }
            }
            else {
                hearts[i].enabled = false;
            }
        }
    }

    private void UpdateMana() {
        manaBar.maxValue = maxMana;
        manaBar.value = mana;
        manaText.text = mana+" / "+maxMana;
    }

    private void UpdateCoins() {
        coinsText.text = coinsCount.ToString();
    }

    public List<GameObject> MoneyToCoin(int _money) {
        int[] values = new int[] { 100, 20, 5, 1};
        List<GameObject> coinsToSpawn = new List<GameObject>();

        foreach (GameObject coin in coinsObjects) {
            int value = coin.GetComponent<Coins>().value;
            if (value != 1 && _money == value && coinsToSpawn.Count == 0) { //préfèrera pour money = 20 par exemple, de faire spawn 4x5 pièces plutôt que 1x20
                continue;
            }
            for (int i = 0; i != _money/value; i++) {
                coinsToSpawn.Add(coin);
            }
            _money %= value;
        }
        return coinsToSpawn;
    }
}
