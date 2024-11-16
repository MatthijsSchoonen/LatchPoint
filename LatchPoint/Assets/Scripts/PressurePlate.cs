using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    private int objectsEntered = 0;
    private Rigidbody rb;
    private Vector3 initialPosition;
    private Vector3 downPosition;
    private Vector3 upPosition;

    [SerializeField] private bool UseOpenDoor = false;
    private OpenDoor openDoorScript;
    private bool movedUp = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GameObject.FindGameObjectWithTag("PressurePlate").GetComponent<Rigidbody>();
        if (UseOpenDoor)
        {
            openDoorScript = GameObject.FindGameObjectWithTag("Door").GetComponent<OpenDoor>();
        }

        // Save the initial position and calculate target positions
        initialPosition = transform.position;
        downPosition = initialPosition + new Vector3(0, -0.2f, 0); // Move down by 0.2 units
        upPosition = initialPosition; // Original position
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Object"))
        {
            if (objectsEntered == 0) // First object enters
            {
                moveDown();

                // Move the door up if the functionality is enabled
                if (UseOpenDoor && !movedUp)
                {
                    movedUp = true;
                    openDoorScript.MoveDoorUp();
                }
            }
            objectsEntered++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Object"))
        {
            objectsEntered--;
            if (objectsEntered == 0) // Last object leaves
            {
                moveUp();

                // Move the door down if the functionality is enabled
                if (UseOpenDoor && movedUp)
                {
                    movedUp = false;
                    openDoorScript.MoveDoorDown();
                }
            }
        }
    }

    private void moveDown()
    {
        if (rb != null)
        {
            // Move the object to the down position (don't keep adding offset)
            rb.MovePosition(downPosition);
        }
    }

    private void moveUp()
    {
        if (rb != null)
        {
            // Move the object back to the original position
            rb.MovePosition(upPosition);
        }
    }
}
