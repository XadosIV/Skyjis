using UnityEngine;

public class Warp : MonoBehaviour
{
    public string sceneName;
    public int spawnNumber;
    private bool teleported = false;

    private GameManagerScript gm;

    public Transform spawnPoint;

    void Start() {
        gm = FindObjectOfType<GameManagerScript>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !teleported) {
            teleported = true;
            StartCoroutine(gm.SwitchScene(sceneName, spawnNumber));
        }
    }
}