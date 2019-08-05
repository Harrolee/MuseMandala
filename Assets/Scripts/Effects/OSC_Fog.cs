using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using extOSC;

public class OSC_Fog : MonoBehaviour
{
    [Range(0,10.0f)]
    public float FogSpeed = 1;

    [Range(0, 10.0f)]
    public float WindSpeed = 1;

    OSCReceiver receiver;
    public List<ParticleSystem> _Fog;
    public WindZone _WindZone;

    void Start()
    {
        receiver = gameObject.AddComponent<OSCReceiver>();
        //osc recieve setup:
        receiver.Bind("/muse/fog", AdjustFog);
        //receiver.Bind("/muse/grain", GrainOSC);
        receiver.Bind("/muse/wind", AdjustWind);
        print("bound");

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //vol.weight += .025f * GrainSpeed;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //vol.weight -= .025f * GrainSpeed;
        }
    }

    void AdjustFog(OSCMessage message)
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



    void AdjustWind(OSCMessage message)
    {
        if (message.ToFloat(out float value))
        {
            print("Wind recieved Val: " + value);
            StartCoroutine(AdjustWind(value));
        }
    }

    IEnumerator AdjustWind(float endVal)
    {
        float startWind = _WindZone.windMain;
        float degree = 0;
        print("coro" + endVal);
        //I think each condition will yield the same results
        if (_WindZone.windMain < endVal)
        {
            while (_WindZone.windMain < endVal)
            {
                degree += (WindSpeed * Time.deltaTime);
                _WindZone.windMain = Mathf.Lerp(startWind, endVal, degree);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (_WindZone.windMain > endVal)
            {
                degree += (WindSpeed * Time.deltaTime);
                _WindZone.windMain = Mathf.Lerp(startWind, endVal, degree);
                yield return new WaitForEndOfFrame();
            }
        }
    }
    //void GrainOSC(OSCMessage message)
    //{
    //    if (message.ToFloat(out float value))
    //    {
    //        print("recieved Val: " + value);
    //        print("val as string" + value.ToString());
    //        StartCoroutine(AdjustWeight(value));
    //    }

    //    //vol.weight += message;//.025f * speed;
    //}

    //IEnumerator AdjustWeight(float endVal)
    //{
    //    float startWeight = vol.weight;
    //    float degree = 0;
    //    print("coro" + endVal);
    //    if (vol.weight < endVal)
    //    {
    //        while (vol.weight < endVal)
    //        {
    //            degree += (GrainSpeed * Time.deltaTime);
    //            vol.weight = Mathf.Lerp(startWeight, endVal, degree);
    //            yield return new WaitForEndOfFrame();
    //        }
    //    }
    //    else
    //    {
    //        while (vol.weight > endVal)
    //        {
    //            degree += (GrainSpeed * Time.deltaTime);
    //            vol.weight = Mathf.Lerp(startWeight, endVal, degree);
    //            yield return new WaitForEndOfFrame();
    //        }
    //    }
    //}
}
