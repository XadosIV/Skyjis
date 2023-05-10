using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeSystemScript : MonoBehaviour
{
    public Animator animator;

    private Image i;

    private void Start() {
        i = GetComponent<Image>();
    }

    public void FadeIn(float r=0, float g=0, float b=0) {
        Color c = new Color(r, g, b);
        i.color = c;
        animator.SetTrigger("FadeIn");
    }
}
