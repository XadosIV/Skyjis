using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynthesisScript : MonoBehaviour
{
    private GameManager gm;

    [SerializeField] int heal;

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        gm.Health += (int)heal;
    }


}
