using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter_Script : MonoBehaviour {

    public Rigidbody projectile;
    public Transform shotPos;
    public float shotForce = 10f;
    public float moveSpeed = 1f;
    private bool shoot = false;
	void Update () {

        if (shoot)
        {
            Rigidbody shot = Instantiate(projectile, shotPos.position, shotPos.rotation) as Rigidbody;
            shot.AddForce(shotPos.forward * shotForce);

            Debug.Log("Shoot in update before : " + shoot);
            shoot = false;
        }
		
	}

    public void FireAtWill()
    {
        shoot = true;
        Debug.Log("Shoot on click : " + shoot);
    }

}
