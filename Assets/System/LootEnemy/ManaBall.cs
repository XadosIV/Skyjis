using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBall : MonoBehaviour
{
    public int manaGain;
    private bool collectable = true;
    void Start()
    {
        StartCoroutine(Pop());
    }

    private IEnumerator Pop() {
        float scale = 0f;
        transform.localScale = new Vector2(scale, scale);
        while (transform.localScale.x < 1f) {
            scale += 0.003f;
            transform.localScale = new Vector3(scale, scale, 1) ;
            yield return null;
        }
        transform.localScale = new Vector3(1, 1, 1);
        collectable = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !collectable) {
            collectable = true;
            GameManager gm = FindObjectOfType<GameManager>();
            gm.Mana += manaGain;
            Destroy(gameObject);
        }
    }
}
