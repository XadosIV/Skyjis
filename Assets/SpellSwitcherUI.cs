using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellSwitcherUI : MonoBehaviour
{
    GameManager gm;

    [SerializeField] private Image[] cells;

    [SerializeField] private Sprite[] cellState;

    [SerializeField] private GameObject view;

    [SerializeField] TextMeshProUGUI voidText;

    private int[] cursor;

    private Dictionary<int, int> spellCells;

    private Image im;

    private bool running;

    private int currentSpellIndex = 0;

    /* 
    0 = nothing
    1 = selected
    2 = disabled
    3 = disabled selected
    4 = choice
    5 = choice selected
    6 = choice and disabled
    7 = choice and disabled selected
    */

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        cursor = new int[2] { 0, 0 };
        im = GetComponent<Image>();

        Visible(false);

        spellCells = new Dictionary<int, int>();
        for (int i = 0; i < gm.save.spellIndex.Length; i++) { 
            spellCells[gm.save.spellIndex[i]] = i;
        }
    }

    void Update() {
        if (gm.save.spellsOwned == 0) return;
        if (running) return;
        if (Input.GetButton("SpellSwitcher")) {
            StartCoroutine(Active());
        }
    }

    int GetSpellsUnlocked() {
        return gm.save.spellsOwned - 1;
    }

    int GetTargetCell() {
        return 3*cursor[0] + cursor[1];
    }

    bool IsDisabled(int id) {
        return id > GetSpellsUnlocked() || //Spell non débloqué
            (spellCells.ContainsKey(id) && id != GetCellIdFromSpellIndex(currentSpellIndex)); // Spell débloqué et dans spellIndex mais pas celui qu'on change.
    }

    bool IsSelected(int id) {
        return id == GetTargetCell();
    }

    bool IsChoosed(int id) {
        return spellCells.ContainsKey(id);
    }

    bool IsValid(int cell) {
        return !(IsDisabled(cell) || IsChoosed(cell));
    }

    Sprite GetSprite(bool disabled, bool selected, bool choice) {
        int id = 0;
        if (selected) id += 1;
        if (disabled) id += 2;
        if (choice) id += 4;
        return cellState[id];
    }

    void UpdateCells() {
        for (int cellId = 0; cellId < cells.Length; cellId++) {
            bool disabled = IsDisabled(cellId);
            bool selected = IsSelected(cellId);
            bool choice = IsChoosed(cellId);

            TextMeshProUGUI cellText = cells[cellId].GetComponentInChildren<TextMeshProUGUI>();
            if (spellCells.ContainsKey(cellId)) {
                cellText.text = (spellCells[cellId] + 1).ToString();
            }
            else {
                cellText.text = "";
            }

            cells[cellId].sprite = GetSprite(disabled, selected, choice);
        }
    }

    void HandleCursor() {
        int width = gm.save.spellsOwned / 3;
        int height = 3;
        if (Input.GetButtonDown("IndicatorLeft")) { // Left
            cursor[0] -= 1;
        }
        if (Input.GetButtonDown("IndicatorRight")) { // Right
            cursor[0] += 1;
        }
        if (Input.GetButtonDown("Interaction")) { // Up
            cursor[1] -= 1;
        }
        if (Input.GetButtonDown("Crouch")) { // Down
            cursor[1] += 1;
        }

        if (cursor[0] < 0) {
            cursor[0] = width-1;
        }else if (cursor[0] >= width) {
            cursor[0] = 0;
        }

        if (cursor[1] < 0) {
            cursor[1] = height-1;
        }
        else if (cursor[1] >= height) {
            cursor[1] = 0;
        }
    }

    void PlaceCursor(int cellId) {
        cursor[0] = cellId / 3;
        cursor[1] = cellId % 3;
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
    }


    IEnumerator Active() {
        PlayerMovement pm = FindObjectOfType<PlayerMovement>();
        int id = pm.StartCinematic(false);
        Visible(true);

        while (Input.GetButton("SpellSwitcher")) {
            HandleSpellSwitchButton();

            UpdateCells();

            voidText.text = (currentSpellIndex + 1).ToString();

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
