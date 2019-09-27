using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowUser : MonoBehaviour
{
    GameObject userCam;
    void OnEnable()
    {
        userCam = GameObject.FindGameObjectWithTag("MainCamera");
        //get user head
    }

    void Update()
    {
        transform.rotation = userCam.transform.rotation;
       //map cam pos to user's head.
    }
}
