using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mandala;
public class SandEffect : MonoBehaviour
{
    Material[] child_mats = new Material[2];

    private void OnEnable()
    {
        //lerp sphere texture up

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            for (int i = 0; i < child_mats.Length; i++)
            {
                child_mats[i] = transform.GetChild(i).GetComponent<MeshRenderer>().material;
            }
            Sequence();
        }
    }

    void Sequence()
    {
        float fadeTime = 4;
        float brushTime = 1f;
        StartCoroutine(Disappear_Sand(fadeTime, brushTime));
    }

    IEnumerator Disappear_Sand(float fadeTime, float brushTime)
    {
        for (int i = 0; i < child_mats.Length; i++)
        {
            StartCoroutine(Effects.LerpMatOverTime(child_mats[i], "_ACT", 0, 1, fadeTime));
        }

        yield return new WaitForSeconds(fadeTime);

        for (int i = 0; i < child_mats.Length; i++)
        {
            StartCoroutine(Effects.LerpMatOverTime(child_mats[i], "_BlowAway", 0, 1, brushTime));
        }

        yield return new WaitForSeconds(brushTime);
        for (int i = 0; i < child_mats.Length; i++)
        {
            child_mats[i].SetFloat("_Alpha", 0);
        }
    }
}
