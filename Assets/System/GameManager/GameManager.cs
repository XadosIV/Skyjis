
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public GameData save {
        get;
        private set;
    }
    private UserInterfaceManager UI;

    private int _health;
    public int Health {
        get => _health;
        set {
            if (_health == value) return;
            _health = Mathf.Max(0, Mathf.Min(maxHealth, value));

            if (_health == 0) {
                StartCoroutine(PlayerDeath());
            }

            UI.UpdateUI();
        }
    }

    private int _mana;
    public int Mana {
        get => _mana;
        set {
            if (_mana == value) return;
            _mana = Mathf.Max(0, Mathf.Min(maxMana, value));
            UI.UpdateUI();
        }
    }

    public int Coins {
        get => save.coinsCount;
        set {
            save.coinsCount = value;
            UI.UpdateUI();
        }
    }

    public int maxHealth {
        get; private set;
    }
    
    public int maxMana {
        get; private set;
    }
    public int attackDamage {
        get; private set;
    }

    public GameObject[] spellList;

    public bool isCaveScene;
    public List<String> caveScenes;

    public ItemManager items;
    
    public List<int> killedEnnemies;
    public bool needAwakeAnimation;

    private int saveFileNumber;
    private FileDataHandler fileDataHandler;

    private bool _inBattle = false;
    public bool InBattle {
        get => _inBattle;
        set {
            if (_inBattle != value) {
                _inBattle = value;
                UI.UpdateUI();
            }
        }
    }
    public int spawnNumber;

    
    [SerializeField] private GameObject[] coinsObjects;
    [SerializeField] private GameObject manaBall;

    void Awake() {
        DontDestroyOnLoad(gameObject);
        UI = FindObjectOfType<UserInterfaceManager>();
        items = new ItemManager();
        isCaveScene = caveScenes.Contains(SceneManager.GetActiveScene().name);

    }

    public void SetSaveFileId(int _id, bool _encrypt) {
        saveFileNumber = _id;
        fileDataHandler = new FileDataHandler(Application.persistentDataPath, "save" + saveFileNumber + ".game", _encrypt);

        //LoadGame(_teleport: true); //si pas encrypté => scène joué depuis l'éditeur => pas de téléport.
        LoadGame(_teleport: _encrypt); //si pas encrypté => scène joué depuis l'éditeur => pas de téléport.
    }

    private IEnumerator PlayerDeath() {
        if (save.coinsCount != 0) {
            save.coinsCount = 0;
            save.amountGoldDeathBag = (int)((float)(save.coinsCount) / 2);
            save.posGoldDeathBag = FindObjectOfType<PlayerMovement>().transform.position;
            save.sceneDeath = SceneManager.GetActiveScene().name;
        }
        SaveGame(false);
        needAwakeAnimation = true;
        InBattle = false;
        killedEnnemies.Clear();
        yield return new WaitForSeconds(2f);
        LoadGame();
    }

    public Vector2[] GetBoundaries() {
        BattleSceneScript bs = FindObjectOfType<BattleSceneScript>();
        if (!InBattle) {
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

        save = data;

        maxHealth = 5 + data.nbUpgradeHealth;
        Health = maxHealth;
        maxMana = 100 + data.nbUpgradeMana * 50; //ajouter ici le nombre de GS récolté
        Mana = maxMana;
        attackDamage = 8 + data.nbUpgradeDamage * 5;
        needAwakeAnimation = true;

        if (_teleport) {
            StartCoroutine(SwitchScene(data.lastSceneSave, data.lastWarpSave));
        } else {
            spawnNumber = 0;
            UI.Show();
        }
    }

    public void AddMaxHealth() {
        if (save.nbUpgradeHealth >= 5) return;
        maxHealth += 1;
        Health = maxHealth;
        save.nbUpgradeHealth += 1;
        UI.UpdateUI();
        SaveGame(false);
    }

    public void AddMaxMana() {
        if (save.nbUpgradeMana >= 10) return;
        maxMana += 50;
        Mana = maxMana;
        save.nbUpgradeMana += 1;
        UI.UpdateUI();
        SaveGame(false);
    }

    public void AddDamage() {
        if (save.nbUpgradeDamage >= 5) return;
        attackDamage += 5;
        save.nbUpgradeDamage += 1;
        UI.UpdateUI();
        SaveGame(false);
    }

    public void SaveGame(bool checkpoint) {
        if (checkpoint) {
            save.lastSceneSave = SceneManager.GetActiveScene().name;
            save.lastWarpSave = -1;
        }
        fileDataHandler.Save(save);
    }

    public IEnumerator SwitchScene(string _sceneName, int _spawnNumber) {
        spawnNumber = _spawnNumber;
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        FadeSystemScript fadeSystem = FindObjectOfType<FadeSystemScript>();
        if (player) {
            player.StartCinematic();
        }
        if (fadeSystem) {
            fadeSystem.FadeIn();
        }
        yield return new WaitForSeconds(1f);

        isCaveScene = caveScenes.Contains(_sceneName);

        SceneManager.LoadScene(_sceneName);
        UI.Show();
    }

    public void SpawnManaBall(int _manaAmount, Transform _position) {
        GameObject manaBallObject = Instantiate(manaBall, _position.position, _position.rotation);
        ManaBall manaBallScript = manaBallObject.GetComponent<ManaBall>();
        manaBallScript.manaGain = _manaAmount;
    }
    
    public List<GameObject> MoneyToCoin(int _money) { // coins value : 100 20 5 1
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

    public void SpawnCoins(int _amount, Vector3 _pos, Quaternion _rotation) {
        if (_amount == 0) return;
        else {
            List<GameObject> coins = MoneyToCoin(_amount);
            foreach (GameObject coin in coins) {
                Instantiate(coin, _pos, _rotation);
            }
        }
        
    }
}
