using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Seal : MonoBehaviour
{
    public float scaleSpeed;
    public float lightIncrease;
    public float waitfor;


    public void Explode() {
        StartCoroutine(SizeUp());
    }

    IEnumerator SizeUp() {
        Light2D light = GetComponent<Light2D>();
        float i = transform.localScale.x;
        while (transform.localScale.x < 35) {
            transform.localScale = new Vector3(i, i, 0);
            i+= scaleSpeed;
            light.intensity += lightIncrease;
            transform.Rotate(new Vector3(0,0,Random.Range(-90,90))) ;
            
            if (i == 20) {
                StartCoroutine(Disappear());
            }

            yield return new WaitForSeconds(waitfor);
        }

    }

    IEnumerator Disappear() {
        FindObjectOfType<FadeSystemScript>().FadeIn(1, 1, 1);
        yield return new WaitForSeconds(1f);

        FindObjectOfType<PlayerMovement>().Show();

        gameObject.SetActive(false);
    }
}
