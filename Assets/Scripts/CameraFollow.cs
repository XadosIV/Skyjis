using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public float timeOffset;
    public Vector3 posOffset;
    public UnityEngine.Tilemaps.Tilemap map;

    private float direction = 1;
    private Vector3 velocity = Vector3.zero;

    private GameObject player;
    private PlayerMovement playerScript;

    void Start() {
        playerScript = FindObjectOfType<PlayerMovement>();
        player = playerScript.gameObject;
        transform.position = player.transform.position;
    }

    void Update() {
        transform.position = Vector3.SmoothDamp(transform.position, player.transform.position + posOffset, ref velocity, timeOffset);
        if (playerScript.direction != direction) {
            posOffset = new Vector3(-posOffset.x, posOffset.y, posOffset.z);
            direction = playerScript.direction;
        }
    }
}
