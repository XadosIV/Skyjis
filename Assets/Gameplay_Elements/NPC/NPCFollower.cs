using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFollower : MonoBehaviour {

    private SpriteRenderer sr;
    private NPCDialog npcd;

    public Transform[] spawnpoints;
    public Transform[] checkpoints;
    public string[] dialogs_tag;
    public int spawnpointsIndex;
    public int checkpointsIndex;

    public bool isFollowing = true;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        npcd = GetComponent<NPCDialog>();
        spawnpointsIndex = 0;
        checkpointsIndex = 0;

        if (isFollowing) {
            UpdatePos();
        }

    }

    void Update()
    {
        if (isFollowing) CheckpointContainPlayer();
    }

    public void UpdatePos() {
        sr.flipX = true;
        transform.position = spawnpoints[spawnpointsIndex].position;
        if (dialogs_tag.Length > spawnpointsIndex) npcd.dialog = dialogs_tag[spawnpointsIndex];
    }

    public void Teleport() {
        spawnpointsIndex++;
        UpdatePos();
        SetCheckpointIndex(checkpointsIndex + 1);
    }

    void SetCheckpointIndex(int index) {
        checkpointsIndex = index;
        for (int i = 0; i < checkpoints.Length; i++) {
            checkpoints[i].gameObject.SetActive(i == checkpointsIndex);
        }
    }


    void CheckpointContainPlayer() {
        Collider2D[] tab = new Collider2D[10];

        if (checkpointsIndex < checkpoints.Length) {
            int index = checkpoints[checkpointsIndex].GetComponent<Collider2D>().OverlapCollider(new ContactFilter2D(), tab);
            for (int i = 0; i < index; i++) {
                if (tab[i].CompareTag("Player")) {
                    Teleport();
                    break;
                }
            }
        }
    }
}
