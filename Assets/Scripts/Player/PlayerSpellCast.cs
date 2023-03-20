using System.Collections;
using UnityEngine;

public class PlayerSpellCast : MonoBehaviour
{
    public Transform attackPointLeft;
    public Transform attackPointRight;

    private Animator animator;
    private PlayerMovement pm;
    private GameManager gm;

    int blockActionId;

    public bool[] canCast;
    private bool isCasting;

    private GameObject currentSpell;
    private SpellData currentSpellData;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        gm = FindObjectOfType<GameManager>();
    }

    public bool IsAvailable(int index) {
        if (isCasting) return false;
        return canCast[index];
    }

    /*public int BlockControl() {
        if (isCasting) return 1;
        return 0;
    }*/


    public void Execute(int index) {
        if (gm.save.spellIndex[index] != -1) {
            currentSpell = gm.spellList[gm.save.spellIndex[index]];
            currentSpellData = currentSpell.GetComponent<SpellData>();
            
            if (gm.Mana >= currentSpellData.manaCost) {
                if (!currentSpellData.canCastMidAir && !pm.isGrounded) return;
                isCasting = true;
                blockActionId = pm.AddBlockAction(new bool[] { true, true, true, true });
                animator.SetTrigger("SpellCast");
                StartCoroutine(HandleSpellCd(index));
            }
        }
    }

    private IEnumerator HandleSpellCd(int index) {
        canCast[index] = false;
        yield return new WaitForSeconds(currentSpellData.cooldown);
        canCast[index] = true;
    }

    private void SpellCreation() {
        gm.Mana -= currentSpellData.manaCost;

        if (pm.direction == 1) {
            Instantiate(currentSpell, attackPointRight.position, attackPointRight.rotation);
        } else {
            Instantiate(currentSpell, attackPointLeft.position, attackPointLeft.rotation);
        }
        isCasting = false;
        pm.RemoveBlockAction(blockActionId);
    }

}
