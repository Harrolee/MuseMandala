using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using extOSC;

public class EffectsMaster_old : MonoBehaviour
{
    [Range(0,10.0f)]
    public float FogSpeed = 1;
    [Range(0, 1.5f)]
    public float GrainSpeed = .5f;
    PostProcessVolume vol;
    public List<ParticleSystem> _Fog;

    void Start()
    {
        //EffectsMaster assumes it's on the same gameobject as 
        //this scene's PostProcessingVolume component.
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

    public void GiveFeedback(float feedback)
    {
        //Value sits between .5 and .72
        //I'll inflate it.

        //sends value to _Fog and Grain
        foreach (var pSystem in _Fog)
        {
            StartCoroutine(AdjustFog(feedback, pSystem.main));
        }
        print("feedback is: " + feedback);
        StartCoroutine(AdjustWeight(feedback));
    }

    public IEnumerator AdjustFog(float endVal, ParticleSystem.MainModule fog)
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

    public IEnumerator AdjustWeight(float endVal)
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
