﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//June 11th Single Line Generation.



public class LerpScript : MonoBehaviour
{
    // Transforms to act as start and end markers for the journey.
    Vector3 lerpStart;
    Vector3 lerpEnd;
    int counter = 0;
    bool firstRun = true;

    Vector3 start;
    public GameObject _CurvePoint;

    List<Vector3> points = new List<Vector3>();
    List<Vector3> pointsRight = new List<Vector3>();
    List<Vector3> pointsLeft = new List<Vector3>();
    List<Vector3> pointsDown = new List<Vector3>();
    List<Vector3> pointsUp = new List<Vector3>();
    Vector3 newPoint;
    // Movement speed in units/sec.
    public float speed = 1.0F;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;

    bool goodToGo;
    void Start()
    {
        //start being the last child of the lineRenderer
        start = transform.GetChild(transform.childCount-1).position;
        print("32");
        // Keep a note of the time the movement started.
        startTime = Time.time;

        //Generate a list of points from a simple algorithm
        points.Add(start);
        for (int i = 1; i < 10; i++)
        {
            if (i % 7 == 0)
                points.Add(points[i-1] + new Vector3(2, 1, -3));
            else if(i % 5 == 0)
                points.Add(points[i - 1] + new Vector3(2, 2, -.5f));
            else if (i % 3 == 0)
                points.Add(points[i - 1] + new Vector3(1, 1, -1.5f));
            else if (i % 2 == 0)
                points.Add(points[i - 1] + new Vector3(1, 0));
            else
                points.Add(points[i-1] + new Vector3(1, 0));
        }
        print("Generated Points");

        ResetPointLerp();
    }
    //perform inverse t
    //could either perform inverse op on point list 4 times when it is generated or when I am about to place the points.
    //Because I am generating all the points at the beginning of the experience and interpolating between those later, I should perform inv op at the beginning.

    private void Update()
    {
        //below is the "lerp engine"
        if (goodToGo)
        {
            // Distance moved = time * speed.
            distCovered = (Time.time - startTime) * speed;

            // Fraction of journey completed = current distance divided by total distance.
            fracJourney = distCovered / journeyLength;
            // Set our position as a fraction of the distance between the markers.

            if (distCovered < journeyLength)
            {
                newPoint = Vector3.Lerp(lerpStart, lerpEnd, fracJourney);

                //Generate the next point in the line
                Instantiate(_CurvePoint, newPoint, Quaternion.identity, transform);
            }
            else //the lerp engine finishes here.
            {
                //this is where we would like to set new points for the lerp engine.
                goodToGo = false;   //if the update loop waits to run until everthing inside of it is complete, I won't need this.
                ResetPointLerp();
            }
        }
        else
        print("looks like update doesn't wait for completion of logic to run again.");

        print("should be more than other print");
    }


    //call this function whenever we begin lerping between a new set of points.
    void ResetPointLerp()
    {
        if (firstRun)
        {
            lerpStart = start;
            lerpEnd = points[counter];
            // Calculate the journey length.
            journeyLength = Vector3.Distance(start, lerpEnd);
            firstRun = false;
        }
        else
        {
            lerpStart = points[counter - 1];
            lerpEnd = points[counter];
            journeyLength = Vector3.Distance(lerpStart, lerpEnd);
        }
        counter++;
        startTime = Time.time;
        goodToGo = true;
    }



 float distCovered =0;
 float fracJourney;
 /*
    // Follows the target position like with a spring
    void Update()
    {            
        // Distance moved = time * speed.
        distCovered = (Time.time - startTime) * speed;

        // Fraction of journey completed = current distance divided by total distance.
        fracJourney = distCovered / journeyLength;
        // Set our position as a fraction of the distance between the markers.

        if(distCovered<journeyLength)
        {
            Debug.Log("added point");
        newPoint = Vector3.Lerp(start, endMarker.position, fracJourney);

            //check density of point generation
        points.Add(newPoint);

        //Generate the line
        Instantiate( _CurvePoint, newPoint, Quaternion.identity, transform);
        }
        else
        {
            Debug.Log("Point count is " + points.Count);
        }
    }
    */
}