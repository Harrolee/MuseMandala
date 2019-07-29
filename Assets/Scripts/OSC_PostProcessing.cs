using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using extOSC;

public class OSC_PostProcessing : MonoBehaviour
{
    [Range(0,10.0f)]
    public float FogSpeed = 1;
    [Range(0, 1.5f)]
    public float GrainSpeed = .5f;
    PostProcessVolume vol;
    public OSCReceiver reciever;
    public List<ParticleSystem> _Fog;

    void Start()
    {
        //osc recieve setup:
        reciever.Bind("/muse/fog", FogOSC);
        reciever.Bind("/muse/grain", GrainOSC);

        print("bound");

        vol = GetComponent<PostProcessVolume>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            vol.weight += .025f * GrainSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            vol.weight -= .025f * GrainSpeed;
        }
    }

    void FogOSC(OSCMessage message)
    {
        if (message.ToFloat(out float value))
        {
            print("fog recieved Val: " + value);
            foreach (var pSystem in _Fog)
            {
                StartCoroutine(AdjustFog(value, pSystem.main));
            }
            
        }
    }


    IEnumerator AdjustFog(float endVal, ParticleSystem.MainModule fog)
    {
        print("fog in coro " + endVal);
            float startLifeMult = fog.startLifetimeMultiplier;
            float degree = 0;
            if (startLifeMult < endVal)
            {
                while (fog.startLifetimeMultiplier < endVal)
                {
                print("increasing: " + degree + " from " + startLifeMult + " toward " + endVal);
                degree += (FogSpeed * Time.deltaTime);
                    fog.startLifetimeMultiplier = Mathf.Lerp(startLifeMult, endVal, degree);
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                while (fog.startLifetimeMultiplier > endVal)
                {
                print("decreasing: " + degree + " from "+ startLifeMult +" toward " + endVal);
                degree += (FogSpeed * Time.deltaTime);
                    fog.startLifetimeMultiplier = Mathf.Lerp(startLifeMult, endVal, degree);
                    yield return new WaitForEndOfFrame();
                }
            }
        print("done");
    }



    void GrainOSC(OSCMessage message)
    {
        if (message.ToFloat(out float value))
        {
            print("recieved Val: " + value);
            print("val as string" + value.ToString());
            StartCoroutine(AdjustWeight(value));
        }

        //vol.weight += message;//.025f * speed;
    }

    IEnumerator AdjustWeight(float endVal)
    {
        float startWeight = vol.weight;
        float degree = 0;
        print("coro" + endVal);
        if (vol.weight < endVal)
        {
            while (vol.weight < endVal)
            {
                degree += (GrainSpeed * Time.deltaTime);
                vol.weight = Mathf.Lerp(startWeight, endVal, degree);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (vol.weight > endVal)
            {
                degree += (GrainSpeed * Time.deltaTime);
                vol.weight = Mathf.Lerp(startWeight, endVal, degree);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
