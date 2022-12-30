using System.Collections;
using UnityEngine;

public class PlayerSpellCast : MonoBehaviour
{
    public Transform attackPointLeft;
    public Transform attackPointRight;

    private Animator animator;
    private PlayerMovement pm;
    private GameManagerScript gm;


    public bool[] canCast;
    private bool isCasting;

    private GameObject currentSpellToCast;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        gm = FindObjectOfType<GameManagerScript>();
    }

    public bool IsAvailable(int index) {
        if (isCasting) return false;
        return canCast[index];
    }


    public void Execute(int index) {
        print(index);
        if (gm.spellIndex[index] != -1) {
            isCasting = true;
            currentSpellToCast = gm.spellList[gm.spellIndex[index]];
            animator.SetTrigger("SpellCast");
            StartCoroutine(HandleSpellCd(index));
        }
    }

    private IEnumerator HandleSpellCd(int index) {
        canCast[index] = false;
        print(currentSpellToCast.GetComponent<Projectile>().cooldown);
        yield return new WaitForSeconds(currentSpellToCast.GetComponent<Projectile>().cooldown);
        canCast[index] = true;
    }

    private void SpellCreation() {
        if (pm.direction == 1) {
            Instantiate(currentSpellToCast, attackPointRight.position, attackPointRight.rotation);
        } else {
            Instantiate(currentSpellToCast, attackPointLeft.position, attackPointLeft.rotation);
        }
        isCasting = false;
    }

}
