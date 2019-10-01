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
    [SerializeField]
    GameObject effectsMGMT;
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

    List<GameObject> mandala = new List<GameObject>();
    GameObject mandalaMother;

    public LayerMask noMandala;

    int currSectionRoot = 0;
    //make section sources
    void Start()
    {
        TimeBreakdown();
        mandalaMother = new GameObject();
        mandalaMother.layer = LayerMask.NameToLayer("mandala");
        centerSection = Instantiate(_CenterSectionSource, mandalaMother.transform);

        //start background pulse
        StartCoroutine(Effects.PingPongLerp(_BackPlaneMat, "_Float", 10));

        MakeSectionSource();
        BackgroundTriangles = Utilities.MakeLR(Prefabs._Background, 4, sectionRoots[0].transform);

        //select one of 5 center textures
        int textureIndex = Random.Range(0, 6);
        _CenterPieceMat = centerSection.GetComponent<MeshRenderer>().sharedMaterial;
        _CenterPieceMat.SetTexture("Texture2D", MandalaParams.AlphaTextures[textureIndex]);
    }

    void OnDisable()
    {
        _IntroCylinder.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Alpha", .5f);
        _CenterPieceMat.SetFloat("_Alpha", 0);
    }

    //Called when CompressReadouts.cs has collected 300 samples
    public IEnumerator BeginSequence()
    {
        float fadeTime = 4;
        StartCoroutine(Effects.LerpMatOverTime(_IntroCylinder.GetComponent<MeshRenderer>().sharedMaterial, "_Alpha", .7f, 0, fadeTime));
        yield return new WaitForSeconds(fadeTime * .3f);
        _Fog.SetActive(true);


        sectionRoots[currSectionRoot].GenerateSection();

        //wake up centerfigure
        //centerSection = Instantiate(_CenterSectionSource);
        //pick a texture from a pool to use for it.
        //centerSection.GetComponent<MeshRenderer>().material = 




        foreach (GameObject go in GameObject.FindGameObjectsWithTag("circle"))
        {
            go.transform.parent = mandalaMother.transform;
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("square"))
        {
            go.transform.parent = mandalaMother.transform;
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("mandalaBit"))
        {
            go.transform.parent = mandalaMother.transform;
        }

        StartCoroutine(Utilities.MoveMandala(mandalaMother, mandalaMother.transform.position.z, mandalaMother.transform.position.z + 3f, IntroSeconds, SectionSeconds));
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
            else if (boundary == 5) //rotate squares
            {
                sectionCount++;
                boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
                StartCoroutine(Effects.RotateOverTime(squares[3].transform, 0, 45, 5));
                StartCoroutine(Effects.RotateOverTime(squares[4].transform, 0, 45, 5, 3));
            }
            else if (boundary == 7)//x tiling
            {
                sectionCount++;
                boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
                StartCoroutine(Effects.PingPongLerp(circles[2].GetComponent<LineRenderer>().material, "_OffsetX", 5, 1, 4));
            }
            else if (boundary == 8) //y offset
            {
                sectionCount++;
                boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
                StartCoroutine(Effects.PingPongLerp(circles[3].GetComponent<LineRenderer>().material, "_TilingX", 4, .08f, 1.5f));
            }
            else if (boundary == 9) //x offset
            {
                print("called boundary 9");
                sectionCount++;
                boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
                StartCoroutine(Effects.PingPongLerp(circles[4].GetComponent<LineRenderer>().material, "_TilingY", 3, 10, .2f));

                //Next is the finale: the blowy outy sand effect!
                StartCoroutine(EndMandala());
            }
        }
    }

    IEnumerator EndMandala()
    {
        //see how long this has to be.
        yield return new WaitForSeconds(TotalSeconds * .13f);
        print("calling sand sequence");

        //stop signal processing
        effectsMGMT.GetComponent<extOSC.Examples.CompressReadouts>().StopCoro();

        //begin sand effect
        Debug.Log("Thanks for playing!");
        _mainCamera.GetComponent<SetFrustrumDimensions>().TurnOnScreen();
        //change the layer that the main camera can see. Cull the MandalaBits Layer.
        _mainCamera.GetComponent<Camera>().cullingMask = noMandala;

        //end game
        StopAllCoroutines();
    }
}