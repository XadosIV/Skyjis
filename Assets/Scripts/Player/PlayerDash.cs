using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    private float dashingDistance = 15f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 0.5f;

    private GameManager gm;
    private Rigidbody2D rb;

    private TrailRenderer tr;
    private PlayerMovement pm;

    private bool inCooldown = false;
    private bool dashing = false;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        gm = FindObjectOfType<GameManager>();
        tr = GetComponent<TrailRenderer>();
        pm = GetComponent<PlayerMovement>();
    }

    public bool IsAvailable() {
        if (!gm.save.hasDash) return false;
        if (dashing || inCooldown) return false;
        return true;
    }

    /*public int BlockControl() {
        if (dashing) return 2; //on bloque les contrôles durant tout le dash
        return 0;
    }*/
    public void Execute() {
        StartCoroutine(Dash(pm.direction));
    }

    private IEnumerator Dash(float direction) {
        int id = pm.AddBlockAction(new bool[] { true, true, true, true });
        dashing = true;
        inCooldown = true;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(new Vector2(dashingDistance * direction, 0f), ForceMode2D.Impulse);
        float gravity = rb.gravityScale;
        rb.gravityScale = 0f;
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        dashing = false;
        pm.RemoveBlockAction(id);
        rb.gravityScale = gravity;
        yield return new WaitForSeconds(dashingCooldown);
        inCooldown = false;
    }

}
