using UnityEngine.UI;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsDontDestroy;

    public int health;
    public int maxHealth;
    public int mana;
    public int maxMana;
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

    public void UpdateHearts() {
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
}
