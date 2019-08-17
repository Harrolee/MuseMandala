using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildLineRunner : MonoBehaviour//LineSource
{

    public GameObject _ChildLineRenderer;
    [HideInInspector]
    public List<Vector3[]> mySectionReflections;

    LineRenderer myLR;
    LineSource parentScript;
    GameObject _branchChild;
    MGMT mgmtScript;

    //myIndex answers the question: Which Reflection am I?
    int reflIndex;
    int lineCounter =0;
    int numPointsPerLine;
    Vector3[] myLine = new Vector3[15];

    int branchPointBuffer;

    //Below is intro logic.
    //-------------------------------
    void Awake()
    {       //center by force. For later, consider whether there is anything preventing this from working when not at origin.
        gameObject.transform.position = Vector3.zero;
        PrepareLR();
        //when the SO is implemented, reference data from SO
        /*
        reflections = lineParams.Reflections;
        sections = lineParams.Sections;
        */
        reflIndex = transform.GetSiblingIndex();
        mgmtScript = GameObject.FindWithTag("MGMT").GetComponent<MGMT>();
        _branchChild = mgmtScript.Prefabs._BranchLR;
    }

    void PrepareLR()
    {
        myLR = GetComponent<LineRenderer>();
        //set position to origin for now.
        myLR.SetPosition(0, transform.position);
        myLR.numCapVertices = 3;
        myLR.numCornerVertices = 3;
        myLR.sharedMaterial = transform.parent.GetComponent<LineRenderer>().sharedMaterial;
    }
    //---------------------------------
    //Above is intro logic.
    //Below is Main Controlling Method.

    int numCalls = 0;
    //Main Logic Method:
    public void StartLineRender(Vector3[] myGivenLine)
    {
        //must get vars after LineSource is finished generating the section of the mandala.
        GetVarsFromParent();

        
        //why is this here? Because this is where Trunks get their lines from root.
        myLine = myGivenLine;

        //spawn as many children as there will be branches.
        CreateBranchGenerators();

        //at this point, We have all the declared point.
        //The branches have their individual lines.

        //below lies slerper
        SendPointsToLR(0);
    }


    //Below is ETL Logic.
    //-----------------------------------------------------------------
    void GetVarsFromParent()
    {
        parentScript = transform.parent.GetComponent<LineSource>();
        numPointsPerLine = parentScript.numPointsPerLine;
        branchPointBuffer = parentScript.branchPointBuffer;
        print("branchPointBuffer= " + branchPointBuffer);

        //if (_ChildLineRenderer == null)
        //    _ChildLineRenderer = parentScript._ChildLineRenderer;
    }

    void CreateBranchGenerators()
    {
        int numBranches = GetBranchCount();

        Vector3[,] branchMatrix;
        Vector3 branchStartPoint;
        Vector3[] lineToPass;
        for (int branch = 0; branch < numBranches; branch++)
        {
            branchMatrix = parentScript.branchMatrixList[branch];

            branchStartPoint = GetBranchStartPoint(branchMatrix, reflIndex);
            MakeBranchChild(branchStartPoint);

            lineToPass = GetLineToPass(branchMatrix, reflIndex);
            PassBranchChildPoints(branch, lineToPass);
        }
    }

    int GetBranchCount()
    {
        return parentScript.NumBranchesPerTrunk;
    }

    //-----------------------
    Vector3 GetBranchStartPoint(Vector3[,] branchMatrix, int reflIndex)
    {
        return branchMatrix[reflIndex, 0];
    }

    void MakeBranchChild(Vector3 startPoint)
    {
        Instantiate(_branchChild, startPoint, Quaternion.identity, transform);//child to this gameObject
    }
    //----------------------


    //-----------------------
    Vector3[] GetLineToPass(Vector3[,] bMatrix, int reflIndex)
    {
        Vector3[] lineContainer = new Vector3[bMatrix.GetLength(1)];
        for (int points = 0; points < bMatrix.GetLength(1); points++)
        {
            lineContainer[points] = bMatrix[reflIndex, points];
        }
        return lineContainer;
    }

    void PassBranchChildPoints(int childCount, Vector3[] lineToPass)
    {
        transform.GetChild(childCount).gameObject.GetComponent<BranchLR>().GetPoints(lineToPass);
    }
    //----------------------------



    /// <summary>
    /// Above is the ETL Point Logic.
    /// Below is the Lerp Logic.
    /// </summary>



    //Here is: a system that sends two points into SlerpLine() to create points between them.

    int currPoint = 0;
    void SendPointsToLR(int plottedPointCount)//Indiscriminate of order Version.
    {
        currPoint++;
        if (currPoint < myLine.Length)
        {
            SlerpLine(myLine[currPoint - 1], myLine[currPoint], plottedPointCount);
        }
        else
        {
            Debug.Log("Done calling SendPointsToLR()");
        }
    }

    Vector3 increment;

    void SlerpLine(Vector3 currPoint, Vector3 declaredPoint, int curPositionIndex)
    {
        float distance = Vector3.Distance(currPoint, declaredPoint);
        float distanceCovered = 0;
        float fracJourney = 0;

        while (fracJourney < 1)
        {
            distanceCovered += 1/distance;
            fracJourney = distanceCovered / distance;
            increment = Vector3.Slerp(currPoint, declaredPoint, fracJourney);
            //rad. connect this to a line renderer.

            myLR.positionCount = curPositionIndex + 1;
            myLR.SetPosition(curPositionIndex, increment);
            curPositionIndex++;
        }
        SendPointsToLR(curPositionIndex);
    }
}