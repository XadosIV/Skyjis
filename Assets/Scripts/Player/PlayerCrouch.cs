using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    private PlayerMovement pm;
    private bool crouching = false;

    public CapsuleCollider2D standingCollider;
    public CapsuleCollider2D crouchingCollider;

    Vector3 offsetStanding;
    Vector3 sizeStanding;
    Vector3 offsetCrouch;
    Vector3 sizeCrouch;

    private Animator animator;

    public Transform floorCheck;
    public float floorCheckRadius;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();

        offsetStanding = standingCollider.offset;
        sizeStanding = standingCollider.size;
        offsetCrouch = crouchingCollider.offset;
        sizeCrouch = crouchingCollider.size;
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
        //crouchingCollider.enabled = true;
        //standingCollider.enabled = false;

        standingCollider.offset = offsetCrouch;
        standingCollider.size = sizeCrouch;

        pm.AddSpeedFactor(this, 0.7f);

        LayerMask collisionLayers = GetComponent<PlayerMovement>().physicsLayers;


        while (Input.GetButton("Crouch") || Physics2D.OverlapCircle(floorCheck.position, floorCheckRadius, collisionLayers)) {
            yield return null;
        }

        //standingCollider.enabled = true;
        //crouchingCollider.enabled = false;
        standingCollider.offset = offsetStanding;
        standingCollider.size = sizeStanding;


        pm.RemoveSpeedFactor(this);
        crouching = false;
        pm.RemoveBlockAction(id);
    }

}
