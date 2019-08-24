﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MGMT : MonoBehaviour
{
    public PrefabsSO Prefabs;
    public LineParametersSO MandalaParams;
    public List<ColorSwatch> ColorSwatches;
    public GameObject _LineSourceGO;
    public List<Material> CircleMatBank;
    public List<Material> SquareMatBank;
    public Material _CenterPiece;
    List<LineSource> sectionRoots = new List<LineSource>();

    int currSectionRoot = 0;
    //make section sources
    void Start()
    {
        MakeSectionSources();
    }

    void Update()
    {

        //we'll want to generate additional child line renderers for each section.
        if (Input.GetKeyDown(KeyCode.A))
            sectionRoots[currSectionRoot].CallRender();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            sectionRoots[currSectionRoot].GenerateSection();
            //the following for if children inherit this script
            //if (transform.GetSiblingIndex() == 2 && transform.childCount == 4)
            //{
            //    WholeEnchilada();
            //}
        }
        //renderInverse
    }

    void MakeSectionSources()
    {
        GameObject newSection;
        for (int sections = 0; sections < MandalaParams.Sections; sections++)
        {
            newSection = Instantiate(_LineSourceGO);
            sectionRoots.Add(newSection.GetComponent<LineSource>());
            MakeChildLR(MandalaParams.LinesPerSection, sectionRoots[sections].transform);
            
        }
        print("Made Enough Children");
    }

    void MakeChildLR(int numOfChildren, Transform parent)
    {
        for (int i = 0; i < numOfChildren; i++)
        {
            Instantiate(Prefabs._TrunkLR, parent);
        }

    }
}
