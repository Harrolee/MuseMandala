﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mandala;

public class MGMT : MonoBehaviour
{
    [SerializeField]
    GameObject Camera;
    public PrefabsSO Prefabs;
    public LineParametersSO MandalaParams;
    public List<ColorSwatch> ColorSwatches;
    public List<ColorSwatch> BackgroundPalletes;
    public GameObject _LineSourceGO;
    public Material CircleMat;
    public Material SquareMat;
    public List<Material> MatBank;
    public Material _CenterPiece;
    //390secs is 6.6mins
    public float TotalSeconds = 390;
    [HideInInspector]
    public float IntroSeconds = 60;
    [HideInInspector]
    public List<float> SectionSeconds;
    List<LineSource> sectionRoots = new List<LineSource>();

    public Material _BackPlaneMat;

    [HideInInspector]
    public LineRenderer[] BackgroundTriangles;

    int currSectionRoot = 0;
    //make section sources
    void Start()
    {
        TimeBreakdown();

        //start background pulse
        StartCoroutine(Effects.PingPongLerp(_BackPlaneMat, "_Float", 10));

        MakeSectionSources();
        BackgroundTriangles = Utilities.MakeLR(Prefabs._Background, 4, sectionRoots[0].transform);

        //Randomly Set CenterFold
        _CenterPiece.SetTexture("Texture2D", MandalaParams.AlphaTextures[3]);//Random.Range(0, MandalaParams.AlphaTextures.Count)]);
    }


    void Update()
    {
        //we'll want to generate additional child line renderers for each section.
        if (Input.GetKeyDown(KeyCode.A))
            sectionRoots[currSectionRoot].CallRender();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(MoveCamera(Camera.transform.position.z, Camera.transform.position.z - 3f, 340f));
            sectionRoots[currSectionRoot].GenerateSection();
        }
            
    }

    void TimeBreakdown()
    {        
        //Time Breakdown:
        //Subtract 60secs from the total time.
        //give all sections 1/7th of the total time.
        for (int ii = 0; ii < MandalaParams.Sections; ii++)
        {
            SectionSeconds.Add((TotalSeconds - IntroSeconds) / MandalaParams.Sections);
        }

        //Give the first section, the centerpiece, 1/5th of every other section's time.
        for (int i = 0; i < MandalaParams.Sections; i++)
        {
            SectionSeconds[0] = SectionSeconds[i] * .2f;
        }
        for (int i = 0; i < MandalaParams.Sections; i++)
        {
            SectionSeconds[i] -= SectionSeconds[i] * .2f;
        }
        //Do more time stuff here.
    }

    IEnumerator MoveCamera(float startZ, float endZ, float totalSecs)
    {
        float retreatInc = 3f;
        float pauseLength = 20f;
        float currZ;
        float startTime;
        float currTime;
        float d;
        for (int i = 0; i < 7; i++)
        {
            Debug.Log("round" + (i + 1));
            yield return new WaitForSeconds(pauseLength * .25f);
            startTime = Time.time;
            currTime = Time.time - startTime;
            Debug.Log("starting round" + (i + 2));
            while (currTime < (totalSecs-(pauseLength * 7))/7)
            {
                currTime = Time.time - startTime;
                d = currTime / ((totalSecs - (pauseLength * 7)) / 7);
                //Debug.Log("d is" + d);
                currZ = Mathf.Lerp(startZ, endZ, d);
                Camera.transform.position = new Vector3(0,0,currZ);
                yield return null;
            }
            yield return new WaitForSeconds(pauseLength * .75f);
            Debug.Log("done first pause");
            endZ -= retreatInc;
            startZ = Camera.transform.position.z;


        }
    }

    void MakeSectionSources()
    {
        GameObject newSection;
        for (int sections = 0; sections < MandalaParams.Sections; sections++)
        {
            newSection = Instantiate(_LineSourceGO);
            sectionRoots.Add(newSection.GetComponent<LineSource>());
            Utilities.MakeLR(Prefabs._TrunkLR, MandalaParams.LinesPerSection, sectionRoots[sections].transform);
        }
    }
}
