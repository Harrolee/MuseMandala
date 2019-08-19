using System.Collections.Generic;
using UnityEngine;
using Mandala;

public class LineSource : MonoBehaviour
{
    public LineParametersSO lineParams;




    [Range(.1f, 4)]
    public float pointDensity = 1;
    public float speed = 1;//move to SO
    public int numPointsPerLine = 15;
    LineRenderer sourceLine;
    public int NumLinesPerSection = 4;
    public int NumBranchesPerTrunk = 2;
    public int PointsPerBranch = 4;
    Vector3[] branchStartPoints;
    //the reflections of sourceLine in each section.
    Vector3[,] branchMatrix;
    public Vector3[,] sectionReflections;
    //public List<LineRenderer> allLines = new List<LineRenderer>();//why tf is this a list of LineRenderers?--After 6/26
    //public List<Vector3[]> allLines = new List<Vector3[]>();
    public int branchPointBuffer;
    //branchPointBuffer is the number of points between each branch start point.

    Vector3[] endPoints;

    public List<Vector3[,]> branchMatrixList;
    //instead of making this static, send a message to all the children who want branches with this in it.
    //or, make a messaging bus.
    //place an instance of branchMatrixList into the bus. Send a message to the kids to grab the instance they need.
    
    /*
     void ImportSO()
     {
         NumLinesPerSection = lineParams.Reflections;
         sectionCount = lineParams.Sections;
         NumBranchesPerTrunk = lineParams.Branches;
        // numPointsPerLine = lineParams.PointsPerSection;
     }
     */
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

        //ImportSO();
        //when the SO is implemented, reference data from SO
        /*

        */

        branchStartPoints = new Vector3[NumBranchesPerTrunk];
    }


    void GenerateSecondSection()
    {
        Vector3[] newLine = Patterns.Sin(numPointsPerLine);
    }

    public void GenerateSection()
    {
        //Create a source line for each section.
        Vector3[] line = Patterns.Square(numPointsPerLine);

        if (!(endPoints == null))
        {
            //add startPoints:
            for (int i = 0; i < line.Length; i++)
            {
                line[i] += endPoints[0];
            }
        }

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
        branchMatrixList = GenerateBranchList(branchMatrix);
            //when it comes to rendering branches, I will have to render all of the rows of each matrix at the same time.
                    //the List datatype supplies an index for when each set of rows has to begin rendering.
                            //the system that cues the branch of each line to generate will select the matrix to generate one set of branches.
        
        //The list<> index
        //cued by a check within the lerp function that looks for a specific point.
           //the function that passes declared points into the lerp function needs the point to check for.
                //it only needs to know the number of points between each branch(numGap).
                    //iterate on each point
                        //when the iterater meets numGap, reset the iterator and cue the next branch in branchMatrixList.

                   

        sectionReflections = ReflectLine(line, NumLinesPerSection, numPointsPerLine);

        //Save value farthest from center.
        List<Vector3> farthestPoints = new List<Vector3>();
        farthestPoints = GetFarthestPoints(farthestPoints);
        

        //Save final value in each array.
        endPoints = new Vector3[NumLinesPerSection];
        endPoints = GetEndpoints(endPoints);

        RenderCircle(endPoints);
        RenderSquare(endPoints);
    }
    
    void RenderSquare(Vector3[] endpoints)
    {
        //Debug.LogFormat("endpoints are: {0},   {1},   {2}", endPoints[0], endPoints[1], endPoints[2]);
        GameObject square = new GameObject("square");
        square.AddComponent<LineRenderer>();
        LineRenderer squareLR = square.GetComponent<LineRenderer>();
        squareLR.useWorldSpace = false;
        squareLR.positionCount = 5;
        squareLR.SetPositions(Boundaries.MakeSquare(endpoints, Vector3.Distance(endPoints[0], endPoints[1])));
    }

    void RenderCircle(Vector3[] endpoints)
    {
        GameObject square = new GameObject("circle");
        square.AddComponent<LineRenderer>();
        LineRenderer squareLR = square.GetComponent<LineRenderer>();
        Vector3[] circlePoints = Boundaries.MakeCircle(endpoints[0], endpoints[2], Vector3.Distance(endpoints[0], endpoints[2]));
        print("5th circle point: " + circlePoints[5]);
        squareLR.useWorldSpace = false;
        squareLR.positionCount = circlePoints.Length;
        squareLR.SetPositions(circlePoints);
    }

    List<Vector3> GetFarthestPoints(List<Vector3> farthestPoints)
    {
        //empty farthestPoints and add (0,0,0) as the first value.
        farthestPoints.Clear();
        farthestPoints.Add(Vector3.zero);

        float maxlistPD = 0;
        float currlistPD = 0;
        float currPointDistance = 0;
        
        //will only be one maxDistance.
        float maxDistance = 0;
        //will be as many FarthestPoints as there are lines in this section.

            //determine max distance.
            for (int col = 0; col < sectionReflections.GetLength(1); col++)
            {
                //set current max distance of list.
                for (int ii = 0; ii < farthestPoints.Count; ii++)
                {
                    currlistPD = Mathf.Abs(Vector3.Distance(Vector3.zero, farthestPoints[ii]));
                    //traverse entire list. Find max distance of the list. Compare list max distance to currPointDistance. If cPD is greater, clear the list and add the new point.
                    if (currlistPD > maxlistPD)
                    {
                        maxlistPD = currlistPD;
                       
                    }
                }

                currPointDistance = Mathf.Abs(Vector3.Distance(Vector3.zero, sectionReflections[0, col]));
                if (maxlistPD < currPointDistance)
                {
                    farthestPoints.Clear();
                    farthestPoints.Add(sectionReflections[0, col]);
                }
                else if (maxlistPD == currPointDistance)
                {
                    farthestPoints.Add(sectionReflections[0, col]);
                }
        }
            maxDistance = maxlistPD;

        //at this point, list<Vector3> farthestPoints has one value. It has the point of the first line that is farthest from the center.                   \
        //Given that all succeeding lines are reflections of the first line, we need only check the maxDistance found in the first line against the points  \
        //of the second line in order to determine which points ought to enter the farthestPoints list.                                                     \

        for (int row = 1; row < sectionReflections.GetLength(0); row++)
        {
            for (int col = 0; col < sectionReflections.GetLength(1); col++)
            {
                currPointDistance = Mathf.Abs(Vector3.Distance(Vector3.zero, sectionReflections[row, col]));
                if (currPointDistance > maxDistance)
                {
                    farthestPoints.Add(sectionReflections[row, col]);
                    //print("new FP member: " +sectionReflections[row, col]);
                }
            }
        }
        print("fp count" + farthestPoints.Count);
        return farthestPoints;
    }

    Vector3[] GetEndpoints(Vector3[] endPoints)
    {
        int lastCol = sectionReflections.GetLength(1) - 1;
        for (int row = 0; row < sectionReflections.GetLength(0); row++)
        {
            endPoints[row] = sectionReflections[row, lastCol];
            print("endpoints are " + endPoints[row]);
        }
        return endPoints;
    }

    //
    Vector3[] MakeBranchPoints(int numPoints, Vector3[] startPoints)
    {
        //add logic to zfunction to apply depth to the mandala
        float sizeCoeff = 1.5f;
        float zfunction = 0;
        float y = 0;
        Vector3[] pointsArr = new Vector3[numPoints];

        //I think i=1 because index 0 has to be vector3.zero
        for (int i = 1; i < numPoints; i++)
        {
            y = Mathf.Sin(i);
            pointsArr[i] = new Vector3(-i, (y + i) * sizeCoeff, zfunction);
        }
        return pointsArr;
    }

    //Starts Main Logic Method on children of LineSource. Begins Trunk Generation.
    public void CallRender()
    {
        Vector3[] arrayToPass = new Vector3[numPointsPerLine];
        //extract one line for each row of the multidimensional array
        for (int ii = 0; ii < NumLinesPerSection; ii++)
        {
            for (int jj = 0; jj < numPointsPerLine; jj++)
            {
                arrayToPass[jj] = sectionReflections[ii, jj];
                arrayToPass[jj] = sectionReflections[ii, jj];
            }
            //pass a different line to each child.
           transform.GetChild(ii).GetComponent<ChildLineRunner>().StartLineRender(arrayToPass);
           //temp comment
        }
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

        //where is the number of times this method will reflect a line?

        float theta;
        Vector3 normal;
        theta = 360 / rowCount;
        theta = theta * Mathf.Deg2Rad;


        Vector3 crossProdUpRight = Vector3.Cross(Vector3.up, Vector3.right);
        Vector3 cPURinverse = Vector3.Scale(crossProdUpRight, new Vector3(-1, -1, -1));
        Vector3 temp3;

        //outer loop is the number of reflections.
        //currently, ReflectLine supports only up to four reflections.
        //Further into the project, please automate this pattern so
        //that it might scale automatically.
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
        }
        //we need this value to track when to place branches.
        return startPointIterator;
    }

    //takes the starting points of a branch and the length of every branch
    //returns a matrix of branches whose positions are set relative to points on each trunk.
    Vector3[,] MakeBranches(Vector3[] startPoints, int branchPointCount)
    {
        Vector3[,] branchMatrix = new Vector3[startPoints.Length, branchPointCount];


        //pass startPoints into MakeBranchPoints. remove startPoints from the plotting section.
        Vector3[] newBranch = MakeBranchPoints(branchPointCount, startPoints);
        Vector3[] inverseNewBranch = new Vector3[newBranch.Length];

        //Each iteration creates points for one branch.
        for (int branchCount = 0; branchCount < startPoints.Length; branchCount++)
        {
            if (branchCount % 2 == 0)
            {
                print("ran");
                //each iteration creates one point for one branch
                for (int column = 0; column < branchPointCount; column++)
                {
                    //add starting point of branchMatrix to every value in array returned by MakePoints.
                    branchMatrix[branchCount, column] = newBranch[column] + startPoints[branchCount]; //there won't always be as many startPoints as columns
                }
            }
            else
            {
                inverseNewBranch = InvertLine(newBranch, startPoints[branchCount]);
                print(branchCount+" inverseNewBranch" + inverseNewBranch[branchCount]);
                print("i ran");
                for (int column = 0; column < branchPointCount; column++)
                {
                    //adding the starting point locates the branch on the trunk.
                    //add starting point of branchMatrix to every value in array returned by MakePoints.
                    branchMatrix[branchCount, column] = inverseNewBranch[column] + startPoints[branchCount];
                }
                    
                
            }
        }    

        return branchMatrix;
    }   //this contains all the branches of the source line.

    Vector3[] InvertLine(Vector3[] line, Vector3 normal)
    {
        normal = new Vector3(1, 1, 0);
        //Vector3 newPoint;
        Vector3[] reflectedLine = new Vector3[line.Length];
        for (int point = 0; point < line.Length; point++)
        {
            reflectedLine[point] = Vector3.Reflect(line[point], normal);
        }
        return reflectedLine;
    }

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
            branchMatrixList.Add(ReflectLine(tempBranch, NumLinesPerSection, branchMatrixCols));
        }
        return branchMatrixList;
        //There are as many entries in branchMatrixList as there are branches for each trunk.
    }


}