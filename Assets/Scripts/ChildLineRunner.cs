using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildLineRunner : MonoBehaviour//LineSource
{
    int speed = 1;
    int lineCounter =0;
    //branch cues consists of the points that each branch begins at.
    Vector3[] branchCues;//do i need to instantiate this here?
    Vector3[,] branchPoints;
    int branchCounter = 0;
    LineRenderer myLR;
    public List<Vector3[]> mySectionReflections;
    int numPointsPerLine;
    Vector3[] myLine = new Vector3[15];
    int myIndex;
    LineSource parentScript;
    public GameObject _ChildLineRenderer;
    int branchPointBuffer;
    Vector3[] myBranchStartPositions;

    void Awake()
    {       //center by force. For later, consider whether there is anything preventing this from working when not at origin.
        gameObject.transform.position = Vector3.zero;

        PrepareLR();
        //when the SO is implemented, reference data from SO
        /*
        reflections = lineParams.Reflections;
        sections = lineParams.Sections;
        */
        GetVarsFromParent();

        myIndex = transform.GetSiblingIndex();



        GetBranchPoints()
            //how do I get branch points?

        //spawn as many children as there will be branches.
        CreateBranchGenerators();
    }

    void PrepareLR()
    {
        myLR = GetComponent<LineRenderer>();
        //set position to origin for now.
        myLR.SetPosition(0, transform.position);
        myLR.numCapVertices = 3;
        myLR.numCornerVertices = 3;
    }

    void GetVarsFromParent()
    {
        parentScript = transform.parent.GetComponent<LineSource>();
        numPointsPerLine = parentScript.numPointsPerLine;
        branchPointBuffer = parentScript.branchPointBuffer;
        print("branchPointBuffer= " + branchPointBuffer);

        if (_ChildLineRenderer == null)
            _ChildLineRenderer = parentScript._ChildLineRenderer;
        myBranchStartPositions =
    }

    public void StartLineRender(Vector3[] myLine, Vector3[] bpoints)
    {

        this.myLine = myLine;
        branchCues = bpoints;
        //to test the look of the mandala, we're using PlotPoints() to plot each child's line at the same time.
        PlotPoints();

        //below lies slerper
        //SendPointsToLR();
            //is wonky

        //render point array.
    }
     
    //plots all the points in myLine at the same time.
    void PlotPoints()
    {
        foreach (Vector3 point in myLine)
        {
            myLR.positionCount = lineCounter + 1;
            myLR.SetPosition(lineCounter, point);

            if(lineCounter % branchPointBuffer == 0)
                //render branch.
                    //cue the rendering of a branch.

            lineCounter++;
        }
    }






    int counter = 0;
    //here is--> a system that sends the current point and the next point into StartCoroutine(SlerpLine(______))

    void SendPointsToLR()
    {
        if (counter == numPointsPerLine -1)
        {
            print("done rendering line");
            //could begin next section.
        }
        else
        {
            StartCoroutine(SlerpLine(Time.time, myLine[counter], myLine[counter + 1], speed));
            //check for branchPoints. When one is discovered, render the branch associated with that point.
            if(myLine[counter] == branchCues[branchCounter])
            {
                RenderBranch();
                branchCounter++;
            }
            counter++;
            print("counter " + counter);
        }
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

    void CreateBranchGenerators(int branchTotal)
    {
        for (int branch = 0; branch < branchTotal; branch++)
        {
            Instantiate(_ChildLineRenderer,)
        }
    }


    void RenderBranch()
    {

       // SlerpLine(Time.time, )
    }
}