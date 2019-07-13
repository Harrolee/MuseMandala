using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchLR : MonoBehaviour
{
    Vector3[] myLine;
    LineRenderer myLR;
    int lineCounter;

    void Awake()
    {
        PrepareLR();
    }

    void PrepareLR()
    {
        myLR = GetComponent<LineRenderer>();
        //set position to origin for now.
        myLR.SetPosition(0, transform.position);
        myLR.numCapVertices = 3;
        myLR.numCornerVertices = 3;
    }

    public void GetPoints(Vector3[] newLine)
    {
        myLine = newLine;
       // print("len of myLine is:" + myLine.Length); //len is correct

        //foreach (Vector3 point in myLine)
        //{
        //    print(transform.GetSiblingIndex() + " points: " + point);
        //}
    }

    //
    public void PlotPoints()
    {
        foreach (Vector3 point in myLine)
        {
            
            myLR.positionCount = lineCounter + 1;
            myLR.SetPosition(lineCounter, point);

            lineCounter++;
        }
    }

    public void RenderBranch()
    {
        //lerp branch into existence.
    }

}
