using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {
    [SerializeField] private GameObject[] objectsDontDestroy;

    public GameData saveData;

    public int health;
    public int mana;

    [NonSerialized] public int maxHealth;
    [NonSerialized] public int maxMana;
    [NonSerialized] public int attackDamage;

    public GameObject[] spellList;
    public GameObject[] itemList;

    public List<int> killedEnnemies;
    public bool needAwakeAnimation;

    private int saveFileNumber;
    private FileDataHandler fileDataHandler;

    private bool inBattle = false;
    public int spawnNumber;

    [SerializeField] private Canvas UI;
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Slider manaBar;
    [SerializeField] private Text manaText;
    [SerializeField] private Slider bossHealthBar;
    [SerializeField] private Text bossHealthText;
    [SerializeField] private Text bossNameText;
    [SerializeField] private GameObject parentBossHealthBar;
    [SerializeField] private Text coinsText;
    [SerializeField] private GameObject[] coinsObjects;
    [SerializeField] private GameObject manaBall;

    private bool deathAnimPlaying = false;

    public void SetSaveFileId(int _id, bool _encrypt) {
        saveFileNumber = _id;
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, "save" + saveFileNumber + ".game", _encrypt);
        
        LoadGame(_teleport:_encrypt); //si pas encrypté => scène joué depuis l'éditeur => pas de téléport.
    }

    void Awake() {
        UI.enabled = false;
        foreach (GameObject element in objectsDontDestroy) {
            DontDestroyOnLoad(element);
        }
    }

    private void Update() {
        if (health <= 0 && !deathAnimPlaying) {
            deathAnimPlaying = true;
            StartCoroutine(PlayerDeath());
        }
    }

    private IEnumerator PlayerDeath() {
        if (saveData.coinsCount != 0) {
            saveData.coinsCount = 0;
            saveData.amountGoldDeathBag = (int)((float)(saveData.coinsCount) / 2);
            saveData.posGoldDeathBag = FindObjectOfType<PlayerMovement>().transform.position;
            saveData.sceneDeath = SceneManager.GetActiveScene().name;
        }
        SaveGame();
        needAwakeAnimation = true;
        killedEnnemies.Clear();
        yield return new WaitForSeconds(2f);
        LoadGame();
        deathAnimPlaying = false;
    }

    public Vector2[] GetBoundaries() {
        BattleSceneScript bs = FindObjectOfType<BattleSceneScript>();
        if (!inBattle) {
            CameraFollow cam = FindObjectOfType<CameraFollow>();
            return cam.GetBoundaries();
        }
        else {
            BoxCollider2D colliders = bs.GetComponent<BoxCollider2D>();
            return new[] { (Vector2)colliders.bounds.min, (Vector2)colliders.bounds.max };
        }
    }

    public void LoadGame(bool _teleport = true) {
        GameData data = fileDataHandler.Load();
        if (data == null) {
            data = new GameData();
        }

        saveData = data;

        maxHealth = 5 + saveData.nbUpgradeHealth;
        health = maxHealth;
        maxMana = 100 + saveData.nbUpgradeMana * 50;
        mana = maxMana;
        attackDamage = 8 + saveData.nbUpgradeDamage * 5;

        needAwakeAnimation = true;
        if (_teleport) {
            StartCoroutine(SwitchScene(data.lastSceneSave, data.lastWarpSave));
        } else {
            spawnNumber = 0;
            UI.enabled = true;
        }
    }

    public void SaveGame() {
        fileDataHandler.Save(saveData);
    }

    public IEnumerator SwitchScene(string _sceneName, int _spawnNumber) {
        spawnNumber = _spawnNumber;
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        FadeSystemScript fadeSystem = FindObjectOfType<FadeSystemScript>();
        if (player) {
            player.StartCinematic(true);
        }
        if (fadeSystem) {
            fadeSystem.FadeIn();
        }
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(_sceneName);
        UI.enabled = true;
    }

    public void SetInBattle(bool _inBattle) {
        inBattle = _inBattle;
    }

    public void AddItem(int id) {
        saveData.inventory.Add(id);
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
        UpdateBossBar();
        UpdateMana();
        UpdateCoins();
    }

    private void UpdateBossBar() {
        parentBossHealthBar.SetActive(inBattle);
        if (inBattle) {
            BattleSceneScript bs = FindObjectOfType<BattleSceneScript>();
            Enemy boss = bs.GetBoss();
            bossHealthBar.maxValue = boss.maxHealth;
            bossHealthBar.value = boss.health;
            bossHealthText.text = boss.health + " / " + boss.maxHealth;
            bossNameText.text = boss.bossName;
         }
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
        manaText.text = mana + " / " + maxMana;
    }

    private void UpdateCoins() {
        coinsText.text = saveData.coinsCount.ToString();
    }

    public List<GameObject> MoneyToCoin(int _money) {
        int[] values = new int[] { 100, 20, 5, 1 };
        List<GameObject> coinsToSpawn = new List<GameObject>();

        foreach (GameObject coin in coinsObjects) {
            int value = coin.GetComponent<Coins>().value;
            if (value != 1 && _money == value && coinsToSpawn.Count == 0) { //préfèrera pour money = 20 par exemple, de faire spawn 4x5 pièces plutôt que 1x20
                continue;
            }
            for (int i = 0; i != _money / value; i++) {
                coinsToSpawn.Add(coin);
            }
            _money %= value;
        }
        return coinsToSpawn;
    }
}
