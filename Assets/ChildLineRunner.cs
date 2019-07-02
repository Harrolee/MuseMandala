using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildLineRunner : MonoBehaviour//LineSource
{
    int speed = 1;
    int lineCounter =0;
    LineRenderer myLR;
    public List<Vector3[]> mySectionReflections;
    int numPointsPerLine;
    Vector3[] myLine = new Vector3[15];
    int myIndex;
    LineSource parentScript;
    public GameObject _ChildLineRenderer;


    void Awake()
    {       //center by force. For later, consider whether there is anything preventing this from working when not at origin.
        gameObject.transform.position = Vector3.zero;
        //put on object with a lineRenderer

        myLR = GetComponent<LineRenderer>();
        //set position to origin for now.
        myLR.SetPosition(0, transform.position);
        myLR.numCapVertices = 3;
        myLR.numCornerVertices = 3;

        //when the SO is implemented, reference data from SO
        /*
        reflections = lineParams.Reflections;
        sections = lineParams.Sections;
        */
        parentScript = transform.parent.GetComponent<LineSource>();
        numPointsPerLine = parentScript.numPointsPerLine;


        if (_ChildLineRenderer == null)
            _ChildLineRenderer = parentScript._ChildLineRenderer;

        myIndex = transform.GetSiblingIndex();
    }

    public void StartLineRender(Vector3[] myLine)
    {

        this.myLine = myLine;
        SendPointsToLR();


        //render point array.
    }
       
    int counter = 0;
    //here is--> a system that sends the current point and the next point into StartCoroutine(SlerpLine(______))

        //this is not repeating. See what happens when it is called.
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
}


