using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GrainCTL : MonoBehaviour
{
    [Range(1,5)]
    public int speed = 1;
    PostProcessVolume vol;

    void Start()
    {
        vol = GetComponent<PostProcessVolume>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            vol.weight += .025f * speed;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            vol.weight -= .025f * speed;
        }
    }
}
