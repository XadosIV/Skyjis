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

    /*public int BlockControl() {
        if (crouching) return 1;
        return 0;
    }*/

    public void Execute() {
        StartCoroutine(Crouch());
    }

    private IEnumerator Crouch() {
        int id = pm.AddBlockAction(new bool[] { false, true, true, true });
        crouching = true;
        standingCollider.enabled = false;
        crouchingCollider.enabled = true;
        pm.AddSpeedFactor(this, 0.7f);

        LayerMask collisionLayers = GetComponent<PlayerMovement>().physicsLayers;


        while (Input.GetButton("Crouch") || Physics2D.OverlapCircle(floorCheck.position, floorCheckRadius, collisionLayers)) {
            yield return null;
        }

        standingCollider.enabled = true;
        crouchingCollider.enabled = false;
        pm.RemoveSpeedFactor(this);
        crouching = false;
        pm.RemoveBlockAction(id);
    }

}
