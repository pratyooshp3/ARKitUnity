using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter_Script : MonoBehaviour {       // Script attached to main camera to shoot balls

    public Rigidbody projectile;            // Projectile to be thrown
    public Transform shotPos;               // Shooting position
    public float shotForce = 25f;            
    public float moveSpeed = 1f;
    private bool shoot = false;


    private void Start()
    {
      
    }
    void Update () {

        if (shoot)
        {
            Rigidbody shot = Instantiate(projectile, shotPos.position, shotPos.rotation) as Rigidbody;
            shot.AddForce(shotPos.forward * shotForce);
            shoot = false;
        }
		
	}

    public void FireAtWill()            // Function attached to the GUI Button, shoots a ball per click
    {
        shoot = true;
    }

}
