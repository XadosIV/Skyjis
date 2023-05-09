using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenAreaScript : MonoBehaviour
{
    BoundsInt area;
    Tilemap tm;
    BoxCollider2D tilesBounds;

    public Color c;
    void Start()
    {
        tm = GameObject.FindGameObjectWithTag("HiddenMap").GetComponent<Tilemap>();
        c = new Color(tm.color.r, tm.color.g, tm.color.b, tm.color.a);
        tilesBounds = GetComponent<BoxCollider2D>();

        Vector3Int position = Vector3Int.FloorToInt(tilesBounds.bounds.min);
        Vector3Int size = Vector3Int.FloorToInt(tilesBounds.bounds.size + new Vector3Int(0,0,1));
        area = new BoundsInt(position, size);
        tilesBounds.enabled = false;

        UpdateColor(c);
    }

    void Update() {
        //Debug.Log(c.a);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        StopAllCoroutines();
        if (collider.CompareTag("Player")) {
            StartCoroutine(Show());
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        StopAllCoroutines();
        if (collider.CompareTag("Player")) {
            StartCoroutine(Hide());
        }
    }

    IEnumerator Show() {
        while (c.a > 0f) {
            c.a-= 0.03f;
            UpdateColor(c);
            yield return new WaitForSeconds(.01f);
        }
        c.a = 0f;
        UpdateColor(c);
    }

    IEnumerator Hide() {
        while (c.a < 1f) {
            c.a+= 0.03f;
            UpdateColor(c);
            yield return new WaitForSeconds(.01f);
        }
        c.a = 1f;
        UpdateColor(c);
    }

    void UpdateColor(Color color) {
        foreach (Vector3Int point in area.allPositionsWithin) {
            tm.SetTileFlags(point, TileFlags.None);
            tm.SetColor(point, color);
        }
    }
}
