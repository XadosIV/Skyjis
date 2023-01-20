using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestingPoint : MonoBehaviour
{
    public SpriteRenderer key;

    private PlayerMovement player;
    private GameManagerScript gm;

    void Start()
    {
        gm = FindObjectOfType<GameManagerScript>();
        player = FindObjectOfType<PlayerMovement>();
        key.enabled = false;
    }

    void Update()
    {
        if (key.enabled && Input.GetButtonDown("Interaction")) {
            if (player.IsInCinematic()) {
                player.animator.SetBool("FallAsleep", false);
                StopAllCoroutines();
                gm.health = gm.maxHealth;
                gm.mana = gm.maxMana;
                gm.UpdateUI();
            } else {
                player.animator.SetBool("FallAsleep", true);
                gm.saveData.lastSceneSave = SceneManager.GetActiveScene().name;
                gm.saveData.lastWarpSave = -1;
                gm.SaveGame();
                StartCoroutine(FullHealth());
                StartCoroutine(FullMana());
                gm.UpdateUI();
            }
        }
    }

    private IEnumerator FullHealth() {
        while (gm.health != gm.maxHealth) {
            gm.GiveLife(1);
            gm.UpdateUI();
            yield return new WaitForSeconds(.2f);
        }
    }

    private IEnumerator FullMana() {
        while (gm.mana != gm.maxMana) {
            gm.GiveMana(3);
            gm.UpdateUI();
            yield return new WaitForSeconds(.1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            key.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            key.enabled = false;
        }
    }
}
