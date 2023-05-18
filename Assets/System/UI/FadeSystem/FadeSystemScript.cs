using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeSystemScript : MonoBehaviour
{
    public Animator animator;

    private Image i;

    private void Awake() {
        i = GetComponent<Image>();
    }

    public void FadeIn(float r=0f, float g=0f, float b=0f) {
        Color c = new Color(r, g, b);
        i.color = c;
        animator.SetTrigger("FadeIn");
    }
}
