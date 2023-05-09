using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeSystemScript : MonoBehaviour
{
    public Animator animator;

    public void FadeIn() {
        animator.SetTrigger("FadeIn");
    }
}
