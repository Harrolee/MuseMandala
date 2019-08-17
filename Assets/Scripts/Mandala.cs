using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mandala
{
    public class Patterns
    {
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
        //better know as "make two overlapping diagonal lines"
        public static Vector3[] MakeCircle(Vector3 startPoint, Vector3 endPoint, float distance)
        {
            float stepSize = 20;
            List<Vector3> points = new List<Vector3>();
            float current = 0;
            float total = distance;
            Vector3 center = (startPoint + endPoint) * .5f;
            Vector3 relStart = startPoint + center;
            Vector3 relEnd = endPoint + center;
            Vector3 nextPoint;
            while (current<total)
            {
                center -= new Vector3(.5f, .5f, 0); 
                nextPoint = Vector3.Slerp(relStart, relEnd, current/total);
                Debug.Log("nextPoint is: " +nextPoint);
                points.Add(nextPoint);
                relStart = startPoint - center;
                //relEnd = endPoint - center;
                nextPoint += center;
                current += stepSize / distance;
                if (current > total)
                    break;
                        //current= total;
            }

            Debug.Log("# of points in this curve: " + points.Count);
            //list now contains the first half of points.

            //Now, add the other side:
            //current = 0;
            //center = (startPoint + endPoint) * .5f;
            //relStart = endPoint + center;
            //relEnd = startPoint + center;
            //while (current <= total)
            //{
            //    center -= new Vector3(.5f, .5f, 0);
            //    current += stepSize / distance;
            //    nextPoint = Vector3.Slerp(relStart, relEnd, current / total);
            //    points.Add(Vector3.Slerp(relStart, relEnd, current / total));
            //    relStart = startPoint + center;
            //    relEnd = endPoint + center;
            //    nextPoint += center;
            //}

            //Debug.Log("# of points in this circle: " + points.Count);
            //convert it to an array and return.
            Vector3[] finalPoints = points.ToArray();//new Vector3[points.Count];
            return finalPoints;
        }

        public static Vector3[] MakeSquare(Vector3[] endpoints, float distance)
        {
            Vector3[] corners = new Vector3[5];
            for (int i = 0; i < 4; i++)
            {
                corners[i] = endpoints[i];
            }
            corners[4] = endpoints[0];
            return corners;
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



