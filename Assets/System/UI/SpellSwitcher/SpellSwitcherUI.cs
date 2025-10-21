using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SpellSwitcherUI : MonoBehaviour
{
    UserInterfaceManager ui;
    GameManager gm;

    [SerializeField] private Image[] cells;

    [SerializeField] private Transform cursor;

    [SerializeField] private Sprite cellEnabled;
    [SerializeField] private Sprite cellDisabled;

    [SerializeField] private Transform[] spellsBorder;

    [SerializeField] private GameObject view;

    [SerializeField] Image voidBorder;

    private int[] cursorPos;

    private Dictionary<int, int> spellCells;

    private Image im;

    private bool running;

    private int currentSpellIndex = 0;

    void Start()
    {
        ui = FindObjectOfType<UserInterfaceManager>();
        gm = FindObjectOfType<GameManager>();
        cursorPos = new int[2] { 0, 0 };
        im = GetComponent<Image>();

        Visible(false);

        spellCells = new Dictionary<int, int>();
        for (int i = 0; i < gm.save.spellIndex.Length; i++) { 
            spellCells[gm.save.spellIndex[i]] = i;
        }

        for (int cellId = 0; cellId < cells.Length; cellId++) {
            Image spellImage = cells[cellId].GetComponentsInChildren<Image>()[1];
            if (!IsUnlocked(cellId)) {
                spellImage.enabled = true;
                SpriteRenderer sr = gm.spellList[cellId].GetComponent<SpriteRenderer>();
                spellImage.sprite = sr.sprite;
                if (sr.flipX) {
                    spellImage.transform.eulerAngles = new Vector3(0, 180, 0);
                }
                if (cellId == 1) {
                    spellImage.rectTransform.sizeDelta = new Vector2(300, 300);
                }
            } else {
                spellImage.enabled = false;
            }
            
        }
        PlaceBorder();
        
    }

    void Update() {
        if (gm.save.spellsUnlocked == 0) return;
        if (running) return;
        if (Input.GetButton("SpellSwitcher")) {
            StartCoroutine(Active());
        }
    }

    int GetSpellsUnlocked() {
        return gm.save.spellsUnlocked-1;
    }

    int GetTargetCell() {
        return 3* cursorPos[0] + cursorPos[1];
    }

    bool IsUnlocked(int id) {
        return id > GetSpellsUnlocked();
    }

    bool IsDisabled(int id) {
        return IsUnlocked(id) || //Spell non débloqué
            (spellCells.ContainsKey(id) && id != GetCellIdFromSpellIndex(currentSpellIndex)); // Spell débloqué et dans spellIndex mais pas celui qu'on change.
    }

    bool IsChoosed(int id) {
        return spellCells.ContainsKey(id);
    }

    bool IsValid(int cell) {
        return !(IsDisabled(cell) || IsChoosed(cell));
    }

    Sprite GetSprite(bool _disabled){
        if (_disabled) return cellDisabled;
        else return cellEnabled;
    }

    void UpdateCells() {
        for (int cellId = 0; cellId < cells.Length; cellId++) {
            bool disabled = IsDisabled(cellId);

            cells[cellId].sprite = GetSprite(disabled);
        }
    }

    int GetHeight() {
        if (gm.save.spellsUnlocked >= 3) return 3;
        else return gm.save.spellsUnlocked;
    }

    int GetWidth() {
        if (GetHeight() >= 3) return gm.save.spellsUnlocked / 3;
        else return 1;
    }

    void HandleCursor() {
        int width = GetWidth();
        int height = GetHeight();
        if (Input.GetButtonDown("IndicatorLeft")) { // Left
            cursorPos[0] -= 1;
        }
        if (Input.GetButtonDown("IndicatorRight")) { // Right
            cursorPos[0] += 1;
        }
        if (Input.GetButtonDown("Interaction")) { // Up
            cursorPos[1] -= 1;
        }
        if (Input.GetButtonDown("Crouch")) { // Down
            cursorPos[1] += 1;
        }

        if (cursorPos[0] < 0) {
            cursorPos[0] = width-1;
        }else if (cursorPos[0] >= width) {
            cursorPos[0] = 0;
        }

        if (cursorPos[1] < 0) {
            cursorPos[1] = height-1;
        }
        else if (cursorPos[1] >= height) {
            cursorPos[1] = 0;
        }

        cursor.position = cells[GetTargetCell()].transform.position;
    }

    void PlaceBorder() {

        /*
         spell.Key = spellId
        spell.Value = BorderId (0,1,2)
         */

        foreach (KeyValuePair<int, int> spell in spellCells) {
            if (spell.Value < gm.save.spellsUnlocked) {
                spellsBorder[spell.Value].GetComponent<Image>().enabled = true;
                spellsBorder[spell.Value].transform.position = cells[spell.Key].transform.position;
            } else {
                spellsBorder[spell.Value].GetComponent<Image>().enabled = false;
            }
            
        }
    }

    void PlaceCursor(int cellId) {
        cursor.position = cells[cellId].transform.position;
    }

    void PlaceCursorOnCurrentSpell() { 
        PlaceCursor(GetCellIdFromSpellIndex(currentSpellIndex));
    }

    int GetCellIdFromSpellIndex(int index) {
        int cellId = -1;
        foreach (KeyValuePair<int, int> spell in spellCells) {
            // spell.Key = cellId / spell.Value = index for save.spellIndex
            if (index == spell.Value) {
                cellId = spell.Key;
            }
        }
        return cellId;
    }

    void SetSpellSlot(int spell, int slot)
    {
        gm.save.spellIndex[slot] = spell;
        PlaceBorder();
        ui.UpdateUI();
    }
    

    void SwitchCell(int cellId) {
        int currentCellId = GetCellIdFromSpellIndex(currentSpellIndex);

        spellCells.Remove(currentCellId);
        spellCells.Add(cellId, currentSpellIndex);

        SetSpellSlot(cellId, currentSpellIndex);
    }

    void HandleSpellSwitchButton()
    {
        if (Input.GetButtonDown("Spell1"))
        {
            currentSpellIndex = 0;
        }
        else if (Input.GetButtonDown("Spell2"))
        {
            currentSpellIndex = 1;
        }
        else if (Input.GetButtonDown("Spell3"))
        {
            currentSpellIndex = 2;
        }
    }

    IEnumerator Active() {
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        int id = pm.StartCinematic(false);
        Visible(true);

        while (Input.GetButton("SpellSwitcher")) {
            UpdateCells();

            voidBorder.sprite = spellsBorder[currentSpellIndex].GetComponent<Image>().sprite;

            HandleCursor();

            if (Input.GetButtonDown("Spell1") || Input.GetButtonDown("Spell2") || Input.GetButtonDown("Spell3")) {
                HandleSpellSwitchButton();
                int cellId = GetTargetCell();
                if (IsValid(cellId)){
                    SwitchCell(cellId);
                } else if (spellCells.ContainsKey(cellId)) { // Si le spell est déjà dans spellIndex
                    //On considère que le joueur veut échanger les deux spells de touche

                    // Au nom c'était horrible à écrire, voilà des coms pour expliquer
                    int pressOn_Spell = cellId;             // La case sur laquelle il appuie
                    int pressOn_Slot = currentSpellIndex;   // La couleur QU'IL VEUT pour cette case
                    int currentSlot = spellCells[cellId];   // Mais la couleur actuelle de la case
                    int currentSpell = GetCellIdFromSpellIndex(pressOn_Slot); // Et la case actuelle de la couleur (pressOnSlot)

                    // On intervertit correctement les 2
                    SetSpellSlot(currentSpell, currentSlot);
                    SetSpellSlot(pressOn_Spell, pressOn_Slot);

                    // On remodifie la magie noir sinon l'affichage se fait mal apparemment
                    spellCells.Remove(pressOn_Spell);
                    spellCells.Add(pressOn_Spell, pressOn_Slot);

                    spellCells.Remove(currentSpell);
                    spellCells.Add(currentSpell, currentSlot);

                    // On lui dit de tout update
                    PlaceBorder();
                    ui.UpdateUI();

                    // Jsuis censé avoir à modifier deux valeurs dans un tableau, pourquoi c'est si compliqué pitié
                }
            }
            yield return null;
        }
        pm.ExitCinematic(id);
        Visible(false);
    }

    void Visible(bool isVisible) {
        running = isVisible;
        if (isVisible) {
            view.SetActive(true);
            im.color = new Color(0, 0, 0, .5f);
        }
        else {
            view.SetActive(false);
            im.color = new Color(0, 0, 0, 0);
        }
    }
}

