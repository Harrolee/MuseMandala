using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFrustrumDimensions : MonoBehaviour
{
    public GameObject[] screens = new GameObject[2];
    Camera cam;
    float h;
    float w;
    float distance;

    public void TurnOnScreen()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        SetScreen();
    }

    private void SetScreen()
    {
        distance = Mathf.Abs(screens[0].transform.position.z - transform.position.z);
        cam = GetComponent<Camera>();
        h = .1f * Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * .5f) * distance * 2f;

        //trust me on this
        w = .805f;//h * cam.aspect

        //set dimensions of planes:
        foreach (GameObject screen in screens)
        {
            screen.transform.localScale = new Vector3(w, 1, h);
        }
    }
}
