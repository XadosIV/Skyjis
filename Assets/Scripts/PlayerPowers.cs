using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowers : MonoBehaviour
{
    private PlayerMovement pm;
    private GameManagerScript gm;

    private PlayerDash dash;
    private PlayerTeleport teleport;
    private PlayerJump jump;
    private PlayerCrouch crouch;
    private PlayerMeleeHit meleeHit;
    private PlayerSpellCast spells;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        gm = FindObjectOfType<GameManagerScript>();

        dash = GetComponent<PlayerDash>();
        teleport = GetComponent<PlayerTeleport>();
        jump = GetComponent<PlayerJump>();
        crouch = GetComponent<PlayerCrouch>();
        meleeHit = GetComponent<PlayerMeleeHit>();
        spells = GetComponent<PlayerSpellCast>();
    }

    void Update()
    {
        if (gm.health <= 0) return;
        if (pm.IsInCinematic()) return;
        if (IsBlockingControl() >= 1) return;

        if (Input.GetButtonDown("MeleeHit") && meleeHit.IsAvailable()) meleeHit.Execute();
        else if (Input.GetButtonDown("Spell1") && spells.IsAvailable(0)) spells.Execute(0);
        else if (Input.GetButtonDown("Spell2") && spells.IsAvailable(1)) spells.Execute(1);
        else if (Input.GetButtonDown("Spell3") && spells.IsAvailable(2)) spells.Execute(2);

        if (Input.GetButtonDown("Jump") && jump.IsAvailable()) jump.Execute();
        else if (Input.GetButtonDown("Dash") && dash.IsAvailable()) dash.Execute();
        else if (Input.GetButtonDown("Teleport") && teleport.IsAvailable()) teleport.Execute();
        else if (Input.GetButtonDown("Crouch") && crouch.IsAvailable()) crouch.Execute();
    }

    public int IsBlockingControl() { // 0 = pas de contrôle bloqués; 1 = contrôles des pouvoirs bloqués; 2 = tout les contrôles bloqués.
        return Mathf.Max(dash.BlockControl(), teleport.BlockControl(), crouch.BlockControl());
    }

    public bool NullifyVelocity() {
        return teleport.BlockControl() == 2;
    }

    public bool NeedJump() {
        return jump.NeedJump();
    }

}