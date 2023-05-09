using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] int chestId;
    [SerializeField] SpriteRenderer key;
    [SerializeField] int goldAmount;
    [SerializeField] Item i;

    Animator anim;

    bool isCollected = false;
    

    void Start()
    {
        anim = GetComponent<Animator>();

        if (chestId == 0) {
            goldAmount = 0;
            i = null;
            Opening();
        }
    }

    void Update()
    {
        if (key.enabled && Input.GetButtonDown("Interaction") && !isCollected) {
            Opening();
        }
    }

    void Opening() {
        isCollected = true;
        key.enabled = false;
        anim.SetTrigger("Opening");
    }

    //Call in animation
    void SpawnLoot() {
        if (goldAmount != 0) FindObjectOfType<GameManager>().SpawnCoins(goldAmount, transform.position, transform.rotation);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !isCollected) {
            key.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !isCollected) {
            key.enabled = true;
        }
    }
}
