using UnityEngine;
using System.Collections.Generic;

public class LineSource : MonoBehaviour
{

    public static int sectionCount = 1;
    public float speed = 1;//move to SO
    public int numPointsPerLine = 15;
    LineRenderer sourceLine;
    public GameObject _ChildLineRenderer;
    public int NumLinesPerSection = 4;
    public int NumBranchesPerTrunk = 2;
    public int PointsPerBranch = 3;
    Vector3[] branchStartPoints;
    //the reflections of sourceLine in each section.
    Vector3[,] branchMatrix;
    public Vector3[,] sectionReflections;
    //public List<LineRenderer> allLines = new List<LineRenderer>();//why tf is this a list of LineRenderers?--After 6/26
    //public List<Vector3[]> allLines = new List<Vector3[]>();
    public int branchPointBuffer;
        //branchPointBuffer is the number of points between each branch start point.


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

        branchStartPoints = new Vector3[NumBranchesPerTrunk];
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
        Vector3[] line = MakePoints(numPointsPerLine);
        //for 6/26 demo, all sections will have the same number of points and there will be 7 sections.
        //this gives us a source line for each section.

        //branchPointBuffer is the number of points between each branch start point.
        branchPointBuffer = MakeBranchStartPoints(line); //divide line length by 2 NumBranchesPerTrunk times.

        //branchMatrix schema:    --------------------
            //rows are branches                     //
            //columns are points on the branch      //
            //--first column is the starting point--//

        //pass in an array of branch start points from the original line.
        //Get a matrix of all the branches at the positions of the original line.
        branchMatrix = MakeBranches(branchStartPoints, PointsPerBranch);

        //Each Vector3[] in List is a set of reflections for one branch on the trunk.
        List<Vector3[,]> branchMatrixList = GenerateBranchList(branchMatrix);
            //when it comes to rendering branches, I will have to render all of the rows of each matrix at the same time.
                    //the List datatype supplies an index for when each set of rows has to begin rendering.
                            //the system that cues the branch of each line to generate will select the matrix to generate one set of branches.





        //The list<> index
        //cued by a check within the lerp function that looks for a specific point.
           //the function that passes declared points into the lerp function needs the point to check for.
                //it only needs to know the number of points between each branch(numGap).
                    //iterate on each point
                        //when the iterater meets numGap, reset the iterator and cue the next branch in branchMatrixList.

                    

        //On cue, fire a second method.
            //second method lerps the branch at the same time as the trunks.
            //this method selects the next branchMatrix in sequence.

        sectionReflections = ReflectLine(MakePoints(numPointsPerLine), NumLinesPerSection, numPointsPerLine);
    }





    //Starts LineRenderer on children of LineSource.
    void CallRender()
    {
        Vector3[] arrayToPass = new Vector3[numPointsPerLine];
        //extract one line for each row of the multidimensional array
        for (int ii = 0; ii < transform.childCount; ii++)
        {
            for (int jj = 0; jj < numPointsPerLine; jj++)
            {
                arrayToPass[jj] = sectionReflections[ii, jj];
                arrayToPass[jj] = sectionReflections[ii, jj];
            }
            //pass a different line to each child.
           // transform.GetChild(ii).GetComponent<ChildLineRunner>().StartLineRender(arrayToPass);
           //temp comment
        }
    }

    //takes the number of points you want. Returns a Vector3[] containing that number of points.
    Vector3[] MakePoints(int numPoints)
    {
        Vector3[] pointsArr = new Vector3[numPoints];

        //I think i=1 because index 0 has to be vector3.zero
        for (int i = 1; i < numPoints; i++)
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


    //presently ReflectLine supports only 4 reflections.
            //after demo with four, add parameter to take reflection count.
    //returns a list of reflected lines
    //reflect ListOfPoints reflectionCount times
    Vector3[,] ReflectLine(Vector3[] line, int rowCount, int columnCount)
    {
        //row count is number of lines this method will reflect
        //columCount is number of points in each line

        //multi-dimensional array to contain the reflected vector3 arrays.
        Vector3[,] fullSection = new Vector3[rowCount, columnCount];

        float theta;
        Vector3 normal;
        theta = 360 / rowCount;
        theta = theta * Mathf.Deg2Rad;


        Vector3 crossProdUpRight = Vector3.Cross(Vector3.up, Vector3.right);
        Vector3 cPURinverse = Vector3.Scale(crossProdUpRight, new Vector3(-1, -1, -1));
        Vector3 temp3;

        //currently, ReflectLine supports only up to four reflections.
        for (int ii = 1; ii < rowCount + 1 ; ii++)
        {
            if(ii == 1)
            {
                normal = new Vector3(0, 1, 0);
                for (int jj = 0; jj < columnCount; jj++)
                {
                    //reflect the original point across a line generated by the number of reflections desired.
                    fullSection[ii - 1, jj] = Vector3.Reflect(line[jj], normal);
                }
            }
            else if(ii==2) //We want to multiply this one's z by -1
            {
                normal = crossProdUpRight;
                for (int jj = 0; jj < columnCount; jj++)
                {
                    //reflect the original point across a line generated by the number of reflections desired.
                    fullSection[ii - 1, jj] = Vector3.Reflect(line[jj], normal);

                    //z edit:
                    temp3 = fullSection[ii - 1, jj];
                    temp3.z = temp3.z * -1;

                    fullSection[ii - 1, jj] = temp3;
                }
            }
            else if (ii == 3)
            {
                normal = new Vector3(1, 0, 0);
                for (int jj = 0; jj < columnCount; jj++)
                {
                    //reflect the original point across a line generated by the number of reflections desired.
                    fullSection[ii - 1, jj] = Vector3.Reflect(line[jj], normal);
                }
            }
            else if (ii == 4)
            {
                normal = cPURinverse;
                for (int jj = 0; jj < columnCount; jj++)
                {
                    //reflect the original point across a line generated by the number of reflections desired.
                    fullSection[ii - 1, jj] = Vector3.Reflect(line[jj], normal);
                    fullSection[ii - 1, jj] = Vector3.Scale(fullSection[ii - 1, jj], new Vector3(-1, -1, -1));
                }
            }
        }


        //===================This Automatically Generates Trunks===================================
        //for (int ii = 1; ii < NumLinesPerSection + 1; ii++)
        //{
        //    print("theta is " + theta + "ii is" + ii);
        //    print("cosine is " + Mathf.Cos(theta * ii) + "and sin is  " + Mathf.Sin(theta * ii));
        //    //For each row, calculate a new value for theta. This has the effect of reflecting evenly accross the circle.
        //    normal = new Vector3(Mathf.Cos(theta * ii), Mathf.Sin(theta * ii), 0);
        //    for (int jj = 0; jj < numPointsPerLine; jj++)
        //    {
        //        //reflect the original point across a line generated by the number of reflections desired.
        //        fullSection[ii - 1, jj] = Vector3.Reflect(line[jj], normal);
        //    }
        //}
        return fullSection;
    }


    //divide line length by 2. Do it NumBranchesPerTrunk times.
    int MakeBranchStartPoints(Vector3[] line)
    {
        int remainingLineLength = line.Length;
        int startPointIterator = 0;

        //int division
        for (int ii = 0; ii < NumBranchesPerTrunk; ii++)
        {
            remainingLineLength /= 2;
            startPointIterator = remainingLineLength;
        }
        for (int ii = 0; ii < NumBranchesPerTrunk; ii++)
        {
            branchStartPoints[ii] = line[(startPointIterator * (ii + 1))];
            print("Should be " + NumBranchesPerTrunk + " different value(s) for this" + branchStartPoints[ii]);
        }
        //we need this value to track when to place branches.
        return startPointIterator;
    }

    //takes the starting points of a branch and the length of every branch
    //returns a matrix of branches whose positions are set relative to points on each trunk.
    Vector3[,] MakeBranches(Vector3[] startPoints, int branchPointCount)
    {
        print("startPoints length: " + startPoints.Length);//2
        print("branchLength: " + branchPointCount);//3
        Vector3[,] branchMatrix = new Vector3[startPoints.Length, branchPointCount];

        Vector3[] newBranch;

        for (int branchCount = 0; branchCount < startPoints.Length; branchCount++)
        {
            newBranch = MakePoints(branchPointCount);
            for (int column = 0; column < branchPointCount; column++)
            {
                //add starting point of branchMatrix to every value in array returned by MakePoints.
                branchMatrix[branchCount, column] = newBranch[column] + startPoints[branchCount]; //there won't always be as many startPoints as columns
            }
        }    

        return branchMatrix;
    }   //this contains all the branches of the source line.

    List<Vector3[,]> GenerateBranchList(Vector3[,] branchMatrix)
    {
        int branchMatrixRows = branchMatrix.GetLength(0);    //the number of branches in the branch Matrix
        int branchMatrixCols = branchMatrix.GetLength(1);   //the number of points per each branch
        List<Vector3[,]> branchMatrixList = new List<Vector3[,]>();

        //tempBranch carries each row from branchMatrix into ReflectBranches()
        Vector3[] tempBranch = new Vector3[PointsPerBranch];
        for (int row = 0; row < branchMatrixRows; row++)    //why'd I pick branchMatrixRows?
        {
            //extract one row from branchMatrix and store it in tempBranch
            for (int col = 0; col < branchMatrixCols; col++)
            {
                tempBranch[col] = branchMatrix[row, col];
                //                print("should be six" + tempBranch[col]);//   six of these. Should be printed row * col times.
            }
            //ReflectLine reflects the given Vector3[] by a given number of times.
            //ReflectBranches returns an array of branch reflections. Each matrix in branchMatrixList contains reflections of the same original branch.
            //There are as many entries in branchMatrixList as there are branches for each trunk.
            branchMatrixList.Add(ReflectLine(tempBranch, branchMatrixRows, branchMatrixCols));
        }
        print("size of list: " + branchMatrixList.Count);
        return branchMatrixList;
        //There are as many entries in branchMatrixList as there are branches for each trunk.
    }

    void MakeChildLR(LineRenderer parentLR, int numOfChildren)
    {
        for (int i = 0; i < numOfChildren; i++)
        {
            Instantiate(_ChildLineRenderer, parentLR.transform);
        }

    }
}