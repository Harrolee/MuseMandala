using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSource : MonoBehaviour
{
    /*
     * After 6/26:
     * 
     * 
    public ScriptableObject lineParams;
    int reflections;
    int sections;
    *
    *       make sections a list within lineParams. Get the number of sections from there
    * 
    *        calculate pointsPerSection by summing the # of points expected for each section.
    List<int> pointsPerSection;
    */






    int numPointsPerLine = 15;
    LineRenderer lineRend;
    public int ReflectionCount = 4;
    List<Vector3[]> sectionReflections;
    List<LineRenderer> allLines = new List<LineRenderer>();
    void Awake()
    {       //center by force. For later, consider whether there is anything preventing this from working when not at origin.
        gameObject.transform.position = Vector3.zero;
        //put on object with a lineRenderer

        //prepare lineRend to carry the points
        lineRend = GetComponent<LineRenderer>();
        lineRend.SetPosition(0, transform.position);
        lineRend.positionCount = numPointsPerLine;

        //when the SO is implemented, reference data from SO
        /*
        reflections = lineParams.Reflections;
        sections = lineParams.Sections;
        */


    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateMandala();

    }

    void GenerateMandala()
    {
        //LineRenderer line = new LineRenderer();
        //for 6/26 demo, all sections will have the same number of points and there will be 7 sections.
        for (int i = 0; i < 7; i++)
        {
            //logic of mandala section generation

            //put points in linerenderer           
            //let those points generate their own corners and end caps.
            lineRend.SetPositions(MakePoints());

            //reflectLine returns a list of lines.
            sectionReflections = ReflectLine(lineRend);

            //line.SetPositions(sectionReflections[0]);
            //now, we put these in as many lines as we have number of reflected lines.
            for (int ii = 0; ii < ReflectionCount; ii++)
            {
                //line.SetPositions(sectionReflections[ii]); //this won't work if c# only points to objects within a list. Let's find out.
                lineRend.SetPositions(sectionReflections[ii]);  

                allLines.Add(lineRend); //NullReferenceException: Object reference not set to an instance of an object
            }
            Debug.Log("section" + i + " has " + allLines.Count + " reflections");
            //this works.           //end caps and curves aren't in there yet, but then again, I never set that parameter on the line renderer.

            //finally, we need to lerp every line into existence.
            //before we do that, I would like to render every line at the same time.
            //Do 6/23
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


    //reflect ListOfPoints reflectionCount times
    List<Vector3[]> ReflectLine(LineRenderer line)
    {
        //extract vectorlist from line
        Vector3[] v3Arr = new Vector3[line.positionCount];
         line.GetPositions(v3Arr);
        //v3Arr now has all the points that line has.

        //transform original v3Arr and store transformations in fullSection
        List<Vector3[]> fullSection = new List<Vector3[]>();
        Vector3[] newRef = new Vector3[numPointsPerLine];
        int theta;
        Vector3 dummy3;
        for (int ii = 1; ii < ReflectionCount +1; ii++)
        {
            //traverse line. For every vertex in line, create a reflection of that vertex.
            for (int jj = 0; jj < v3Arr.Length-1; jj++)
            {
                theta = 360 / ReflectionCount;
                dummy3 = new Vector3(Mathf.Cos(theta * ii), Mathf.Sin(theta * ii), 0);
                newRef[jj] = Vector3.Reflect(v3Arr[jj], dummy3);
                //this is making the points and populating array newRef with them. After one traversal, newRef contains all new points for a new reflection.
            }
            fullSection.Add(newRef);

        }
        //returns a list of reflected lines
        return fullSection;
    }

}
//things not covered:
    /*
     *end of section need correspond with start of next section
     *      possible fix: generate a circle between each section.
     */