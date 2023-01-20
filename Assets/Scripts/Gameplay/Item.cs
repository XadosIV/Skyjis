using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int id;
    public string itemName;
    public string description;
    public SpriteRenderer key;
    private GameManagerScript gm;
    private bool given = false;
    void Start()
    {
        gm = FindObjectOfType<GameManagerScript>();
        key.enabled = false;
        if (gm.saveData.inventory.Contains(id)) Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (key.enabled && Input.GetButtonDown("Interaction") && !given) {
            given = true;
            gm.AddItem(id);
            Destroy(gameObject);
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
