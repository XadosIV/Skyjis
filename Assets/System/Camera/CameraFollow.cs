using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform boundariesTransform;
    public float timeOffset;
    public Vector3 posOffset;
    private Vector3 targetPosOffset;

    private Vector3 velocity = Vector3.zero;

    private GameObject player;
    private PlayerMovement playerScript;
    private BoxCollider2D boundaries;

    private Vector3 basePosOffset;
    private float size;
    private float baseTimeOffset;

    private float yMin;
    private float yMax;
    private float xMin;
    private float xMax;


    public void SetSize(float _targetSize, Vector3? offset = null) {
        StopAllCoroutines();
        if (offset != null) {
            targetPosOffset = (Vector3) offset;

            targetPosOffset.y -= 0.25f; // fix for dialog box
        
        }
        StartCoroutine(Zoom(_targetSize, true));
    }

    public void TransformDefault() {
        StopAllCoroutines();
        targetPosOffset = basePosOffset;
        StartCoroutine(Zoom(size, false));

    }

    IEnumerator Zoom(float targetSize, bool zoomIn) {
        float ratio = (targetSize - Camera.main.orthographicSize) / 100;
        timeOffset = 0;
        for (int i = 0; i < 100; i++) {
            Camera.main.orthographicSize += ratio;
            CalculateBoundaries();
            yield return new WaitForSeconds(.005f);
        }
        timeOffset = baseTimeOffset;
    }

    void CalculateBoundaries() {
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

        basePosOffset = new Vector3(posOffset.x, posOffset.y, posOffset.z);
        baseTimeOffset = timeOffset;
        if (!playerScript.CanMove()) {
            posOffset = new Vector3(0, 0, -10);
        }
        targetPosOffset = posOffset;
        size = Camera.main.orthographicSize;
        CalculateBoundaries();
    }

    void Update() {
        if (Mathf.Sign(targetPosOffset.x) != playerScript.direction) {
            targetPosOffset.x *= -1;
        }

        if (posOffset != targetPosOffset) {
            Vector3 ratio = (targetPosOffset - posOffset) / 100;
            posOffset += ratio;
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
