using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchLR : MonoBehaviour
{
    Vector3[] myLine;
    LineRenderer myLR;
    int lineCounter;

    //Wake up Logic:::::::
    void Awake()
    {
        PrepareLR();
    }

    void PrepareLR()
    {
        //Later, address the design requirement that forces us to move this to the origin at the start.
        gameObject.transform.position = Vector3.zero;
        myLR = GetComponent<LineRenderer>();
        myLR.numCapVertices = 3;
        myLR.numCornerVertices = 3;
        myLR.sharedMaterial = transform.parent.GetComponent<LineRenderer>().sharedMaterial;
    }
    //----------------------



    public void GetPoints(Vector3[] newLine)
    {
        myLine = newLine;

        PlotPoints();
    }

    public void PlotPoints()
    {
        foreach (Vector3 point in myLine)
        {
            myLR.positionCount = lineCounter + 1;
            myLR.SetPosition(lineCounter, point);

            lineCounter++;
        }
    }
}
