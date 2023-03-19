using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldingSpell : MonoBehaviour
{
    private SpellData data;
    private PlayerMovement pm;
    private GameManagerScript gm;
    private string holdingButton = "Spell";

    private readonly int enemyLayers = 8;


    public float offsetYTexture;

    public float width;
    public float height;
    public float offsetX;
    public float offsetY;

    void Start()
    {
        data = GetComponent<SpellData>();
        pm = FindObjectOfType<PlayerMovement>();
        gm = FindObjectOfType<GameManagerScript>();
        transform.position = pm.transform.position + new Vector3(0, offsetYTexture, 0);
        GameData save = gm.save;
        int i = 0;
        while (i < 3) {
            if (save.spellIndex[i] == data.id) {
                holdingButton += i + 1;
            }
            i++;
        }
        StartCoroutine(Holding());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = pm.transform.position + new Vector3(0, offsetYTexture, 0) ;
    }

    void Damage() {
        Vector2 pos = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, new Vector2(width, height), 0f);
        foreach (Collider2D collider in colliders) {
            if (enemyLayers == collider.gameObject.layer) {
                collider.GetComponentInParent<Enemy>().TakeDamage(data.damage, new Vector2(data.knockback, 0));
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Vector2 pos = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);

        Gizmos.DrawWireCube(pos, new Vector2(width, height));
    }


    IEnumerator Holding() {
        while (Input.GetButton(holdingButton) && gm.Mana >= data.manaCost) {
            gm.Mana -= data.manaCost;
            Damage();
            yield return new WaitForSeconds(.2f);
        }
        Debug.Log("off");
        Destroy(gameObject);
    }
}
