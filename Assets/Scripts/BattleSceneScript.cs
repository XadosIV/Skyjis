using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneScript : MonoBehaviour
{
    private Camera MainCamera;
    public Camera BattleCamera;
    public Transform FireBorderLeft;
    public Transform FireBorderRight;
    public GameObject bossToSpawn;
    public Transform spawnPoint;
    private Animator LeftAnimator;
    private Animator RightAnimator;
    private bool spawned = false;
    private Enemy bossSpawned;
    private bool inBattle = false; 
    private GameManagerScript gm;
    void Start()
    {
        MainCamera = Camera.main;
        LeftAnimator = FireBorderLeft.GetComponent<Animator>();
        RightAnimator = FireBorderRight.GetComponent<Animator>();
        gm = FindObjectOfType<GameManagerScript>();
    }

    void Update()
    {
        if (spawned) {
            if (bossSpawned.health <= 0) {
                inBattle = false;
                UpdateSceneMod();
            }
        }
    }

    public Enemy GetBoss() {
        if (!inBattle) {
            return null;
        }
        else {
            return bossSpawned;
        }
    }

    private void UpdateSceneMod() {
        MainCamera.enabled = !inBattle;
        BattleCamera.enabled = inBattle;
        FireBorderLeft.GetComponent<SpriteRenderer>().enabled = inBattle;
        FireBorderRight.GetComponent<SpriteRenderer>().enabled = inBattle;
        FireBorderLeft.GetComponent<BoxCollider2D>().enabled = inBattle;
        FireBorderRight.GetComponent<BoxCollider2D>().enabled = inBattle;
        if (inBattle) {
            LeftAnimator.SetTrigger("InBattle");
            RightAnimator.SetTrigger("InBattle");
            bossSpawned = Instantiate(bossToSpawn, spawnPoint.position, spawnPoint.rotation).GetComponent<Enemy>();
            bossSpawned.health = bossSpawned.maxHealth;
        } else {
            LeftAnimator.SetTrigger("OutBattle");
            RightAnimator.SetTrigger("OutBattle");
        }
        gm.SetInBattle(inBattle);
        gm.UpdateUI();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !spawned) {
            spawned = true;
            inBattle = true;
            UpdateSceneMod();
        }
    }

}
