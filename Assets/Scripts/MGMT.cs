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
    public float transLength = 4;
    [SerializeField]
    GameObject effectsMGMT;
    //Music is 7minutes long (420seconds)
    public float TotalSeconds = 420;
    [HideInInspector]
    public float IntroSeconds;
    [HideInInspector]
    public float[] SectionSeconds = new float[8];

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

        //start background pulse
        StartCoroutine(Effects.PingPongLerp(_BackPlaneMat, "_Float", 10));

        MakeSectionSource();
        BackgroundTriangles = Utilities.MakeLR(Prefabs._Background, 4, sectionRoots[0].transform);

        mandalaMother = new GameObject();
        //select one of 5 center textures
        int textureIndex = Random.Range(0, 6);
        print(textureIndex + " was selected");
        float distFromCam = 2f; //why does the centerpiece disappear?
        centerSection = Instantiate(_CenterSectionSource, new Vector3(0,0,distFromCam), Quaternion.identity, mandalaMother.transform);
        centerSection.transform.eulerAngles = new Vector3(0, -90, 90);
        _CenterPieceMat = centerSection.GetComponent<MeshRenderer>().sharedMaterial;
        _CenterPieceMat.SetTexture("Texture2D", MandalaParams.AlphaTextures[textureIndex]);
    }



    //Called when CompressReadouts.cs has collected "history_count" samples
    public IEnumerator BeginSequence()
    {
        
        yield return new WaitForSeconds(30f);

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

        StartCoroutine(Utilities.MoveMandala(mandalaMother, mandalaMother.transform.position.z, mandalaMother.transform.position.z + 3f, SectionSeconds, transLength));
    }

    void TimeBreakdown()
    {
        //we're leaving out the last 30 secs of wind.
        //The wind effect will have to be added manually.

        float intro = 3 / 14;       //90seconds
        IntroSeconds = TotalSeconds * intro;

        //Set section ratios
        SectionSeconds[1] =  3 / 22;  //45seconds
        SectionSeconds[2] = 5 / 33;  //50s
        SectionSeconds[3] = 1 / 6;   //55s
        SectionSeconds[4] = .1f;     //35s
        SectionSeconds[5] = 4 / 33;  //40s
        SectionSeconds[6] = .1f;     //30s
        SectionSeconds[7] = .1f;     //30s
        SectionSeconds[8] = 4 / 33;  //40s
    }

    void MakeSectionSource()
    {
        GameObject newSection;
        newSection = Instantiate(_LineSourceGO);
        sectionRoots.Add(newSection.GetComponent<LineSource>());
        Utilities.MakeLR(Prefabs._TrunkLR, MandalaParams.LinesPerSection, sectionRoots[0].transform);
    }

    public IEnumerator CentralLoop(Material[] boundaryMats, Material centerpieceMat, GameObject[] squares, GameObject[] circles)
    {
        //if alpha is fucked, uncomment this
        ////set alpha of all boundaries to 1;
        //alphaVal = 1;
        //int count = 0;
        //foreach (Material mat in boundaryMats)
        //{
        //    mat.SetFloat("_Alpha", alphaVal);
        //    count++;
        //}



        //first section is the centerpiece generation
        float start = 0;
        float end = 1.5f;
        int sectionCounter = 1;

        //first section:

        Effects.LerpMatOverTime(centerpieceMat, "_Alpha", start, end, SectionSeconds[0]);
        sectionCounter++;
        yield return new WaitForSeconds(SectionSeconds[0] + transLength);


        start = 1;
        end = 0;

        //second section:
        Effects.LerpMatOverTime(boundaryMats[0], "_Alpha", start, end, SectionSeconds[1]);
        yield return new WaitForSeconds(SectionSeconds[1] + transLength);
        //add some shit
        //first ring is quick.
        //After it's appearance, it should perform. 
        //I imagine making a radial wave by changing the width.
        sectionCounter++;

        //third section: create squares
        for(int ii = 1; ii < 6; ii++)
        {
            Effects.LerpMatOverTime(boundaryMats[ii], "_Alpha", start, end, SectionSeconds[2]);
        }
        yield return new WaitForSeconds(SectionSeconds[2] + transLength);
        sectionCounter++;

        //fourth section: rotate squares
        StartCoroutine(Effects.RotateOverTime(squares[3].transform, 0, 45, 5));
        StartCoroutine(Effects.RotateOverTime(squares[4].transform, 0, 45, 5, 3));
        yield return new WaitForSeconds(SectionSeconds[3] + transLength);
        sectionCounter++;

        //fifth section: first circle
        Effects.LerpMatOverTime(boundaryMats[6], "_Alpha", start, end, SectionSeconds[4]);
        yield return new WaitForSeconds(SectionSeconds[4] + transLength);
        StartCoroutine(Effects.PingPongLerp(circles[2].GetComponent<LineRenderer>().material, "_OffsetX", 14, 1, 8));
        sectionCounter++;

        //sixth section: 
        Effects.LerpMatOverTime(boundaryMats[7], "_Alpha", start, end, SectionSeconds[5]);
        yield return new WaitForSeconds(SectionSeconds[5] + transLength);
        StartCoroutine(Effects.PingPongLerp(circles[3].GetComponent<LineRenderer>().material, "_TilingX", 4, .08f, 1.5f));
        sectionCounter++;

        //seventh section:
        Effects.LerpMatOverTime(boundaryMats[8], "_Alpha", start, end, SectionSeconds[6]);
        yield return new WaitForSeconds(SectionSeconds[6] + transLength);
        StartCoroutine(Effects.PingPongLerp(circles[4].GetComponent<LineRenderer>().material, "_TilingY", 3, 10, .2f));
        sectionCounter++;

        //eighth section:
        Effects.LerpMatOverTime(boundaryMats[9], "_Alpha", start, end, SectionSeconds[7]);
        yield return new WaitForSeconds(SectionSeconds[7] + transLength);
        sectionCounter++;

        yield return new WaitForSeconds(SectionSeconds[8] + transLength);
        StartCoroutine(EndMandala());
    }

    IEnumerator EndMandala()
    {
        //see how long this has to be.
        yield return new WaitForSeconds(8f);

        //stop signal processing
        effectsMGMT.GetComponent<extOSC.Examples.CompressReadouts>().StopCoro();

        //begin sand effect
        Debug.Log("Thanks for playing!");
        //to keep this from flashing blue, we can change its layer.
        _mainCamera.GetComponent<SetFrustrumDimensions>().TurnOnScreen();

        for (int i = 0; i < mandalaMother.transform.childCount; i++)
        {
           mandalaMother.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("mandala");
        }

        yield return new WaitForSeconds(5);
        //change the layer that the main camera can see. Cull the mandala layer and activate the maskingPlane.
        _mainCamera.GetComponent<Camera>().cullingMask = noMandala;

        //end game
        StopAllCoroutines();
    }

    void OnDisable()
    {
        _IntroCylinder.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Alpha", .5f);
        _CenterPieceMat.SetFloat("_Alpha", 0);
    }

}