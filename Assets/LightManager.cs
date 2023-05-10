using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    public List<Light2D> caveLights;
    public List<Light2D> outsideLights;
    GameManager gm;
    void Start() {
        gm = FindObjectOfType<GameManager>();

        foreach (Light2D l in caveLights) {
            l.enabled = gm.isCaveScene;
        }

        foreach(Light2D l in outsideLights) {
            l.enabled = !gm.isCaveScene;
        }
    }

    public void OffLights() {
        if (gm.isCaveScene) {
            foreach (Light2D l in caveLights) {
                l.enabled = false;
            }
        } else {
            foreach (Light2D l in outsideLights) {
                l.enabled = false;
            }
        }
    }

    public void OnLights() {
        if (gm.isCaveScene) {
            foreach (Light2D l in caveLights) {
                l.enabled = true;
            }
        }
        else {
            foreach (Light2D l in outsideLights) {
                l.enabled = true;
            }
        }
    }
}
