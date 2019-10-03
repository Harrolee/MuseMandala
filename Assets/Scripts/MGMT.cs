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
    public float[] sectionSeconds;

    List<LineSource> sectionRoots = new List<LineSource>();

    public Material _BackgroundMat;

    [HideInInspector]
    public LineRenderer[] BackgroundTriangles;

    List<GameObject> mandala = new List<GameObject>();
    GameObject mandalaMother;

    public LayerMask noMandala;

    int currSectionRoot = 0;
    //make section sources
    void Start()
    {
        sectionSeconds = TimeBreakdown();
        StartCoroutine(BeginSequence());
        //start background pulse
        StartCoroutine(Effects.PingPongLerp(_BackgroundMat, "_Float", 10));

        MakeSectionSource();
        BackgroundTriangles = Utilities.MakeLR(Prefabs._Background, 4, sectionRoots[0].transform);

        mandalaMother = new GameObject();


        //the texture placed below is overidden somewhere

        //select one of 5 center textures
        int textureIndex = Random.Range(0, 6);
        print(textureIndex + " was selected");
        float distFromCam = 2f; //why does the centerpiece disappear?
        centerSection = Instantiate(_CenterSectionSource, new Vector3(0,0,distFromCam), Quaternion.identity, mandalaMother.transform);
        centerSection.transform.eulerAngles = new Vector3(0, -90, 90);
        _CenterPieceMat = centerSection.GetComponent<MeshRenderer>().sharedMaterial;
        _CenterPieceMat.SetTexture("Texture2D", MandalaParams.AlphaTextures[textureIndex]);
    }

    public IEnumerator BeginSequence()
    {
        yield return new WaitForSeconds(IntroSeconds);
        print("introsecs: " + IntroSeconds);
        _Fog.SetActive(true);

        //CentralLoop is cued through this call
        sectionRoots[currSectionRoot].GenerateSection();


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

        StartCoroutine(Utilities.MoveMandala(mandalaMother, mandalaMother.transform.position.z, mandalaMother.transform.position.z + 3f, sectionSeconds, transLength));
    }

    float[] TimeBreakdown()
    {
        //calculate # of seconds reserved for the intro.
        //Deduct that from TotalSeconds.
        float intro = 3f / 14f;       //90seconds
        IntroSeconds = TotalSeconds * intro;
        TotalSeconds -= IntroSeconds;


        sectionSeconds = new float[8]
        {
            3f / 22f,  //45seconds
            5f / 33f,  //50s
            1f / 6f,   //55s
            .1f,     //35s
            4f / 33f,  //40s
            .1f,     //30s
            .1f,     //30s
            4f / 33f   //40s
        };
        print(sectionSeconds[0]);
        for (int i = 0; i < sectionSeconds.Length; i++)
        {
            sectionSeconds[i] *= TotalSeconds;
        }

        //we're leaving out the last 30 secs of wind.
        //The wind effect will have to be added manually.
        return sectionSeconds;
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

        StartCoroutine(Effects.LerpMatOverTime(centerpieceMat, "_Alpha", start, end, sectionSeconds[0]));
        sectionCounter++;
        Debug.LogFormat("first section at {0}", sectionSeconds[0]);
        yield return new WaitForSeconds(sectionSeconds[0] + transLength);


        start = 1;
        end = 0;

        //second section:
        StartCoroutine(Effects.LerpMatOverTime(boundaryMats[0], "_Alpha", start, end, sectionSeconds[1]));
        Debug.LogFormat("first section at {0}", sectionSeconds[1]);
        yield return new WaitForSeconds(sectionSeconds[1] + transLength);
        //add some shit
        //first ring is quick.
        //After it's appearance, it should perform. 
        //I imagine making a radial wave by changing the width.
        sectionCounter++;

        //third section: create squares
        for(int ii = 1; ii < 6; ii++)
        {
            StartCoroutine(Effects.LerpMatOverTime(boundaryMats[ii], "_Alpha", start, end, sectionSeconds[2]));
        }
        yield return new WaitForSeconds(sectionSeconds[2] + transLength);
        sectionCounter++;

        //fourth section: rotate squares
        StartCoroutine(Effects.RotateOverTime(squares[3].transform, 0, 45, 5));
        StartCoroutine(Effects.RotateOverTime(squares[4].transform, 0, 45, 5, 3));
        yield return new WaitForSeconds(sectionSeconds[3] + transLength);
        sectionCounter++;

        //fifth section: first circle
        StartCoroutine(Effects.LerpMatOverTime(boundaryMats[6], "_Alpha", start, end, sectionSeconds[4]));
        yield return new WaitForSeconds(sectionSeconds[4] + transLength);
        StartCoroutine(Effects.PingPongLerp(circles[2].GetComponent<LineRenderer>().material, "_OffsetX", 14, 1, 8));
        sectionCounter++;

        //sixth section: 
        StartCoroutine(Effects.LerpMatOverTime(boundaryMats[7], "_Alpha", start, end, sectionSeconds[5]));
        yield return new WaitForSeconds(sectionSeconds[5] + transLength);
        StartCoroutine(Effects.PingPongLerp(circles[3].GetComponent<LineRenderer>().material, "_TilingX", 4, .08f, 1.5f));
        sectionCounter++;

        //seventh section:
        StartCoroutine(Effects.LerpMatOverTime(boundaryMats[8], "_Alpha", start, end, sectionSeconds[6]));
        yield return new WaitForSeconds(sectionSeconds[6] + transLength);
        StartCoroutine(Effects.PingPongLerp(circles[4].GetComponent<LineRenderer>().material, "_TilingY", 3, 10, .2f));
        sectionCounter++;

        //eighth section:
        StartCoroutine(Effects.LerpMatOverTime(boundaryMats[9], "_Alpha", start, end, sectionSeconds[7]));
        yield return new WaitForSeconds(sectionSeconds[7] + transLength);
        sectionCounter++;

        yield return new WaitForSeconds(sectionSeconds[8] + transLength);
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