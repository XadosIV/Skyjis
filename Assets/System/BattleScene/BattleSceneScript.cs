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
    private GameManager gm;
    void Start()
    {
        MainCamera = Camera.main;
        LeftAnimator = FireBorderLeft.GetComponent<Animator>();
        RightAnimator = FireBorderRight.GetComponent<Animator>();
        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (spawned) {
            if (bossSpawned.hp <= 0) {
                UpdateSceneMod();
                gm.InBattle = false;
            }
        }
    }

    public Enemy GetBoss() {
        if (!gm.InBattle) {
            return null;
        }
        else {
            return bossSpawned;
        }
    }

    private void UpdateSceneMod() {
        if (spawned) {
            LeftAnimator.SetTrigger("OutBattle");
            RightAnimator.SetTrigger("OutBattle");
            enabled = false;
        } else {
            bossSpawned = Instantiate(bossToSpawn, spawnPoint.position, spawnPoint.rotation).GetComponent<Enemy>();
        }
        MainCamera.enabled = spawned;
        BattleCamera.enabled = !spawned;
        FireBorderLeft.GetComponent<SpriteRenderer>().enabled = !spawned;
        FireBorderRight.GetComponent<SpriteRenderer>().enabled = !spawned;
        FireBorderLeft.GetComponent<BoxCollider2D>().enabled = !spawned;
        FireBorderRight.GetComponent<BoxCollider2D>().enabled = !spawned;
        
        LeftAnimator.SetTrigger("InBattle");
        RightAnimator.SetTrigger("InBattle");
        
        
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !spawned) {
            UpdateSceneMod();
            spawned = true;
            gm.InBattle = true;
        }
    }

}
