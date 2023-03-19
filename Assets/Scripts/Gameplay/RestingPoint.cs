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
                gm.Health = gm.maxHealth;
                gm.Mana = gm.maxMana;
            } else {
                player.animator.SetBool("FallAsleep", true);
                gm.SaveGame(true);
                StartCoroutine(FullHealth());
                StartCoroutine(FullMana());
            }
        }
    }

    private IEnumerator FullHealth() {
        while (gm.Health != gm.maxHealth) {
            gm.Health+=1;
            yield return new WaitForSeconds(.2f);
        }
    }

    private IEnumerator FullMana() {
        while (gm.Mana != gm.maxMana) {
            gm.Mana += 3;
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
