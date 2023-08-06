using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    public List<Light2D> caveLights;
    public List<Light2D> outsideLights;
    public List<Light2D> insideLights;
    GameManager gm;
    void Start() {
        gm = FindObjectOfType<GameManager>();

        foreach (Light2D l in caveLights) {
            l.enabled = gm.sceneType == 1;
        }

        foreach(Light2D l in outsideLights) {
            l.enabled = gm.sceneType == 0;
        }

        foreach (Light2D l in insideLights) {
            l.enabled = gm.sceneType == 2;
        }
    }

    public void OffLights() {
        if (gm.sceneType == 1) {
            foreach (Light2D l in caveLights) {
                l.enabled = false;
            }
        } else if (gm.sceneType == 2) {
            foreach (Light2D l in insideLights) {
                l.enabled = false;
            }
        } else {
            foreach (Light2D l in outsideLights) {
                l.enabled = false;
            }
        }
    }

    public void OnLights() {
        if (gm.sceneType == 1) {
            foreach (Light2D l in caveLights) {
                l.enabled = true;
            }
        }
        else if (gm.sceneType == 2) {
            foreach (Light2D l in insideLights) {
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
