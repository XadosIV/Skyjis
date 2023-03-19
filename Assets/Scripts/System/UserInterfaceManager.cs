using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    private GameManagerScript gm;
    private Canvas canvas;
    [SerializeField] private GameObject inGameCanvas;
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

    private void Awake() {
        gm = FindObjectOfType<GameManagerScript>();
        canvas = GetComponent<Canvas>();
        Hide();
    }

    public void Show() {
        inGameCanvas.SetActive(true);
    }

    public void Hide() {
        inGameCanvas.SetActive(false);
    }

    public void UpdateUI() {
        UpdateHearts();
        UpdateBossBar();
        UpdateMana();
        UpdateCoins();
    }

    private void UpdateHearts() {
        if (gm.Health > gm.maxHealth) {
            gm.Health = gm.maxHealth;
        }

        for (int i = 0; i < hearts.Length; i++) {
            if (i < gm.maxHealth) {
                hearts[i].enabled = true;
                if (i < gm.Health) {
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
        manaBar.maxValue = gm.maxMana;
        manaBar.value = gm.Mana;
        manaText.text = gm.Mana + " / " + gm.maxMana;
    }

    private void UpdateCoins() {
        coinsText.text = gm.Coins.ToString();
    }

    private void UpdateBossBar() {
        parentBossHealthBar.SetActive(gm.InBattle);
        if (gm.InBattle) {
            BattleSceneScript bs = FindObjectOfType<BattleSceneScript>();
            Enemy boss = bs.GetBoss();
            bossHealthBar.maxValue = boss.maxHealth;
            bossHealthBar.value = boss.health;
            bossHealthText.text = boss.health + " / " + boss.maxHealth;
            bossNameText.text = boss.bossName;
        }
    }

}
