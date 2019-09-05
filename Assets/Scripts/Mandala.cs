using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mandala
{
    //General is a staging bucket for new functions.
    //When enough new functions pile up, I can form
    //classes out of them.
    public class Utilities
    {
        public static LineRenderer[] MakeLR(GameObject prefab, int numOfChildren, Transform parent)
        {
            LineRenderer[] lrArray= new LineRenderer[numOfChildren];
            for (int ii = 0; ii < numOfChildren; ii++)
                {
                    lrArray[ii] = MonoBehaviour.Instantiate(prefab, parent).GetComponent<LineRenderer>();
                }
            return lrArray;
        }

       //this is becoming the central game loop
       public static IEnumerator RevealBoundaries(Material[] boundaryMats, Material centerpieceMat, List<float> sectionSeconds, int sectionCount)
       {
            //this first section is the centerpiece generation
            float start = 0;
            float end = 1.5f;
            float alphaVal;
            float startTime = Time.time;
            float currTime = Time.time - startTime;
            float x;
            float centerpieceSecs = sectionSeconds[0];
            //7 is the number of sections.
            while (currTime < centerpieceSecs)
            {
                currTime = Time.time - startTime;
                x = currTime / centerpieceSecs;
                alphaVal = Mathf.Lerp(start, end, x);
                centerpieceMat.SetFloat("_Alpha", alphaVal);
                yield return null;
            }
            //====================================----------------------


            //set alpha of all boundaries to 1;
            alphaVal = 1;
            int count = 0;
            foreach (Material mat in boundaryMats)
            {
                mat.SetFloat("_Alpha", alphaVal);
                count++;
            }

            //first ring is quick.
            //After it's appearance, it should perform. 
            //I imagine making a radial wave by changing the width.



            //This is the central experience loop.
            //In sequence, lerp all boundaries.
            start = 1;
            end = 0;
            float boundarySecs = sectionSeconds[1];//totalSecs / boundaryMats.Length;
            int sectionCounter = 0;

            for (int boundary = 0; boundary < boundaryMats.Length; boundary++)
            {
                if (boundary == 1)
                {
                    boundarySecs = 10f;
                }
                startTime = Time.time;
                currTime = Time.time - startTime;
                while (currTime < boundarySecs)
                {
                    currTime = Time.time - startTime;
                    x = currTime / boundarySecs;
                    alphaVal = Mathf.Lerp(start, end, x);
                    boundaryMats[boundary].SetFloat("_Alpha", alphaVal);
                    yield return null;
                }
                //reset timing for next run.
                if (boundary == 1)
                {
                    sectionCounter++;
                    boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
                }
                if (boundary == 5)
                {
                    sectionCount++;
                    boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
                }
                if (boundary == 7)
                {
                    sectionCount++;
                    boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
                }
                if (boundary == 8)
                {
                    sectionCount++;
                    boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
                }
                if (boundary == 9)
                {
                    sectionCount++;
                    boundarySecs = sectionSeconds[sectionCounter];//totalSecs / boundaryMats.Length;
                }
            }


            //Next is the finale: the closing ring dance.
       }
    }

    public class Effects
    {

        //we want this to advance at ten hertz or ten times a second.
        //Every 1/10th second, the background changes.
        public static IEnumerator PingPongLerp(Material mat, string floatToPing, float periodLength)
        {
            float startTime;
            float currTime;
            float x;
            float newVal;
            int counter = 0;
            while(0 < 1)
            {
                startTime = Time.time;
                currTime = Time.time - startTime;

                while (currTime < periodLength * .5)
                {
                    currTime = Time.time - startTime;
                    x = currTime / (periodLength * .5f);
                    newVal = Mathf.Lerp(-1, 1, x);
                    mat.SetFloat(floatToPing, newVal);
                    yield return new WaitForSeconds(.1f);
                }

                startTime = Time.time;
                currTime = Time.time - startTime;

                while (currTime < periodLength * .5)
                {
                    currTime = Time.time - startTime;
                    x = currTime / (periodLength * .5f);
                    newVal = Mathf.Lerp(1, -1, x);
                    mat.SetFloat(floatToPing, newVal);
                    yield return new WaitForSeconds(.1f);
                }
                counter++;
            }
        }

        public static IEnumerator LerpFloatOverTime(Material mat, string floatToLerp, float startVal, float endVal, float totalSecs)
        {
            float alphaVal;
            float startTime = Time.time;
            float currTime = Time.time - startTime;
            float x;
            while (currTime < totalSecs)
            {
                currTime = Time.time - startTime;
                x = currTime / totalSecs;
                alphaVal = Mathf.Lerp(startVal, endVal, x);
                mat.SetFloat(floatToLerp, alphaVal);
                yield return null;
            }
        }
    }
    public class Patterns
    {
        public static Vector3[] Diagonal(int numPoints)
        {
            //add logic to zfunction to apply depth to the mandala
            float zfunction = 0;
            float y = 0;
            Vector3[] pointsArr = new Vector3[numPoints];
            //I think i=1 because index 0 has to be vector3.zero
            for (int i = 1; i < numPoints; i++)
            {
                y = i;
                pointsArr[i] = new Vector3(-i, y, zfunction);
            }
            return pointsArr;
        }

        public static Vector3[] Sin(int numPoints)
        {
            //add logic to zfunction to apply depth to the mandala
            float zfunction = 0;
            float y = 0;
            Vector3[] pointsArr = new Vector3[numPoints];
            //I think i=1 because index 0 has to be vector3.zero
            for (int i = 1; i < numPoints; i++)
            {
                y = Mathf.Sin(i);
                pointsArr[i] = new Vector3(-i, y + i, zfunction);
            }
            return pointsArr;
        }

        public static Vector3[] Tan(int numPoints)
        {
            //add logic to zfunction to apply depth to the mandala
            float zfunction = 0;
            float y = 0;
            Vector3[] pointsArr = new Vector3[numPoints];
            //I think i=1 because index 0 has to be vector3.zero
            for (int i = 1; i < numPoints; i++)
            {
                y = Mathf.Tan(i);
                pointsArr[i] = new Vector3(-i, y + i, zfunction);
            }
            return pointsArr;
        }

        public static Vector3[] Square(int numPoints)
        {
            //add logic to zfunction to apply depth to the mandala
            float zfunction = 0;
            float y = 0;
            Vector3[] pointsArr = new Vector3[numPoints];
            //I think i=1 because index 0 has to be vector3.zero
            for (int x = 1; x < numPoints; x++)
            {
                y = Mathf.Sign(Mathf.Sin(x));
                Debug.Log("sq: " + y);
                pointsArr[x] = new Vector3(-x, y + x, zfunction);
            }
            return pointsArr;
        }
    }

    public class Boundaries
    {
        public static void PlaceSquare(Vector3[] cornerPoints, float startWidth, float endWidth)
        {
            //distance is here in case, in a future iteration 
            //of MakeSquare, I derive a square from two given points.
            //float distance = Vector3.Distance(endpoints[0], endpoints[1]);
            GameObject square = new GameObject("square");
            square.tag = "square";
            square.AddComponent<LineRenderer>();
            LineRenderer lineRenderer = square.GetComponent<LineRenderer>();
            ConfigureSquareLR(lineRenderer, startWidth, endWidth);
            lineRenderer.SetPositions(MakeSquare(cornerPoints));
        }

        static void ConfigureSquareLR(LineRenderer lineRenderer, float startWidth, float endWidth)
        {
            lineRenderer.startWidth = startWidth;
            lineRenderer.endWidth = endWidth;
            lineRenderer.positionCount = 5;
            lineRenderer.textureMode = LineTextureMode.Tile;
            lineRenderer.loop = true;
            lineRenderer.useWorldSpace = false;
            lineRenderer.numCornerVertices = 4;
        }

        static Vector3[] MakeSquare(Vector3[] endpoints)
        {
            Vector3[] corners = new Vector3[5];
            for (int i = 0; i < 4; i++)
            {
                corners[i] = endpoints[i];
            }
            corners[4] = endpoints[0];
            return corners;
        }

        public static void PlaceCircle(Vector3 startPoint, Vector3 endpoint, float startWidth, float endWidth)
        {
            GameObject circle = new GameObject("circle");
            circle.tag = "circle";
            circle.AddComponent<LineRenderer>();
            LineRenderer lineRenderer = circle.GetComponent<LineRenderer>();
            ConfigureCircleLR(lineRenderer, startWidth, endWidth);
            Vector3[] circlePoints = MakeCircle(startPoint, endpoint, Vector3.Distance(startPoint, endpoint));
            lineRenderer.positionCount = circlePoints.Length;
            lineRenderer.SetPositions(circlePoints);
        }
        static void ConfigureCircleLR(LineRenderer lineRenderer, float startWidth, float endWidth)
        {
            lineRenderer.startWidth = startWidth;
            lineRenderer.endWidth = endWidth;
            lineRenderer.textureMode = LineTextureMode.Tile;
            lineRenderer.loop = true;
            lineRenderer.useWorldSpace = false;
        }

        static Vector3[] MakeCircle(Vector3 startPoint, Vector3 endPoint, float distance)
        {
            float stepSize = 20;
            List<Vector3> points = new List<Vector3>();
            float current = 0;
            float total = distance;
            //call GetCenter()
            Vector3 center = (startPoint + endPoint) * .5f;
            //Arbitrarily define direction... FOR NOW!
            Vector3 direction = Vector3.up;
            center -= direction;
            Vector3 relStart = startPoint - center;
            Vector3 relEnd = endPoint - center;
            Vector3 nextPoint;

            while (current<total)
            {
                nextPoint = Vector3.Slerp(relStart, relEnd, current/total) + center;

                points.Add(nextPoint);
                
                current += stepSize / distance;

                if (current > total)
                    break;
            }

            //Debug.Log("# of points in this curve: " + points.Count);
            //List<Vector3> points now contains half of the circle's points.

            //Now, add the other side:
            current = 0;
            center = (startPoint + endPoint) * .5f;
            direction = Vector3.down;
            center -= direction;
            relStart = startPoint - center;
            relEnd = endPoint - center;
            while (current < total)
            {
                nextPoint = Vector3.Slerp(relEnd, relStart, current / total);
                nextPoint += center;

                points.Add(nextPoint);

                current += stepSize / distance;

                if (current > total)
                    break;
            }
            //Add the startpoint to the end of the
            //array to cap the circle off.
            points.Add(startPoint);
            
            //convert it to an array and return.
            Vector3[] finalPoints = points.ToArray();
            return finalPoints;
        }
    }
}



