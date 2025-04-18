using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowers : MonoBehaviour
{
    private PlayerMovement pm;
    private GameManager gm;

    private PlayerDash dash;
    private PlayerTeleport teleport;
    private PlayerJump jump;
    private PlayerCrouch crouch;
    private PlayerMeleeHit meleeHit;
    private PlayerSpellCast spells;
    private PlayerParry parry;
    private PlayerAbsorb absorb;

    private const float absorbTiming = .4f;
    private float absorbTimer = .4f;
    private bool absorbing = false;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        gm = FindObjectOfType<GameManager>();

        dash = GetComponent<PlayerDash>();
        teleport = GetComponent<PlayerTeleport>();
        jump = GetComponent<PlayerJump>();
        crouch = GetComponent<PlayerCrouch>();
        meleeHit = GetComponent<PlayerMeleeHit>();
        spells = GetComponent<PlayerSpellCast>();
        parry = GetComponent<PlayerParry>();
        absorb = GetComponent<PlayerAbsorb>();
    }

    void Update()
    {
        if (gm.Health <= 0) return;
        if (!pm.CanAttacks()) return;
        //if (IsBlockingControl() >= 1) return;


        if (Input.GetButtonDown("MeleeHit") && meleeHit.IsAvailable()) meleeHit.Execute();
        else if (Input.GetButtonDown("Spell1") && spells.IsAvailable(0)) spells.Execute(0);
        else if (Input.GetButtonDown("Spell2") && spells.IsAvailable(1)) spells.Execute(1);
        else if (Input.GetButtonDown("Spell3") && spells.IsAvailable(2)) spells.Execute(2);

        if (Input.GetButtonDown("Jump") && jump.IsAvailable()) jump.Execute();
        else if (Input.GetButtonDown("Dash") && dash.IsAvailable()) dash.Execute();
        else if (Input.GetButtonDown("Teleport") && teleport.IsAvailable()) teleport.Execute();
        else if (Input.GetButtonDown("Crouch") && crouch.IsAvailable()) crouch.Execute();
        else if (Input.GetButtonUp("Focus")) {
            if (absorbTimer > 0) {
                if (parry.IsAvailable()) parry.Execute();
            }
            absorbTimer = absorbTiming;
            absorbing = false;
        } else if (Input.GetButton("Focus")) {
            if (absorbing) return;
            absorbTimer -= Time.deltaTime;
            if (absorbTimer <= 0) {
                if (absorb.IsAvailable()) absorb.Execute();
                absorbing = true;
            }
        }
    }


    /*public int IsBlockingControl() { // 0 = pas de contr�le bloqu�s; 1 = contr�les des pouvoirs bloqu�s; 2 = tout les contr�les bloqu�s.
        return Mathf.Max(dash.BlockControl(), teleport.BlockControl(), crouch.BlockControl(), spells.BlockControl());
    }*/

    public bool DontNullifyVelocity() {
        return dash.dashing;
    }

    public bool NeedJump() {
        return jump.NeedJump();
    }
    
    public bool IsParrying() {
        return parry.isParrying();
    }

    public void SuccessParry(Enemy enemy) {
        parry.Effects(enemy);
    }
}