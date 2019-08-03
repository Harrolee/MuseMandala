using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Vignette : MonoBehaviour
{
    public float speed = .2f;
    float degree = 0;
    PostProcessVolume vol;
   // Material mat;

    void Start()
    {
        vol = GetComponent<PostProcessVolume>();
//        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
           StopAllCoroutines();
            StartCoroutine(OpenVignette());
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            StopAllCoroutines();
            StartCoroutine(CloseVignette());
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
    //        mat.SetFloat("_ApertureSize", mat.GetFloat("_ApertureSize") + .05f);
            //StopAllCoroutines();
            //StartCoroutine(OpenPlaneVignette());
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            StopAllCoroutines();
            StartCoroutine(ClosePlaneVignette());
        }
    }

    IEnumerator OpenVignette()
    {
        degree = 0;

        while (degree < 1)
        {
            vol.weight = Mathf.Lerp(1, 0, degree);
            degree += (speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator CloseVignette()
    {
        degree = 0;
        while (degree < 1)
        {
            vol.weight = Mathf.Lerp(0, 1, degree);
            degree += (speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator OpenPlaneVignette()
    {
        degree = 0;

        while (degree < .5)
        {
   //         mat.SetFloat("_ApertureSize", Mathf.Lerp(.5f, 0, degree));
            degree += (speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ClosePlaneVignette()
    {
        degree = 0;
        while (degree < .5)
        {
 //           mat.SetFloat("_ApertureSize", Mathf.Lerp(.5f, 0, degree));
            degree += (speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
