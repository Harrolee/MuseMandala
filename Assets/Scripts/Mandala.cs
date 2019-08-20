using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mandala
{
    //General is a staging bucket for new functions.
    //When enough new functions pile up, I can form
    //classes out of them.
    public class General
    {
        public static IEnumerator RevealBoundaries(Material[] boundaryMats)
        {
            float alphaVal = 1f;
            //Start by setting alpha to full.
            foreach (Material mat in boundaryMats)
            {
                mat.SetFloat("Vector1", alphaVal);
            }
            
            float startTime = Time.time;
            float currTime = startTime;
            float x;
            float totalSecs = 5f;

            foreach (Material mat in boundaryMats)
            {
                while (currTime < 5f)
                {
                    currTime = Time.time - startTime;
                    Debug.Log("currTime: " + currTime);
                    x = currTime / totalSecs;
                    alphaVal = Mathf.Lerp(1, 0, x);
                    mat.SetFloat("Vector1", alphaVal);
                    yield return null;
                }
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
        public static void PlaceSquare(Vector3[] cornerPoints)
        {
            //distance is here in case, in a future iteration 
            //of MakeSquare, I derive a square from two given points.
            //float distance = Vector3.Distance(endpoints[0], endpoints[1]);
            GameObject square = new GameObject("square");
            square.tag = "square";
            square.AddComponent<LineRenderer>();
            LineRenderer squareLR = square.GetComponent<LineRenderer>();
            squareLR.useWorldSpace = false;
            squareLR.positionCount = 5;
            squareLR.SetPositions(MakeSquare(cornerPoints));
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

        public static void PlaceCircle(Vector3 startPoint, Vector3 endpoint)
        {
            GameObject circle = new GameObject("circle");
            circle.tag = "circle";
            circle.AddComponent<LineRenderer>();
            LineRenderer squareLR = circle.GetComponent<LineRenderer>();
            Vector3[] circlePoints = MakeCircle(startPoint, endpoint, Vector3.Distance(startPoint, endpoint));
            squareLR.positionCount = circlePoints.Length;
            squareLR.SetPositions(circlePoints);
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

            Debug.Log("# of points in this curve: " + points.Count);
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
    //temp methods for 8/26 demo
    public class RemoveMeLater
    {
        public static void MakeLine()
        {

        }
    }
}



