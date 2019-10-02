using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullTest : MonoBehaviour
{
    public LayerMask test;
    Camera cam;
    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        cam.cullingMask = test;
    }
}
