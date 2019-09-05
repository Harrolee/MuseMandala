using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using extOSC;

public class EffectsMaster : MonoBehaviour
{
    [Range(0,10.0f)]
    public float FogSpeed = 1;
    [Range(0, 1.5f)]
    public float GrainSpeed = .5f;
    PostProcessVolume vol;
    public List<ParticleSystem> _Fog;
    public WindZone _WindZone;

    //feedback seems to range between .5 and .98
    public void GiveFeedback(float feedback)
    {
        //Value sits between .5 and .72
        //I'll inflate it.

        //StartCoroutine(AdjustWind);
        //_WindZone.windMain=

        //sends value to _Fog
        foreach (var pSystem in _Fog)
        {
            StartCoroutine(AdjustFog(feedback * 10, pSystem.main));
        }
        //print("feedback is: " + feedback);
    }

    public IEnumerator AdjustFog(float endVal, ParticleSystem.MainModule fog)
    {
        //print("fog in coro " + endVal);
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
    }
}
