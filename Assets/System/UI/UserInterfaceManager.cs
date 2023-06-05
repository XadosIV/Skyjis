using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour
{
    private GameManager gm;
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

    [SerializeField] private GameObject[] spellSlots;

    private void Awake() {
        gm = FindObjectOfType<GameManager>();
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
        UpdateSpell();
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

    private void UpdateSpell() {
        int[] spellTab = gm.save.spellIndex;
        for (int i = 0; i < spellTab.Length; i++) {
            int spellId = spellTab[i];
            if (spellTab[i] == -1) {
                spellSlots[i].SetActive(false);
            } else {
                Image spellImage= spellSlots[i].GetComponentsInChildren<Image>()[1];

                SpriteRenderer sr = gm.spellList[spellId].GetComponent<SpriteRenderer>();
                spellImage.sprite = sr.sprite;
                if (sr.flipX) {
                    spellImage.transform.eulerAngles = new Vector3(0, 180, 0);
                } else {
                    spellImage.transform.eulerAngles = new Vector3(0, 0, 0);
                }
                if (spellId == 1) {
                    spellImage.rectTransform.sizeDelta = new Vector2(225, 225);
                } else {
                    spellImage.rectTransform.sizeDelta = new Vector2(0,0);
                }
                spellSlots[i].SetActive(true);
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
        /*if (gm.InBattle) {
            BattleSceneScript bs = FindObjectOfType<BattleSceneScript>();
            Enemy boss = bs.GetBoss();
            bossHealthBar.maxValue = boss.maxHp;
            bossHealthBar.value = boss.hp;
            bossHealthText.text = boss.hp + " / " + boss.maxHp;
            bossNameText.text = boss.bossName;
        }*/
    }

}
