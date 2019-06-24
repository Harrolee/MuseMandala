using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LineParametersSO", order = 1)]
public class LineParametersSO : ScriptableObject
{
    //parameters are as follows:
    [Range(1, 7)]
    public int Sections;

    [Range(5, 15)]
    public List<int> PointsPerSection;

    [Range(1, 7)]
    public int Reflections;

    [Range(1, 7)]
    public int Branches;

    [Range(1, 7)]
    public int Material;

    [Range(1, 7)]
    public int WidthOfLine;

    [Range(1, 7)]
    public int Color;
}
