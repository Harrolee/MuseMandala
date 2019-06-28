using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineSource : MonoBehaviour
{

    public static int sectionCount = 1;
    public float speed = 1;//move to SO
    public int numPointsPerLine = 15;
    LineRenderer sourceLine;
    public GameObject _ChildLineRenderer;
    public int NumLinesPerSection = 4;

    //the reflections of sourceLine in each section.
    public List<Vector3[]> sectionReflections;
    //public List<LineRenderer> allLines = new List<LineRenderer>();//why tf is this a list of LineRenderers?--After 6/26
    //public List<Vector3[]> allLines = new List<Vector3[]>();




    void Awake()
    {       //center by force. For later, consider whether there is anything preventing this from working when not at origin.
        gameObject.transform.position = Vector3.zero;
        //put on object with a lineRenderer

        //prepare lineRend to carry the points
        sourceLine = GetComponent<LineRenderer>();
        sourceLine.SetPosition(0, transform.position);
        sourceLine.positionCount = numPointsPerLine;
        sourceLine.numCapVertices = 3;
        sourceLine.numCornerVertices = 3;

        //when the SO is implemented, reference data from SO
        /*
        reflections = lineParams.Reflections;
        sections = lineParams.Sections;
        */

        if (_ChildLineRenderer == null)
            _ChildLineRenderer = gameObject;
    }

    void Start()
    {
        if (sectionCount != 0)
        {
            //create children, one for each Reflection.
            MakeChildLR(sourceLine, NumLinesPerSection);
            sectionCount--;
        }
        else
            print("Made Enough Children");
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            CallRender();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateSection();
            //the following for if children inherit this script
            //if (transform.GetSiblingIndex() == 2 && transform.childCount == 4)
            //{
            //    WholeEnchilada();
            //}
        }
    }


    void GenerateSection()
    {
        //for 6/26 demo, all sections will have the same number of points and there will be 7 sections.
        //this gives us a source line for each section.

        sectionReflections = ReflectLine(MakePoints());
        Debug.Log(sectionReflections.Count + ": if 4, Ready to press :A key:");
    }
    

    void CallRender()
    {

        for (int i = 0; i < transform.childCount; i++)
        {

    //test
            ////each iteration: pull a v3[]  from sectionREflections and check it's 5.
            //Vector3[] newVE3 = sectionReflections[i];
            //print("section" + i);
            //foreach (var item in newVE3)
            //{
            //    print(item);
            //}
            //print("end Section" + i);

            //print("my sib ind is " + transform.GetChild(i).transform.GetSiblingIndex());
            //print( newVE3[5]);
    //end test


            transform.GetChild(i).GetComponent<ChildLineRunner>().StartLineRender(sectionReflections[i]);
        }
    }


    Vector3[] MakePoints()
    {
        Vector3[] pointsArr = new Vector3[numPointsPerLine];

        //I think i=1 because index 0 has to be vector3.zero
        for (int i = 1; i < numPointsPerLine; i++)
        {
            if (i % 7 == 0)
                pointsArr[i] = (pointsArr[i - 1] + new Vector3(2, 1, -1));
            else if (i % 5 == 0)
                pointsArr[i] = (pointsArr[i - 1] + new Vector3(2, 2, -.5f));
            else if (i % 3 == 0)
                pointsArr[i] = (pointsArr[i - 1] + new Vector3(1, 1, -.5f));
            else if (i % 2 == 0)
                pointsArr[i] = (pointsArr[i - 1] + new Vector3(1, 0, -.2f));
            else
                pointsArr[i] = (pointsArr[i - 1] + new Vector3(1, 0));
            //after 6/26 demo, use a prime number function to scale this behaviour to the declared number of points
            //add a coeff to each dimension to scale their tilt
        }

        return pointsArr;
    }


    //returns a list of reflected lines
    //reflect ListOfPoints reflectionCount times
    List<Vector3[]> ReflectLine(Vector3[] line)
    {
        //transform line and store transformations in fullSection
        List<Vector3[]> fullSection = new List<Vector3[]>();

        int theta;
        Vector3 dummy3;
        theta = 360 / NumLinesPerSection;
        for (int ii = 1; ii < NumLinesPerSection + 1; ii++)
        {

            //issue is here, that fullSection count is not populated by anything.
            //This occurs because newRef does not exist outside of this loop.
            //A solution could be:
                //create an empty array of the size of each v3 array in the first loop and then copy point values into it.
                    //this way, the array carrying the new point values is ephemeral and the array which they are passed into persists.

            Vector3[] newRef = new Vector3[numPointsPerLine];
            //print("line # " + ii);
            dummy3 = new Vector3(Mathf.Cos(theta * ii), Mathf.Sin(theta * ii), 0);
            //traverse line. For every vertex in line, create a reflection of that vertex.
            for (int jj = 0; jj < line.Length; jj++)
            {
                //print("traversal: " + jj);
                newRef[jj] = Vector3.Reflect(line[jj], dummy3);
                //this is making the points and populating array newRef with them. After one traversal, newRef contains all new points for a new reflection.
            }
        }
        print("fullSection count is " + fullSection.Count);
        return fullSection;
    }


    void MakeChildLR(LineRenderer parentLR, int numOfChildren)
    {
        for (int i = 0; i < numOfChildren; i++)
        {
            Instantiate(_ChildLineRenderer, parentLR.transform);
        }

    }
}