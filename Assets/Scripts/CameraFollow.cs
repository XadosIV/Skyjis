using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public float timeOffset;
    public Vector3 posOffset;
    public UnityEngine.Tilemaps.Tilemap map;
    
    private PlayerMovement playerScript;
    private float direction = 1;
    private Vector3 velocity = Vector3.zero;

    void Start() {
        playerScript = player.GetComponent<PlayerMovement>();
        Vector3Int size = map.size;
    }

    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, player.transform.position + posOffset, ref velocity, timeOffset);
        if (playerScript.direction != direction) {
            posOffset = new Vector3(-posOffset.x, posOffset.y, posOffset.z);
            direction = playerScript.direction;
        }
    }
}
