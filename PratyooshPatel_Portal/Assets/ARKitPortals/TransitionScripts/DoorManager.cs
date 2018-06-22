using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;

// This class shows and hides doors (aka portals) when you walk into them. It listens for all OnPortalTransition events
// and manages the active portal.
public class DoorManager : MonoBehaviour
{

    public delegate void DoorAction(Transform door);
    public static event DoorAction OnDoorOpen;

    public Text instructionText;
    public GameObject doorToVirtual;
    public GameObject doorToReality;
    public GameObject destination;                      // Position selection of 3D model to be displayed 
    public UnityEngine.ParticleSystem particleLauncher;
    public Camera mainCamera;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private GameObject currDoor;
    private Vector3 position;
    private Quaternion rotation;
    private bool isCurrDoorOpen = false;
    private bool isNextDoorVirtual = true;

    public Rigidbody projectile;            // Projectile to be thrown
    public Transform shotPos;              // Shooting position
    public float shotForce = 25f;
    public float moveSpeed = 1f;
    private bool shoot = false;

    void Start()
    {
        
        instructionText.text = "Looking for the planes, keep the camera pointed to the floor.";         // User instruction to keep the phone pointed to the floor
        PortalTransition.OnPortalTransition += OnDoorEntrance;
        PortalTransition.OnBallPortalTransition += OnBallEntrance;


        UnityARSessionNativeInterface.ARAnchorAddedEvent += PlanesFound;   
       // UnityARSessionNativeInterface.ARFrameUpdatedEvent += LookingForPlanes;                         
       // UnityARSessionNativeInterface.ARAnchorRemovedEvent += AnchorRemoved;
    }

    void Update()
    {

        if (shoot)
        {
            shoot = false;
            Rigidbody shot = Instantiate(projectile, shotPos.position, shotPos.rotation) as Rigidbody;
            shot.AddForce(shotPos.forward * shotForce);
           
        }

    }

    private void PlanesFound(ARPlaneAnchor anchorData)
    {
        instructionText.text = "Plane detected!! Now place portal to the room of your choice and shoot a ball.";    // User instruction to notify users about placing the portal after plane detection
   
    }

    //private void AnchorRemoved(ARPlaneAnchor anchorData)
    //{
    //    instructionText.text = "Planes removed";
    //}


    public void Visit_Living_Room()    // Place a portal to the Living Room 3D model
    {

        destination.transform.position = new Vector3(2.134f, 176.6f, 1.889f);                   // Relocate the destination of 3D model of Living Room
        position = destination.transform.position;
        ARPoint point = new ARPoint
        {
            x = 0.5f, //do a hit test at the center of the screen
            y = 0.5f
        };

        // prioritize result types
        ARHitTestResultType[] resultTypes = {
            ARHitTestResultType.ARHitTestResultTypeHorizontalPlane
                       ,ARHitTestResultType.ARHitTestResultTypeFeaturePoint
        };

        foreach (ARHitTestResultType resultType in resultTypes)
        {
            if (HitTestWithResultType(point, resultType))
            {
                return;
            }
        }
    }

    public void Visit_Model_House()     // Place a portal to the Model House 3D model
    {
        destination.transform.position = new Vector3(2.134f, 201.6f, 1.889f);           // Relocate the destination of 3D model of Model House
        position = destination.transform.position;
        ARPoint point = new ARPoint
        {
            x = 0.5f, //do a hit test at the center of the screen
            y = 0.5f
        };

        // prioritize result types
        ARHitTestResultType[] resultTypes = {
            ARHitTestResultType.ARHitTestResultTypeHorizontalPlane
                       ,ARHitTestResultType.ARHitTestResultTypeFeaturePoint
        };

        foreach (ARHitTestResultType resultType in resultTypes)
        {
            if (HitTestWithResultType(point, resultType))
            {
                return;
            }
        }
    }

    bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)      // UI button
    {
        List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
        if (hitResults.Count > 0)
        {
            foreach (var hitResult in hitResults)
            {   // Manually adding 1 unit on Y axis because of the pivot point and center of the 3D model
                Vector3 position = UnityARMatrixOps.GetPosition(hitResult.worldTransform) + new Vector3(0, 1f, 0);      
                Quaternion rotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);


                OpenDoorInFront(position , rotation);             // Passing the data to spwan the portal on detected dimentions from hit test

                Instantiate(particleLauncher, position + new Vector3(0, 0.2f, 0), rotation);        // particle animation
                instructionText.text = "Enjoy your journey!!";
                Vector3 currentAngle = transform.eulerAngles;
                transform.LookAt(Camera.main.transform);
                transform.eulerAngles = new Vector3(currentAngle.x, transform.eulerAngles.y, currentAngle.z);               // Angle correction for the door that is placed
                return true;

            }
        }
        return false;
    }



    // This method is called from the Spawn Portal button in the UI. It spawns a portal in front of you.
    // This needs to be replaced with the one with featurepoints
    public void OpenDoorInFront(Vector3 pos, Quaternion rot)
    {
        if (!isCurrDoorOpen)        
        {
            //if (isNextDoorVirtual)
            //    currDoor = doorToVirtual;
            //else
                //currDoor = doorToReality;
            Debug.Log("$$$$$$$$$$ Frontdoor Opened $$$$$$$$$$");

            currDoor = doorToVirtual;

            currDoor.SetActive(true);

            currDoor.transform.position = pos;

            currDoor.transform.rotation = rot;

            rotation = rot;

            currDoor.GetComponentInParent<Portal>().Source.transform.localPosition = currDoor.transform.position;

           // StartCoroutine(ScaleOverTime(2, currDoor));               // coroutine to scale the door overtime
            isCurrDoorOpen = true;

            if (OnDoorOpen != null)
            {
                OnDoorOpen(currDoor.transform);
            }
        }
    }

   

    private void OnBallEntrance()
    {
        if (isCurrDoorOpen)     // Entry
        {
            

            Debug.Log("~~~~~~~~~~~~~~~BallEntrance called~~~~~~~~~~~~~~~");
            shotPos.transform.position = position +new Vector3(0, 0f, 1f);
            shotPos.transform.rotation  = rotation;         //*= Quaternion.Euler(0, 180, 0);
           
            shoot = true;

            Debug.Log("Location of the back door ball shooter " + shotPos.transform.position.ToString("F4"));


            //OpenDoorOnBack(position, rotation);

        }
       

       
    }

    // Respond to the player walking into the doorway. Since there are only two portals, we don't need to pass which
    // portal was entered.
    private void OnDoorEntrance()
    {
        if (isCurrDoorOpen)     // Entry
        {
            Debug.Log("$$$$$$$$$$ Frontdoor Entered $$$$$$$$$$");
            currDoor.SetActive(false);
            isCurrDoorOpen = false;
            isNextDoorVirtual = !isNextDoorVirtual;
            OpenDoorOnBack(position, rotation);
           
        }
        else
        {                  // Exit
            currDoor.SetActive(false);
            isCurrDoorOpen = true;
            isNextDoorVirtual = !isNextDoorVirtual;
            instructionText.text = "I hope you enjoyed your visit. See you soon.";         // User instruction to keep the phone pointed to the floor
        }
    }

    public void OpenDoorOnBack(Vector3 pos, Quaternion rot)
    {
            //if (isNextDoorVirtual)
            //    currDoor = doorToVirtual;
            //else
                //currDoor = doorToReality;

            Debug.Log("$$$$$$$$$$ Backdoor Opened $$$$$$$$$$");

            currDoor = doorToReality;

            currDoor.SetActive(true);

            currDoor.transform.position = pos + new Vector3(0,0,-1f);

            rot *= Quaternion.Euler(0, 180, 0);

            currDoor.transform.rotation = rot;

            rotation = rot;

            currDoor.GetComponentInParent<Portal>().Source.transform.localPosition = currDoor.transform.position;

            Debug.Log("Location of the back door "+currDoor.transform.position.ToString("F4"));

            // StartCoroutine(ScaleOverTime(2, currDoor));               // coroutine to scale the door overtime
            isCurrDoorOpen = false;

            if (OnDoorOpen != null)
            {
                OnDoorOpen(currDoor.transform);
            }
      
    }

    public void ReTrack()           // Used for retracking of planes
    {
        ARKitWorldTrackingSessionConfiguration sessionConfig = new ARKitWorldTrackingSessionConfiguration(UnityARAlignment.UnityARAlignmentGravity, UnityARPlaneDetection.Horizontal);
        UnityARSessionNativeInterface.GetARSessionNativeInterface().RunWithConfigAndOptions(sessionConfig, UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors | UnityARSessionRunOption.ARSessionRunOptionResetTracking);
    }

    IEnumerator ScaleOverTime(float time, GameObject crDoor)
    {
        Vector3 originalScale = crDoor.transform.localScale;
        Vector3 destinationScale = new Vector3(1.2f, 1.2f, 1.2f);

        float currentTime = 0.0f;

        do
        {
            crDoor.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);


    }


}







//----------------------




//public void OpenDoorInFront()
//{
//    if (!isCurrDoorOpen)
//    {
//        if (isNextDoorVirtual)
//            currDoor = doorToVirtual;
//        else
//            currDoor = doorToReality;


//        currDoor.SetActive(true);

//        currDoor.transform.position = (Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up)).normalized
//            + mainCamera.transform.position;

//        currDoor.transform.rotation = Quaternion.LookRotation(
//            Vector3.ProjectOnPlane(currDoor.transform.position - mainCamera.transform.position, Vector3.up));

//        currDoor.GetComponentInParent<Portal>().Source.transform.localPosition = currDoor.transform.position;

//        Instantiate(particleLauncher, currDoor.transform.position, currDoor.transform.rotation);      //+ new Vector3(0,0,0)

//        isCurrDoorOpen = true;

//        if (OnDoorOpen != null)
//        {
//            OnDoorOpen(currDoor.transform);
//        }
//    }
//}
