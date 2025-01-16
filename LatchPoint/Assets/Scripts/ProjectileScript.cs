using System;
using System.Collections;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float damage = 10f;  // Damage that the projectile does when it hits something
    private GrapplingGun GrapplingGun;
    private void Start()
    {
        GrapplingGun = GameObject.FindGameObjectWithTag("gun").GetComponent<GrapplingGun>();
        // Destroy the projectile after a set time
        Destroy(gameObject, 1000f);
    }



    private void OnCollisionEnter(Collision collision)
    {
        print(collision);

        if (collision.collider.tag == "Player")
        {
            GrapplingGun.IncreaseAmmo();
            Destroy(gameObject);
        }
        // Check for collisions and apply damage
        if (collision.collider.tag == "Enemy")
        {
            // Apply damage to the enemy
            // You can add your damage logic here based on the type of object hit
            Debug.Log("Hit enemy!");
        }     
    }
    private void OnTriggerEnter(Collider other)
    {
        print(other);

        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
        // Check for collisions and apply damage
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Apply damage to the enemy
            // You can add your damage logic here based on the type of object hit
            Debug.Log("Hit enemy!");
        }
    }

     
    


    private void OnDestroy()
    {
        if (GrapplingGun != null)
        {
            GrapplingGun.IncreaseAmmo();
        }
    }
}
