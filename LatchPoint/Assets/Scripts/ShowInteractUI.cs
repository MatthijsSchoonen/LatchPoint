using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShowInteractUI : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject button;
    [SerializeField] private GameObject objectToSpawn; // Optional: assign a prefab in the inspector
    [SerializeField] private Transform spawnLocation;  // Optional: specify the spawn location

    [SerializeField] private bool UseOpenDoor = false;
    private OpenDoor openDoorScript; 
    private bool nearby = false;

    // Update is called once per frame\

    private void Start()
    {
        openDoorScript = GameObject.FindGameObjectWithTag("Door").GetComponent<OpenDoor>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && nearby)
        {
            

            if (button != null)
            {
                // Move button slightly down
                button.transform.position = button.transform.position + new Vector3(0, -0.02f, 0);
                canvas.SetActive(false);

                // Optional: Spawn an object if specified
                if (objectToSpawn != null && spawnLocation != null)
                {
                    Instantiate(objectToSpawn, spawnLocation.position, spawnLocation.rotation);
                }

                // Call the MoveDoorUp() method from the OpenDoor script
                if (UseOpenDoor)
                {
                    StartCoroutine(openDoorScript.MoveDoorUp());
                }

                // Optional: Destroy this GameObject
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        canvas.SetActive(true);
        nearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        canvas.SetActive(false);
        nearby = false;
    }
}