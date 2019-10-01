using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFrustDims : MonoBehaviour
{
    public GameObject screen;
    Camera cam;
    float pos;
    float h;
    private void Start()
    {
        //distance = Mathf.Abs(screen.transform.position.z - transform.position.z);
        cam = GetComponent<Camera>();
        pos = (cam.nearClipPlane + 0.01f);
        //var frustumHeight = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        //set the renderTexture dimensions of screen equal to frustrum height.
    }

    void Update()
    {

        screen.transform.position = cam.transform.position + cam.transform.forward * pos;

        h = Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;

        screen.transform.localScale = new Vector3(h * cam.aspect, h, 1f);




        screen.transform.position = cam.transform.position + cam.transform.forward * pos;
        screen.transform.LookAt(cam.transform);
        screen.transform.Rotate(90.0f, 0.0f, 0.0f);

        h = (Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f) / 10.0f;

        transform.localScale = new Vector3(h * cam.aspect, 1.0f, h);
    }
}
