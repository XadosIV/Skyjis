using System.Collections;
using System.Collections.Generic;
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

    void HandleSpellSwitchButton() {
        if (Input.GetButtonDown("Spell1")) {
            currentSpellIndex = 0;
            PlaceCursorOnCurrentSpell();
        }
        else if (Input.GetButtonDown("Spell2")) {
            currentSpellIndex = 1;
            PlaceCursorOnCurrentSpell();
        }
        else if (Input.GetButtonDown("Spell3")) {
            currentSpellIndex = 2;
            PlaceCursorOnCurrentSpell();
        }
    }

    void SwitchCell(int cellId) {
        int currentCellId = GetCellIdFromSpellIndex(currentSpellIndex);

        spellCells.Remove(currentCellId);

        gm.save.spellIndex[currentSpellIndex] = cellId;

        spellCells.Add(cellId, currentSpellIndex);

        PlaceBorder();

        ui.UpdateUI();
    }


    IEnumerator Active() {
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        int id = pm.StartCinematic(false);
        Visible(true);

        while (Input.GetButton("SpellSwitcher")) {
            HandleSpellSwitchButton();

            UpdateCells();

            voidBorder.sprite = spellsBorder[currentSpellIndex].GetComponent<Image>().sprite;

            HandleCursor();

            if (Input.GetButtonDown("Jump")) {
                int cellId = GetTargetCell();
                if (IsValid(cellId)){
                    SwitchCell(cellId);
                } else if (spellCells.ContainsKey(cellId)) { // Si le spell est déjà dans spellIndex
                    //On considère que le joueur veut "bouger la sélection", on change donc de currentSpellIndex.
                    currentSpellIndex = spellCells[cellId];
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
