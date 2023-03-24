using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSwitcherUI : MonoBehaviour
{
    GameManager gm;

    [SerializeField] private Image[] cells;

    [SerializeField] private Sprite[] cellState;

    [SerializeField] private GameObject view;

    private int[] cursor;

    private Image im;

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
    }

    void Update() {
        if (gm.save.spellsOwned == 0) return;
        if (Input.GetButton("SpellSwitcher")) {
            if (Input.GetButtonDown("Spell1")) {
                StartCoroutine(Active(0));
            }else if (Input.GetButtonDown("Spell2")) {
                StartCoroutine(Active(1));
            }
            else if (Input.GetButtonDown("Spell3")) {
                StartCoroutine(Active(2));
            }
        }
    }

    int GetSpellsUnlocked() {
        return gm.save.spellsOwned - 1;
    }

    int GetTargetCell() {
        return 3*cursor[0] + cursor[1];
    }

    bool IsDisabled(int id) {
        return id > GetSpellsUnlocked();
    }

    bool IsSelected(int id) {
        return id == GetTargetCell();
    }

    bool IsChoosed(int id) {
        for (int i = 0; i < gm.save.spellIndex.Length; i++) {
            if (id == gm.save.spellIndex[i]) {
                return true;
            }
        }
        return false;
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
        for (int i = 0; i < cells.Length; i++) {
            bool disabled = IsDisabled(i);
            bool selected = IsSelected(i);
            bool choice = IsChoosed(i);

            cells[i].sprite = GetSprite(disabled, selected, choice);
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

    IEnumerator Active(int index) {
        Visible(true);
        while (Input.GetButton("SpellSwitcher")) {
            UpdateCells();

            HandleCursor();

            if (Input.GetButtonDown("Jump")) {
                int cell = GetTargetCell();
                if (IsValid(cell)){
                    gm.save.spellIndex[index] = cell;
                }
            }
            yield return null;
        }
        Visible(false);
    }

    void Visible(bool isVisible) {
        if (isVisible) {
            view.SetActive(true);
            im.color = new Color(0, 0, 0, 128f);
        }
        else {
            view.SetActive(false);
            im.color = new Color(0, 0, 0, 0);
        }
    }
}
