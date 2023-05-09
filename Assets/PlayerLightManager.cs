using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightManager : MonoBehaviour
{
    public List<Light2D> caveLights;
    GameManager gm;
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        foreach (Light2D l in caveLights) {
            l.enabled = gm.isCaveScene;
        }
    }
}
