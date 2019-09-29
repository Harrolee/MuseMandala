using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using extOSC;

public class EffectsMaster : MonoBehaviour
{
    #region Public Vars
    [Range(0, 10.0f)]
    public float WindSpeed = 1;
    public WindZone _WindZone;
    public GameObject _Photosphere;
    #endregion

    #region Incoming Val Range
    //Learn the new value range and inflate accordingly.

    //need to suit value range to behavior of wind.4
    //incoming range is  .5-.85
    //wind range is .01w-.8w
    //need .8w to be .3 and .01w to be .5
    //variables for linear conversion:
    readonly float oldMax = .98f;
    readonly float oldMin = .5f;
    readonly float newMax = 3.5f;
    readonly float newMin = .01f;
    float oldRange;
    float newRange;
    float changeValue;
    //oldMax is smaller than oldMin because we 
    //want ascent in the old scale to  
    //translate to descent in the new scale.
    #endregion


    private void Awake()
    {
        //prepare linear conversion variables.
        oldRange = oldMax - oldMin;
        newRange = newMax - oldMin;
    }

    //feedback seems to range between .5 and .98
    public void GiveFeedback(float feedback)
    {
        Debug.Log("received val is: " + feedback);
        changeValue = (feedback - oldMin) / oldRange * newRange + newMin;
        Debug.Log("change val is: " + changeValue);
        StartCoroutine(AdjustWind(changeValue));
    }
    GameObject photosphere;
    public void BlowAwayMandala()
    {
        //end of experience.
        Debug.Log("Thanks for playing!");
        photosphere = Instantiate(_Photosphere, GameObject.FindWithTag("MainCamera").transform);
    }

    IEnumerator AdjustWind(float endVal)
    {
        float startWind = _WindZone.windMain;
        float degree = 0;
        print("coro" + endVal);
        //I wonder if each condition will yield the same results
        if (_WindZone.windMain < endVal)
        {
            while (_WindZone.windMain < endVal)
            {
                degree += (WindSpeed * Time.deltaTime);
                _WindZone.windMain = Mathf.Lerp(startWind, endVal, degree);
                yield return null;
            }
        }
        else
        {
            while (_WindZone.windMain > endVal)
            {
                degree += (WindSpeed * Time.deltaTime);
                _WindZone.windMain = Mathf.Lerp(startWind, endVal, degree);
                yield return null;
            }
        }
    }

}
