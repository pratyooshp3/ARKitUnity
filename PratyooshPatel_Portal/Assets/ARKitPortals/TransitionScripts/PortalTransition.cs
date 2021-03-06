﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This component lives on the camera parent object and triggers a transition when you walk through a portal 
[RequireComponent(typeof(Rigidbody))]
public class PortalTransition : MonoBehaviour {

	public delegate void PortalTransitionAction();
	public static event PortalTransitionAction OnPortalTransition;

    public delegate void BallPortalTransitionAction();
    public static event BallPortalTransitionAction OnBallPortalTransition;

    public UnityEngine.ParticleSystem particleLauncher;

    // The main camera is surrounded by a SphereCollider with IsTrigger set to On
    void OnTriggerEnter(Collider portal)
    {

        //Debug.Log("Portal.name " + portal.name);                                                                      // 
        //Debug.Log("Portal.Gameobj.name "+portal.gameObject.name);                                                     // Movebox
        //Debug.Log("GetComponentInChildren<Camera> "+GetComponentInChildren<Camera>());                               // Main Camera
        //Debug.Log("GetComponentInChildren<RigidBody> "+GetComponentInChildren<Rigidbody>());                        // Ballparent(Clone)
        //Debug.Log("GetComponentInChildren<RigidBody> "+GetComponentInChildren<Rigidbody>().name);                    // Ballparent(Clone)
        //Debug.Log("portal.GetComponentInParent<Portal>() "+portal.GetComponentInParent<Portal>());                  // VRDoor

        if (GetComponentInChildren<Rigidbody>().name == "Ballparent(Clone)")
        {

            OnBallPortalTransition();
            // Now replicate a function like shown below (OnPortalTransition )and get a call on door class, then go to virtual world location and re throw the ball from there.
        }
        else { 


        Portal logic = portal.GetComponentInParent<Portal>();
        transform.position = logic.PortalCameras[1].transform.position - GetComponentInChildren<Camera>().transform.localPosition;

        if (OnPortalTransition != null)
        {
            // Emit a static OnPortalTransition event every time the camera enters a portal. The DoorManager listens for this event.

            Instantiate(particleLauncher, transform.position, transform.rotation);      // Animation of particle system when user enters the portal

            OnPortalTransition();

        }
    }
	}
}
