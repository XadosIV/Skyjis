using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGameObject : MonoBehaviour
{
    public int id;
    public SpriteRenderer key;

    private SpriteRenderer sr;
    private GameManager gm;

    private Item item;


    private bool given = false;
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        key.enabled = false;

        if (gm.save.itemsCollected.Contains(id)) Destroy(gameObject);


        item = gm.items.getItem(id);
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = item.sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (key.enabled && Input.GetButtonDown("Interaction") && !given) {
            given = true;
            //im.AddItem(item);
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
