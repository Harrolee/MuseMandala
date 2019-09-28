using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mandala;

public class MGMT : MonoBehaviour
{
    [SerializeField]
    GameObject _mainCamera;
    public PrefabsSO Prefabs;
    public LineParametersSO MandalaParams;
    public List<ColorSwatch> ColorSwatches;
    public List<ColorSwatch> BackgroundPalletes;
    public GameObject _LineSourceGO;
    public GameObject _IntroCylinder;
    public Material CircleMat;
    public Material SquareMat;
    public List<Material> MatBank;
    GameObject centerSection;
    public GameObject _CenterSectionSource;
    public Material _CenterPieceMat;
    public GameObject _Fog;
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

        MakeSectionSource();
        BackgroundTriangles = Utilities.MakeLR(Prefabs._Background, 4, sectionRoots[0].transform);

        //Randomly Set CenterFold
        _CenterPieceMat.SetTexture("Texture2D", MandalaParams.AlphaTextures[3]);//Random.Range(0, MandalaParams.AlphaTextures.Count)]);
    }


    void Update()
    {
        //we'll want to generate additional child line renderers for each section.
        if (Input.GetKeyDown(KeyCode.A))
            sectionRoots[currSectionRoot].CallRender();

        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(BeginSequence());
            //wake up centerfigure
            //centerSection = Instantiate(_CenterSectionSource);
            //pick a texture from a pool to use for it.
            //centerSection.GetComponent<MeshRenderer>().material = 

            //first: be in a cylinder.
            //be in a cylinder while muse calibrates.

        }

    }

    void OnDisable()
    {
        _IntroCylinder.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Alpha", .5f);
    }

    public IEnumerator BeginSequence()
    {
        float fadeTime = 3;
        StartCoroutine(Effects.LerpMatOverTime(_IntroCylinder.GetComponent<MeshRenderer>().sharedMaterial, "_Alpha", 1, 0, fadeTime));
        yield return new WaitForSeconds(fadeTime * .3f);
        _Fog.SetActive(true);
        //when muse finishes calibrating, start the below
        StartCoroutine(MoveCamera(_mainCamera.transform.position.z, _mainCamera.transform.position.z - 3f, IntroSeconds, SectionSeconds));
        sectionRoots[currSectionRoot].GenerateSection();
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
        print("sectionCount is " + SectionSeconds.Count);
        //Give the first section, the centerpiece, 1/5th of every other section's time.
        for (int i = 0; i < MandalaParams.Sections; i++)
        {
            SectionSeconds[0] += SectionSeconds[i] * .2f;
        }
        for (int i = 0; i < MandalaParams.Sections; i++)
        {
            SectionSeconds[i] -= SectionSeconds[i] * .2f;
        }
        //Do more time stuff here.
    }

    IEnumerator MoveCamera(float startZ, float endZ, float introSecs, List<float> sectionSecs)
    {
        float retreatInc = 6f;
        float pauseLength = 4f;
        float currZ;
        float startTime;
        float currTime;
        float d;

        //remove pause time from section time
        for (int ii = 0; ii < sectionSecs.Count; ii++)
        {
            sectionSecs[ii] -= pauseLength / sectionSecs.Count;
        }

        //intro
        //no movement for introsecs
        print("set camerawork for intro" + introSecs);

        //main event

        for (int i = 0; i < 7; i++)
        {
            Debug.Log("round" + (i + 1));
            yield return new WaitForSeconds(pauseLength * .25f);
            startTime = Time.time;
            currTime = Time.time - startTime;
            print(sectionSecs[i]);
            while (currTime < (sectionSecs[i]))
            {
                currTime = Time.time - startTime;
                d = currTime / sectionSecs[i];
                //Debug.Log("d is" + d);
                currZ = Mathf.Lerp(startZ, endZ, d);
                _mainCamera.transform.position = new Vector3(0, 0, currZ);
                yield return null;
            }
            yield return new WaitForSeconds(pauseLength * .75f);
            Debug.Log("done first pause");
            endZ -= retreatInc;
            startZ = _mainCamera.transform.position.z;
        }
    }

    void MakeSectionSource()
    {
        GameObject newSection;
        newSection = Instantiate(_LineSourceGO);
        sectionRoots.Add(newSection.GetComponent<LineSource>());
        Utilities.MakeLR(Prefabs._TrunkLR, MandalaParams.LinesPerSection, sectionRoots[0].transform);
    }

    public IEnumerator CentralLoop(Material[] boundaryMats, Material centerpieceMat, List<float> sectionSeconds, int sectionCount, GameObject[] squares, GameObject[] circles)
    {
        //this first section is the centerpiece generation
        float start = 0;
        float end = 1.5f;
        float alphaVal;
        float startTime = Time.time;
        float currTime = Time.time - startTime;
        float x;
        float centerpieceSecs = sectionSeconds[0];
        //7 is the number of sections.
        while (currTime < centerpieceSecs)
        {
            currTime = Time.time - startTime;
            x = currTime / centerpieceSecs;
            alphaVal = Mathf.Lerp(start, end, x);
            centerpieceMat.SetFloat("_Alpha", alphaVal);
            yield return null;
        }
        //====================================----------------------


        //set alpha of all boundaries to 1;
        alphaVal = 1;
        int count = 0;
        foreach (Material mat in boundaryMats)
        {
            mat.SetFloat("_Alpha", alphaVal);
            count++;
        }

        //first ring is quick.
        //After it's appearance, it should perform. 
        //I imagine making a radial wave by changing the width.



        //This is the central experience loop.
        //In sequence, lerp all boundaries.
        start = 1;
        end = 0;
        float boundarySecs = sectionSeconds[1];//totalSecs / boundaryMats.Length;
        int sectionCounter = 0;

        for (int boundary = 0; boundary < boundaryMats.Length; boundary++)
        {
            if (boundary == 1)
            {
                boundarySecs = 10f;
            }
            startTime = Time.time;
            currTime = Time.time - startTime;
            while (currTime < boundarySecs)
            {
                currTime = Time.time - startTime;
                x = currTime / boundarySecs;
                alphaVal = Mathf.Lerp(start, end, x);
                boundaryMats[boundary].SetFloat("_Alpha", alphaVal);
                yield return null;
            }
            //reset timing for next run.
            if (boundary == 1)
            {
                sectionCounter++;
                boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
            }
            if (boundary == 5) //rotate squares
            {
                sectionCount++;
                boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
                StartCoroutine(Effects.RotateOverTime(squares[3].transform, 0, 45, 5));
                StartCoroutine(Effects.RotateOverTime(squares[4].transform, 0, 45, 5, 3));
            }
            if (boundary == 7)//x tiling
            {
                sectionCount++;
                boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
                StartCoroutine(Effects.PingPongLerp(circles[2].GetComponent<LineRenderer>().material, "_OffsetX", 5, 1, 4));
            }
            if (boundary == 8) //y offset
            {
                sectionCount++;
                boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
                StartCoroutine(Effects.PingPongLerp(circles[3].GetComponent<LineRenderer>().material, "_TilingX", 4, .08f, 1.5f));
            }
            if (boundary == 9) //x offset
            {
                sectionCount++;
                boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
                StartCoroutine(Effects.PingPongLerp(circles[4].GetComponent<LineRenderer>().material, "_TilingY", 3, 10, .2f));
            }
        }
        //Next is the finale: the blowy outy sand effect!
    }
}