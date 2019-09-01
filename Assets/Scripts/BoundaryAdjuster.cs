using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryAdjuster : MonoBehaviour
{
    [Range(0, 1.5f)]
    public float XTextureScaler = 1;

    [Range(0, 1.5f)]
    public float YTextureScaler = 1;

    private void Update()
    {
        gameObject.GetComponent<LineRenderer>().material.mainTextureScale = new Vector2(XTextureScaler, YTextureScaler);
    }
}
