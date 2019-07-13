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
    }
    //---------------------------------
    //Above is intro logic.
    //Below is Main Controlling Method.

    //Main Logic Method:
    public void StartLineRender(Vector3[] myLine)
    {

        //must get vars after LineSource is finished generating the section of the mandala.
        GetVarsFromParent();
        
        //why is this here? Because this is where Trunks get their lines from root.
        this.myLine = myLine;
   
        //spawn as many children as there will be branches.
        CreateBranchGenerators();
        
        //to test the look of the mandala, we're using PlotPoints() to plot each child's line at the same time.
        PlotPoints();

        //below lies slerper
        //SendPointsToLR();
            //is wonky

        //render point array.
    }


    //Below is ETL Logic.
    //-----------------------------------------------------------------
    void GetVarsFromParent()
    {
        parentScript = transform.parent.GetComponent<LineSource>();
        numPointsPerLine = parentScript.numPointsPerLine;
        branchPointBuffer = parentScript.branchPointBuffer;
        print("branchPointBuffer= " + branchPointBuffer);

        if (_ChildLineRenderer == null)
            _ChildLineRenderer = parentScript._ChildLineRenderer;
    }

    void CreateBranchGenerators()
    {
        int numBranches = GetBranchCount();

        Vector3[,] branchMatrix;

        //Vector3[,] thisReflBranchMatrix = parentScript.branchMatrixList[reflIndex]; //reflIndex exceeds size of Branch Matrix. 
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
    /// Below is the Plot Point Logic.
    /// </summary>



    //plots all the points in myLine at the same time.
    void PlotPoints()
    {
        int branchCue = 0;
        foreach (Vector3 point in myLine)
        {
            myLR.positionCount = lineCounter + 1;
            myLR.SetPosition(lineCounter, point);

            if(lineCounter % branchPointBuffer == 0 && branchCue < 2) //branchPointBuffer is 3 and lineCounter will reach 15. Examine this logic.
            {
                //render branch.
                //cue the rendering of a branch.
                CueBranchLR(branchCue);
                branchCue++;
            }
            lineCounter++;
        }
    }

    void CueBranchLR(int branch)
    {
        //when we're lerping, use the commented one.
        //transform.GetChild(branch).gameObject.GetComponent<BranchLR>().RenderBranch();

        //for now, use this one
        transform.GetChild(branch).gameObject.GetComponent<BranchLR>().PlotPoints();
    }





    /// <summary>
    /// Above is the Plot Point Logic.
    /// Below is the Lerp Logic.
    /// </summary>






    int counter = 0;
    //here is--> a system that sends the current point and the next point into StartCoroutine(SlerpLine(______))

    void SendPointsToLR()
    {
        //if (counter == numPointsPerLine -1)
        //{
        //    print("done rendering line");
        //    //could begin next section.
        //}
        //else
        //{
        //    StartCoroutine(SlerpLine(Time.time, myLine[counter], myLine[counter + 1], speed));
        //    //check for branchPoints. When one is discovered, render the branch associated with that point.
        //    if(myLine[counter] == branchCues[branchCounter])
        //    {
        //        RenderBranch();
        //        branchCounter++;
        //    }
        //    counter++;
        //    print("counter " + counter);
        //}
    }

    Vector3 increment;
    //spherically interpolates a line between the current point and the next point.
    //pass Time.time into this param
    IEnumerator SlerpLine(float startTime, Vector3 currPoint, Vector3 declaredPoint, float speed)
    {
        float distance = Vector3.Distance(currPoint, declaredPoint);
        float distanceCovered = (Time.time - startTime) * speed;
        float fracJourney = distanceCovered / distance;
        print("and");
        while (fracJourney < 1)
        {
            distanceCovered = (Time.time - startTime) * speed;
            fracJourney = distanceCovered / distance;
            print("how far? " + fracJourney);

            //rad. connect this to a line renderer.
            increment = Vector3.Slerp(currPoint, declaredPoint, fracJourney);
            myLR.positionCount = lineCounter + 1;
            myLR.SetPosition(lineCounter, increment);
            lineCounter++;
            yield return new WaitForEndOfFrame();
        }
        //after the line is rendered, say that you are done.
        //this message is needed to spark the next point.
        print("should happen 15 times per reflection. Counter val is: " + counter);
        SendPointsToLR();
    }
    //everyone is getting the same set of points.

    void RenderBranch()
    {

       // SlerpLine(Time.time, )
    }
}