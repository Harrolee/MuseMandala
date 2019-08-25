using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mandala;

public class MGMT : MonoBehaviour
{
    public PrefabsSO Prefabs;
    public LineParametersSO MandalaParams;
    public List<ColorSwatch> ColorSwatches;
    public List<ColorSwatch> BackgroundPalletes;
    public GameObject _LineSourceGO;
    public Material CircleMat;
    public Material SquareMat;
    public Material _CenterPiece;
    List<LineSource> sectionRoots = new List<LineSource>();

    [HideInInspector]
    public LineRenderer[] BackgroundTriangles;

    int currSectionRoot = 0;
    //make section sources
    void Start()
    {
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
            sectionRoots[currSectionRoot].GenerateSection();
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
