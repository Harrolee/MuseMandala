﻿using System.Collections.Generic;
using UnityEngine;
using Mandala;

public class LineSource : MonoBehaviour
{
    LineParametersSO lineParams;
    MGMT MGMT;

    Vector3[] squarePoints = new Vector3[4];

    #region Import Vars from LineParameters SO
    [Range(.1f, 4)]
    public float pointDensity = 1;
    public float speed = 1;//move to SO
    [HideInInspector]
    public int numPointsPerLine;
    int NumLinesPerSection;
    [HideInInspector]
    public int NumBranchesPerTrunk;
    [HideInInspector]
    public int PointsPerBranch;
    #endregion

    LineRenderer sourceLine;
    Vector3[] branchStartPoints;
    //the reflections of sourceLine in each section.
    Vector3[,] branchMatrix;
    public Vector3[,] sectionReflections;
    //public List<LineRenderer> allLines = new List<LineRenderer>();//why tf is this a list of LineRenderers?--After 6/26
    //public List<Vector3[]> allLines = new List<Vector3[]>();
    public int branchPointBuffer;
    //branchPointBuffer is the number of points between each branch start point.

    ColorSwatch colorSwatch;
    Vector3[] endPoints;

    public List<Vector3[,]> branchMatrixList;
    //instead of making this static, send a message to all the children who want branches with this in it.
    //or, make a messaging bus.
    //place an instance of branchMatrixList into the bus. Send a message to the kids to grab the instance they need.
    
    
     void ImportLineParams()
     {
         NumLinesPerSection = lineParams.Reflections;
         //sectionCount = lineParams.Sections;
         NumBranchesPerTrunk = lineParams.Branches;
         numPointsPerLine = lineParams.PointsPerSection[0];
         PointsPerBranch = lineParams.PointsPerBranch;
     }
     
    void Awake()
    {       //center by force. For later, consider whether there is anything preventing this from working when not at origin.
        gameObject.transform.position = Vector3.zero;



        //set reference to singleton.
        MGMT = GameObject.FindGameObjectWithTag("MGMT").GetComponent<MGMT>();

        //set reference to MandalaParameters
        lineParams = MGMT.MandalaParams;
        ImportLineParams();

        //put on object with a lineRenderer
        //prepare lineRend to carry the points
        sourceLine = GetComponent<LineRenderer>();
        sourceLine.SetPosition(0, transform.position);
        sourceLine.positionCount = numPointsPerLine;
        sourceLine.numCapVertices = 3;
        sourceLine.numCornerVertices = 3;

        PickColorSwatch();

        branchStartPoints = new Vector3[NumBranchesPerTrunk];

    }

    //each run, randomly select a color theme.
    void PickColorSwatch()
    {
        colorSwatch = MGMT.ColorSwatches[Random.Range(0, MGMT.ColorSwatches.Count)];
    }

    void GenerateSecondSection()
    {
        Vector3[] newLine = Patterns.Sin(numPointsPerLine);
    }

    public void GenerateSection()
    {
        //Create a source line for each section.
        Vector3[] line = Patterns.Diagonal(numPointsPerLine);

        //This sets line to start at the end of the last line.
        //Vector3[] endpoints is filled with the endpoints of
        //the line generated each pass.
        if (!(endPoints == null))
        {
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
                   

        sectionReflections = ReflectLine(line, NumLinesPerSection, numPointsPerLine);

        //Save value farthest from center.
        //There may be a case where the farthest point
        //differs from the endpoints.
        //List<Vector3> farthestPoints = new List<Vector3>();
        //farthestPoints = GetFarthestPoints(farthestPoints);
        
        //Save final value in each array.
        endPoints = new Vector3[NumLinesPerSection];
        endPoints = GetEndpoints(endPoints);

        MakeBoundaries(endPoints, sectionReflections);

        //GenerateBackground(endPoints);

    }

    //This section will remain expanded until I
    //Move the Circles here by changing their transform.
    void MakeBoundaries(Vector3[] endPoints, Vector3[,] sectionReflections)
    {
        //innermost circle
        Boundaries.PlaceCircle(sectionReflections[0, 7], sectionReflections[2, 7], lineParams.BoundaryWidth, lineParams.BoundaryWidth);

        //innermost square
        for (int ii = 0; ii < 4; ii++)
        {
            squarePoints[ii] = sectionReflections[ii, 7];
        }
        Boundaries.PlaceSquare(squarePoints, lineParams.BoundaryWidth, lineParams.BoundaryWidth);

        //Three concentric squares:
        //one will be a gate, eventually.
        int col = 4;
        for (int z = 1; z < 4; z++)
        {
            for (int ii = 0; ii < 4; ii++)
            {
                squarePoints[ii] = sectionReflections[ii, sectionReflections.GetLength(1) - col];
                squarePoints[ii].z -= z;
            }
            Boundaries.PlaceSquare(squarePoints, lineParams.BoundaryWidth, lineParams.BoundaryWidth);
            col--;
        }
        //outermost square. Use endpoints rather than sectionReflections.
        //note that endPoints lie within sectionReflections.
        for (int ii = 0; ii < 4; ii++)
        {
            squarePoints[ii] = endPoints[ii];
            squarePoints[ii].z -= 4;
        }
        Boundaries.PlaceSquare(squarePoints, lineParams.BoundaryWidth, lineParams.BoundaryWidth);

        //four outermost concentric circles:
        //4 is the offset at which the squares left off.
        //int z = -4;
        for (int XYoffset = 0; XYoffset < 4; XYoffset++)
        {
            Boundaries.PlaceCircle(endPoints[0] - new Vector3(XYoffset, XYoffset, 0), endPoints[2] + new Vector3(XYoffset, XYoffset, 0), lineParams.BoundaryWidth, lineParams.BoundaryWidth);
            //z--;
        }
        
        //At this point, the boundaries have been placed.
        //give Materials and set color for Circles and squares:
        GameObject[] circles = GameObject.FindGameObjectsWithTag("circle");
        GameObject[] squares = GameObject.FindGameObjectsWithTag("square");
        AssignBoundaryMats(circles, squares);

        //set width, offset, and tiling
        ConfigureBoundaryCostumes(circles, squares);

        //Order when each boundary appears
        Material[] boundaryMats = OrderBoundaries(circles, squares);

        //Reveal the Boundaries
        StartCoroutine(MGMT.CentralLoop(boundaryMats, MGMT._CenterPieceMat, MGMT.SectionSeconds, lineParams.Sections, squares, circles));
        //Reveal the Background triangles
        SetBackgroundTriangles(MGMT.BackgroundTriangles, endPoints);
    }

    #region BoundaryConfig Methods
    void ConfigureBoundaryCostumes(GameObject[] circles, GameObject[] squares)
    {
        //squares[0]
        squares[0].GetComponent<LineRenderer>().material = MGMT.MandalaParams.Materials[2];//thunder
        squares[0].GetComponent<LineRenderer>().startWidth = 2f;
        squares[0].GetComponent<LineRenderer>().endWidth = 2f;
        squares[0].GetComponent<LineRenderer>().material.SetFloat("_TilingX", 0.249f);
        squares[0].GetComponent<LineRenderer>().material.SetFloat("_OffsetX", .39f);

        //squares[1]
        squares[1].GetComponent<LineRenderer>().material.SetFloat("_TilingX", .28f);
        squares[1].GetComponent<LineRenderer>().material.SetFloat("_OffsetX", .7f);
        squares[1].GetComponent<LineRenderer>().startWidth = 3;
        squares[1].GetComponent<LineRenderer>().endWidth = 3;
        squares[1].transform.localScale = new Vector3(.85f, .85f, .85f);

        //portraits
        squares[2].GetComponent<LineRenderer>().material = MGMT.MandalaParams.Materials[1];//opaque ribbon
        squares[2].GetComponent<LineRenderer>().material.SetFloat("_TilingX", .28f);
        squares[2].GetComponent<LineRenderer>().startWidth = 3;
        squares[2].GetComponent<LineRenderer>().endWidth = 3;
        squares[2].GetComponent<LineRenderer>().material.SetFloat("_TilingX", 0.22f);

        //squares 3 and 4:
        //make ginormous and animate!
        squares[3].GetComponent<LineRenderer>().startWidth = 7;
        squares[3].GetComponent<LineRenderer>().endWidth = 7;
        squares[3].GetComponent<LineRenderer>().material.SetFloat("_TilingX", 0.13f);
        

        squares[4].GetComponent<LineRenderer>().startWidth = 7;
        squares[4].GetComponent<LineRenderer>().endWidth = 7;
        squares[4].GetComponent<LineRenderer>().material.SetFloat("_TilingX", 0.13f);
        squares[4].GetComponent<LineRenderer>().material.SetFloat("_TilingY", 10f);
        squares[4].transform.localScale = new Vector3(.85f, .85f, .85f);

        //render this circle before the first square
        circles[0].GetComponent<LineRenderer>().material = MGMT.MandalaParams.Materials[3];//inner circle
        circles[0].GetComponent<LineRenderer>().startWidth = .2f;
        circles[0].GetComponent<LineRenderer>().endWidth = .2f;
        circles[0].transform.localScale = new Vector3(.56f, .56f, .56f);

        //assign filigree
        circles[1].GetComponent<LineRenderer>().material.SetTexture("_Texture2D", MGMT.MandalaParams.AlphaTextures[8]);
        circles[1].transform.position += new Vector3(0, 0, -6);
        circles[1].GetComponent<LineRenderer>().startWidth = 2;
        circles[1].GetComponent<LineRenderer>().endWidth = 2;
        circles[1].GetComponent<LineRenderer>().material.SetFloat("_TilingX", .1f);

        circles[2].transform.position += new Vector3(0, 0, -8);
        circles[2].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);//
        circles[2].GetComponent<LineRenderer>().startWidth = 12;
        circles[2].GetComponent<LineRenderer>().endWidth = 12;
        circles[2].GetComponent<LineRenderer>().material.SetFloat("_TilingX", .09f);

        circles[3].transform.position += new Vector3(0, 0, -11);
        circles[3].transform.localScale = new Vector3(1.49f, 1.49f, 1.49f);//
        circles[3].GetComponent<LineRenderer>().startWidth = 12f;
        circles[3].GetComponent<LineRenderer>().endWidth = 12f;
        circles[3].GetComponent<LineRenderer>().material.SetFloat("_TilingX", .09f);

        circles[4].transform.position += new Vector3(0, 0, -15);
        circles[4].transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);//
        circles[4].GetComponent<LineRenderer>().startWidth = 13;
        circles[4].GetComponent<LineRenderer>().endWidth = 13;
        circles[4].GetComponent<LineRenderer>().material.SetFloat("_TilingX", .37f);
        circles[4].GetComponent<LineRenderer>().material.SetFloat("_TilingY", .95f);
    }

    Material[] OrderBoundaries(GameObject[] circles, GameObject[] squares)
    {
        Material[] boundaryMats = new Material[circles.Length + squares.Length];

        //first circle should render quickly and then shimmer.
        boundaryMats[0] = circles[0].GetComponent<LineRenderer>().material;

        boundaryMats[1] = squares[0].GetComponent<LineRenderer>().material;

        //These squares box in the 4 triangles.
        boundaryMats[2] = squares[1].GetComponent<LineRenderer>().material;
        //Use an opaque texture for this square.
        boundaryMats[3] = squares[2].GetComponent<LineRenderer>().material;
        boundaryMats[4] = squares[3].GetComponent<LineRenderer>().material;
        //5th square, the square below this line, should actually be a gate.
        boundaryMats[5] = squares[4].GetComponent<LineRenderer>().material;
        //Only 3 rings in sample mandala. 
        //Lotus
        boundaryMats[6] = circles[1].GetComponent<LineRenderer>().material;
        //Diamond/Thunder
        boundaryMats[7] = circles[2].GetComponent<LineRenderer>().material;
        //Fire
        boundaryMats[8] = circles[3].GetComponent<LineRenderer>().material;
        boundaryMats[9] = circles[4].GetComponent<LineRenderer>().material;

        return boundaryMats;
    }

    void SetBackgroundTriangles(LineRenderer[] lrArray, Vector3[] endPoints)
    {

        float zEndPoint = 5;
        float zStartPoint = 10;
        Vector3[] avg = new Vector3[endPoints.Length];
        //first section of averages
        for (int ii = 1; ii < avg.Length; ii++)
        {
            avg[ii-1] = (endPoints[ii] + endPoints[ii - 1]) * .5f;
        }
        //last average
        avg[avg.Length-1] = (endPoints[0] + endPoints[endPoints.Length-1]) * .5f;

        //move avg[] to the background
        for (int i = 0; i < avg.Length; i++)
        {
            avg[i].z -= zEndPoint;
        }

        //prep for lerping. Includes:
        //place backgroundLR points
        //set their width
        for (int ii = 0; ii < lrArray.Length; ii++)
        {
            lrArray[ii].startWidth = .1f;
            lrArray[ii].SetPosition(0, new Vector3(0,0,zStartPoint));
            lrArray[ii].SetPosition(1, avg[ii]);
            lrArray[ii].endWidth = Vector3.Distance(endPoints[0], endPoints[1]);

            //lerp material in for each
            lrArray[ii].material.SetColor("_Color", MGMT.BackgroundPalletes[Random.Range(0, MGMT.BackgroundPalletes.Count)].colors[ii].color);
           // lrArray[ii].material.SetFloat("_Alpha", 0);
            StartCoroutine(Effects.LerpMatOverTime(lrArray[ii].material, "_Alpha", 1, 0, MGMT.TotalSeconds/7));
        }
    }

    void AssignBoundaryMats(GameObject[] circles, GameObject[] squares)
    {
        Material mat;
        //circles
        for (int ii = 0; ii < circles.Length; ii++)
        {
            mat = MGMT.CircleMat;
            circles[ii].GetComponent<LineRenderer>().material = mat;
            circles[ii].GetComponent<LineRenderer>().material.SetColor("_Color", colorSwatch.colors[ii].color);
            circles[ii].GetComponent<LineRenderer>().material.SetTexture("_Texture2D", MGMT.MandalaParams.AlphaTextures[ii]);

        }
        //squares
        for (int ii = 0; ii < squares.Length; ii++)
        {
            mat = MGMT.SquareMat;
            squares[ii].GetComponent<LineRenderer>().material = mat;
            squares[ii].GetComponent<LineRenderer>().material.SetColor("_Color", colorSwatch.colors[ii].color);
            squares[ii].GetComponent<LineRenderer>().material.SetTexture("_Texture2D", MGMT.MandalaParams.AlphaTextures[ii]);
        }
    }

    #endregion

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
            //print("endpoints are " + endPoints[row]);
        }
        return endPoints;
    }

    #region Line and Branch Generation Methods
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
                //print(branchCount+" inverseNewBranch" + inverseNewBranch[branchCount]);

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
    #endregion


}