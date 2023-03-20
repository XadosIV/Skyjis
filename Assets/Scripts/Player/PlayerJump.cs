using System.Collections;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private float jumpTime = 0.43f;

    private GameManager gm;
    private PlayerMovement pm;

    private bool isChargingJump = false;

    private bool isDoubleJumping = false;

    private bool jumping = false;

    private float jumpTimeCounter;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        pm = GetComponent<PlayerMovement>();
        jumpTimeCounter = jumpTime;

    }

    public bool IsAvailable() {
        if (isChargingJump) return false;
        if (pm.isGrounded) return true;
        if (gm.save.hasDoubleJump && !isDoubleJumping) return true;
        return false;
    }

    public bool NeedJump() {
        return jumping;
    }

    public void Execute() {
        if (!pm.isGrounded) {
            isDoubleJumping = true;
        }
        StartCoroutine(Jump());
    }

    void Update()
    {
        if (pm.isGrounded) {
            isDoubleJumping = false;
        }
    }

    private IEnumerator Jump() {
        isChargingJump = true;
        while (Input.GetButton("Jump") && jumpTimeCounter > 0) {
            jumpTimeCounter -= Time.deltaTime;
            jumping = true;
            yield return null;
        }
        jumping = false;
        jumpTimeCounter = jumpTime; //reset counter
        isChargingJump = false;
    }
}
