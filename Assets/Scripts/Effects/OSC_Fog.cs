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

    //need to suit value range to behavior of wind.4
    //incoming range is  .5-.9999999
    //wind range is .01w-.8w
    //need .8w to be .3 and .01w to be .5
    //variables for linear conversion:
    readonly float oldMax = .3f;
    readonly float oldMin = 1;
    readonly float newMax = .9f;
    readonly float newMin = .01f;
    float oldRange;
    float newRange;
    float windValue;
    //oldMax is smaller than oldMin because we 
    //want ascent in the old scale to  
    //translate to descent in the new scale.

    void Start()
    {
        receiver = gameObject.AddComponent<OSCReceiver>();
        //osc recieve setup:
        receiver.Bind("/muse/fog/*", AdjustFog);
        //receiver.Bind("/muse/grain", GrainOSC);
        receiver.Bind("/muse/*", AdjustWind);
        print("bound");

        //prepare linear conversion variables.
        oldRange = oldMax - oldMin;
        newRange = newMax - oldMin;
    }

    void AdjustFog(OSCMessage message)
    {
        if (message.ToFloat(out float value))
        {
            print("fog recieved Val: " + value);
            //perform a linear conversion 
            windValue = (value - oldMin) / oldRange * newRange + newMin;

            foreach (var pSystem in _Fog)
            {
                StartCoroutine(AdjustFog(windValue, pSystem.main));
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
