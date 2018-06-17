using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePoolObjects : MonoBehaviour
{

    public Camera ARCamera;
    public UnityEngine.ParticleSystem particleLauncher;
    public GameObject door;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySpawningItems(Input.mousePosition);
        }
    }

    public void TrySpawningItems(Vector2 screenPos)
    {
        Ray r = ARCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit))
        {
            Instantiate(door, hit.point + new Vector3(0, 0.2f, 0), Quaternion.identity);        // particle animation
           
        }
    }

}
