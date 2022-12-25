using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform boundariesTransform;
    public float timeOffset;
    public Vector3 posOffset;
    public UnityEngine.Tilemaps.Tilemap map;

    private float direction = 1;
    private Vector3 velocity = Vector3.zero;

    private GameObject player;
    private PlayerMovement playerScript;
    private BoxCollider2D boundaries;

    private float yMin;
    private float yMax;
    private float xMin;
    private float xMax;

    void Awake() {
        boundaries = boundariesTransform.GetComponent<BoxCollider2D>();

        Vector3 min = boundaries.bounds.min;
        Vector3 max = boundaries.bounds.max;

        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;


        xMax = max.x - width / 2;
        xMin = min.x + width / 2;
        yMax = max.y - height / 2;
        yMin = min.y + height / 2;
    }

    void Start() {
        playerScript = FindObjectOfType<PlayerMovement>();
        player = playerScript.gameObject;
        transform.position = player.transform.position;
    }

    void Update() {
        if (playerScript.direction != direction) {
            posOffset = new Vector3(-posOffset.x, posOffset.y, posOffset.z);
            direction = playerScript.direction;
        }

        Vector3 nextPos = Vector3.SmoothDamp(transform.position, player.transform.position + posOffset, ref velocity, timeOffset);
        
        if (nextPos.x > xMax) {
            nextPos = new Vector3(xMax, nextPos.y, nextPos.z);
        } else if (nextPos.x < xMin) {
            nextPos = new Vector3(xMin, nextPos.y, nextPos.z);
        }

        if (nextPos.y > yMax) {
            nextPos = new Vector3(nextPos.x, yMax, nextPos.z);
        } else if (nextPos.y < yMin) {
            nextPos = new Vector3(nextPos.x, yMin, nextPos.z);
        }

        transform.position = nextPos;
    }

    public Vector2[] GetBoundaries () {
        Vector2 min = boundaries.bounds.min;
        Vector2 max = boundaries.bounds.max;
        return new[] { min, max};
    }

}
