using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LineParametersSO", order = 1)]
public class LineParametersSO : ScriptableObject
{
    //parameters are as follows:
    [Range(1, 7)]
    public int Sections;

    [Range(1, 7)]
    public int LinesPerSection;

    [Range(5, 15)]
    public List<int> PointsPerSection;

    [Range(1, 7)]
    public int Reflections;

    [Range(1, 7)]
    public int Branches;

    public List<Material> Materials;

    [Range(0, 1)]
    public float WidthOfLine;

    [Range(0, 5)]
    public float BoundaryWidth;

    public List<Texture2D> AlphaTextures;
    public List<Texture2D> Textures;


    public Material CircleMat;
    public Material SquareMat;
    public List<Material> MatBank;
    public Material _CenterPiece;

    public List<float> XTile;
    public List<float> YTile;

    public List<float> XOffset;
    public List<float> YOffSet;

    public float ExperienceLength;

    //[Range(1, 7)]
    //public int Color;

    //[Range(.01f, 1)]
    //public float Density;

}
