using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHit : MonoBehaviour
{
    private Respawn respawn;
    private void Start()
    {
        respawn  = GameObject.FindGameObjectWithTag("Logic").GetComponent<Respawn>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            respawn.Died();       
        }
    }
}
