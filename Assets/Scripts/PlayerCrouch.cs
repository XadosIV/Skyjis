using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    private PlayerMovement pm;
    private bool crouching = false;

    public Collider2D standingCollider;
    public Collider2D crouchingCollider;

    private Animator animator;

    public Transform floorCheck;
    public float floorCheckRadius;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    void Update() {
        animator.SetBool("Crouching", crouching);

    }

    public bool IsAvailable() {
        if (!pm.isGrounded) return false;
        return !crouching;
    }

    public int BlockControl() {
        if (crouching) return 1;
        return 0;
    }

    public void Execute() {
        StartCoroutine(Crouch());
    }

    private IEnumerator Crouch() {
        
        crouching = true;
        standingCollider.enabled = false;
        crouchingCollider.enabled = true;
        pm.speedFactor = 0.7f;

        LayerMask collisionLayers = GetComponent<PlayerMovement>().collisionLayers;


        while (Input.GetButton("Crouch") || Physics2D.OverlapCircle(floorCheck.position, floorCheckRadius, collisionLayers)) {
            yield return null;
        }

        standingCollider.enabled = true;
        crouchingCollider.enabled = false;
        pm.speedFactor = 1;
        crouching = false;
    }

}
