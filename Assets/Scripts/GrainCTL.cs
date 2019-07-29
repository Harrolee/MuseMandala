using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using extOSC;

public class GrainCTL : MonoBehaviour
{
    [Range(1,5)]
    public int speed = 1;
    PostProcessVolume vol;
    public OSCReceiver reciever;

    void Start()
    {
        //osc recieve setup:
        reciever.Bind("/muse/test", GrainOSC);
        print("bound");

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

    public void GrainOSC(OSCMessage message)
    {
        if(message.ToFloat(out float value))
        {
            print("recieved Val: " + value);
            print("val as string" + value.ToString());
            StartCoroutine(IncGrain(value));
        }

        //vol.weight += message;//.025f * speed;
    }

    IEnumerator IncGrain(float endVal)
    {
        float startWeight = vol.weight;
        float degree = 0;
        print("coro" + endVal);
        if (vol.weight < endVal)
        {
            while (vol.weight < endVal)
            {
                degree += (speed * Time.deltaTime);
                vol.weight = Mathf.Lerp(0, endVal, degree);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (vol.weight > endVal)
            {
                degree += (speed * Time.deltaTime);
                vol.weight = Mathf.Lerp(startWeight, endVal, degree);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
