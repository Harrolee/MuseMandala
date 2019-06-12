using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpScript : MonoBehaviour
{
    // Transforms to act as start and end markers for the journey.
    int start;
    int nextPoint;
    public GameObject _CurvePoint;
    public Transform startMarker;
    public Transform endMarker;
    List<Vector3> points = new List<Vector3>();
    Vector3 newPoint;
    // Movement speed in units/sec.
    public float speed = 1.0F;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;

    void Start()
    {
        //start being the last child of the lineRenderer
        start = transform.GetChild(transform.childCount-1).position;
        // Keep a note of the time the movement started.
        startTime = Time.time;

        // Calculate the journey length.
        journeyLength = Vector3.Distance(start, endMarker.position);
    }

 float distCovered =0;
 float fracJourney;
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
        newPoint = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
        points.Add(newPoint);
        //here's the test
        Instantiate( _CurvePoint, newPoint, Quaternion.identity, transform);
        }
        else
        {
            Debug.Log("Point count is " + points.Count);
        }
    }
}