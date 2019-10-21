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
    public GameObject _Frame;
    public GameObject _CircleFrame;
    public Material CircleMat;
    public Material SquareMat;
    public List<Material> MatBank;
    GameObject centerSection;
    public GameObject _CenterSectionSource;
    public Material _CenterPieceMat;
    public GameObject _Fog;
    public GameObject _IntroCylinder;
    public AudioSource _Sound;
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
    Camera gameCam;
    //make section sources
    void Start()
    {
        gameCam = _mainCamera.GetComponent<Camera>();
        sectionSeconds = TimeBreakdown();
        //start background pulse
        StartCoroutine(Effects.PingPongLerp(_BackgroundMat, "_Float", 10));

        MakeSectionSource();
        BackgroundTriangles = Utilities.MakeLR(Prefabs._Background, 4, sectionRoots[0].transform);

        mandalaMother = new GameObject();

        //select one of 5 center textures
        int textureIndex = Random.Range(0, 6);
        print(textureIndex + " was selected");
        float distFromCam = 2f; //why does the centerpiece disappear?
        centerSection = Instantiate(_CenterSectionSource, new Vector3(0, 0, distFromCam), Quaternion.identity, mandalaMother.transform);
        centerSection.transform.eulerAngles = new Vector3(0, -90, 90);
        _CenterPieceMat = centerSection.GetComponent<MeshRenderer>().sharedMaterial;
        _CenterPieceMat.SetTexture("Texture2D", MandalaParams.AlphaTextures[textureIndex]);


        StartCoroutine(Intro());
    }

    public IEnumerator Intro()
    {
        gameCam.farClipPlane = 100;

        TextMesh text = GetComponent<TextMesh>();
        float readTime = 6;

        //ToDo: 
            //instead of time, make a sight-based acknowledgement system, like in Dream of Dali
            //place text on object in front of the user, inside of the cylinder

        text.text = "Open your eyes at the\nstart of the experience.";
        yield return new WaitForSeconds(readTime);

        text.text = "Close your eyes when\nyou hear the crashing gong.";
        yield return new WaitForSeconds(readTime);

        text.text = "Open your eyes again\nat the next gong.";
        yield return new WaitForSeconds(readTime);

        text.text = "Then focus on clearing\nthe fog from the emerging mandala";
        yield return new WaitForSeconds(readTime);

        text.text = "";
        yield return new WaitForSeconds(readTime * .2f);

        //when all four are finished, lerp the cylinder out.
        float fadeTime = 3;
        StartCoroutine(Effects.LerpMatOverTime(_IntroCylinder.GetComponent<MeshRenderer>().sharedMaterial, "_Alpha", .55f, 0, fadeTime));
        yield return new WaitForSeconds(fadeTime);
        //should be black here.

        //start the music.
        StartCoroutine(BeginSequence());
    }

    public IEnumerator BeginSequence()
    {
        _Sound.Play();

        effectsMGMT.GetComponent<extOSC.Examples.CompressReadouts>().BindAddress();

        int min = 100;
        //LayerMask normal = gameCam.cullingMask;
        //_mainCamera.GetComponent<Camera>().cullingMask = normal;
        gameCam.farClipPlane = 25;

        TextMesh text = GetComponent<TextMesh>();
        print("introsecs: " + IntroSeconds);
        //cull everything but MGMT
        //gameCam.cullingMask = LayerMask.NameToLayer("UI");

        yield return new WaitForSeconds(IntroSeconds * .85f);

        //restore camera cutting point
        StartCoroutine(Effects.LerpFarPlaneOverTime(gameCam, min, 1000, 10));
        _Fog.SetActive(true);

        yield return new WaitForSeconds(IntroSeconds * .15f);
        print("finished intro");

        //CentralLoop is cued through the call call GenerateSection
        sectionRoots[currSectionRoot].GenerateSection();

        //child everything to a parent object.
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

        StartCoroutine(Utilities.MoveMandala(mandalaMother, mandalaMother.transform.position.z, mandalaMother.transform.position.z + 9f, sectionSeconds, transLength));
    }

    float[] TimeBreakdown()
    {
        //calculate # of seconds reserved for the intro.
        //Deduct that from TotalSeconds.
        float intro = 3f / 14f;       //intro * total time(420seconds) = 90seconds
        IntroSeconds = TotalSeconds * intro;
        TotalSeconds -= IntroSeconds;

        //section seconds + intro seconds = 315seconds. There are 5 seconds unfilled. The wind effect happens after 420 seconds.
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
        //first section is the centerpiece generation
        float start = -.3f;
        float end = 5.5f;

        //first section:

        StartCoroutine(Effects.LerpMatOverTime(centerpieceMat, "_Alpha", start, end, sectionSeconds[0]));
        Debug.LogFormat("first section at {0}", sectionSeconds[0]);
        yield return new WaitForSeconds(sectionSeconds[0]);

        //set params for first circle and all of the squares
        start = 3.5f;
        end = 7f;

        //second section:
        StartCoroutine(Effects.LerpMatOverTime(boundaryMats[0], "_Alpha", start, end, sectionSeconds[1]));
        Debug.LogFormat("second section at {0}", sectionSeconds[1]);
        yield return new WaitForSeconds(sectionSeconds[1]);
        //add some shit
        //first ring is quick.
        //After it's appearance, it should perform. 
        //I imagine making a radial wave by changing the width.


        //third section: create squares
        float lerpTime = sectionSeconds[2] / 5;
        for (int ii = 1; ii < 6; ii++)
        {
            StartCoroutine(Effects.LerpMatOverTime(boundaryMats[ii], "_Alpha", start, end, lerpTime));
            yield return new WaitForSeconds(lerpTime);
        }

        //fourth section: rotate squares
        StartCoroutine(Effects.RotateOverTime(squares[3].transform, 0, 45, 5));
        yield return new WaitForSeconds(2);
        StartCoroutine(Effects.RotateOverTime(squares[4].transform, 0, 45, 5, 3));
        yield return new WaitForSeconds(sectionSeconds[3]);


        //reset params for the remaining circles
        start = 1;
        end = 0;

        //fifth section: first circle
        StartCoroutine(Effects.LerpMatOverTime(boundaryMats[6], "_Alpha", start, end, sectionSeconds[4]));
        yield return new WaitForSeconds(sectionSeconds[4]);

        //sixth section: 
        StartCoroutine(Effects.LerpMatOverTime(boundaryMats[7], "_Alpha", start, end, sectionSeconds[5]));
        yield return new WaitForSeconds(sectionSeconds[5]);
        //StartCoroutine(Effects.PingPongLerp(circles[2].GetComponent<Renderer>().material, "_OffsetX", 14, 1, 8));
        StartCoroutine(VegasSpin(circles[2].transform));

        //seventh section:
        StartCoroutine(Effects.LerpMatOverTime(boundaryMats[8], "_Alpha", start, end, sectionSeconds[6]));
        yield return new WaitForSeconds(sectionSeconds[6]);
        StartCoroutine(Effects.PingPongLerp(circles[3].GetComponent<LineRenderer>().material, "_TilingX", 4, .08f, 1.5f));

        //eighth section:
        StartCoroutine(Effects.LerpMatOverTime(boundaryMats[9], "_Alpha", start, end, sectionSeconds[7]));
        yield return new WaitForSeconds(sectionSeconds[7]);
        StartCoroutine(Effects.PingPongLerp(circles[4].GetComponent<LineRenderer>().material, "_TilingY", 3, 10, .2f));

        StartCoroutine(EndMandala());
    }

    IEnumerator VegasSpin(Transform go)
    {
        for (int i = 0; i < 15; i++)
        {
            StartCoroutine(Effects.RotateOverTime(go, 0, 360, 16, true));
            yield return new WaitForSeconds(16);
            StartCoroutine(Effects.RotateOverTime(go, 360, 0, 15, true));
            yield return new WaitForSeconds(15);
        }
    }

    IEnumerator EndMandala()
    {
        yield return new WaitForSeconds(10);
        //stop signal processing
        effectsMGMT.GetComponent<extOSC.Examples.CompressReadouts>().StopCoro();

        //begin sand effect

        //to keep this from flashing blue, we can change its layer.
        _mainCamera.GetComponent<SetFrustrumDimensions>().TurnOnScreen();

        for (int i = 0; i < mandalaMother.transform.childCount; i++)
        {
            mandalaMother.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("mandala");
        }
        //remove fog.
        _Fog.SetActive(false);
        yield return new WaitForSeconds(5);
        //change the layer that the main camera can see. Cull the mandala layer and activate the maskingPlane.
        _mainCamera.GetComponent<Camera>().cullingMask = noMandala;

        yield return new WaitForSeconds(8);
        //show words,
        TextMesh text = GetComponent<TextMesh>();
        text.text = "Welcome back.";
        yield return new WaitForSeconds(10);

        StartCoroutine(Effects.LerpFarPlaneOverTime(_mainCamera.GetComponent<Camera>(), 1000, 200, 10));
        StartCoroutine(Effects.LerpNearPlaneOverTime(_mainCamera.GetComponent<Camera>(), .03f, 10, 10));
        yield return new WaitForSeconds(10);

        //end game
        StopAllCoroutines();
    }

    void OnDisable()
    {
        _IntroCylinder.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_Alpha", .5f);
        _CenterPieceMat.SetFloat("_Alpha", 0);
    }
}