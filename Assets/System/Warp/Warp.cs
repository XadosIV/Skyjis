using UnityEngine;

public class Warp : MonoBehaviour
{
    public bool isDoor;
    public SpriteRenderer key;

    public string sceneName;
    public int spawnNumber;
    private bool teleported = false;

    private GameManager gm;
    private PlayerMovement pm;

    public Transform spawnPoint;

    void Start() {
        gm = FindObjectOfType<GameManager>();
        pm = FindObjectOfType<PlayerMovement>();

        key.enabled = false;
    }

    private void Update() {
        if (key.enabled && Input.GetButtonDown("Interaction") && !teleported && pm.CanInteract()) {
            Teleport();
        }
    }

    void Teleport() {
        teleported = true;
        StartCoroutine(gm.SwitchScene(sceneName, spawnNumber));
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (isDoor) {
            key.enabled = true;
        } else {
            if (collision.CompareTag("Player") && !teleported) {
                Teleport();
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (isDoor) key.enabled = false;
    }
}