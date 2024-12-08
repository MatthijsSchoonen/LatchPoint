using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleble;
    public Transform gunTip;
    public Transform cam;
    public Transform player;
    [SerializeField] private float maxDistance = 8f;
    private SpringJoint joint;
    [SerializeField] private PlayerMovement playerMovement;

    // Enum for firing modes
    private enum FiringMode { Swing, Pull, Zipline, Connect, Fire }
    private FiringMode currentMode = FiringMode.Swing;


    private GameObject pulledObject; // Reference to the pulled object
    private SpringJoint objectSpringJoint; // SpringJoint for the object

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0; // Ensure it's initialized as empty
    }

    private void Update()
    {
        HandleModeSwitch();

        if (Input.GetMouseButtonDown(0) && playerMovement.isAlive)
        {
            HandleGrappleAction();
        }
        else if (Input.GetMouseButtonUp(0) && playerMovement.isAlive)
        {
            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        if (currentMode == FiringMode.Pull && objectSpringJoint != null)
        {
            // Dynamically update the connectedAnchor to follow the gunTip position
            objectSpringJoint.connectedAnchor = gunTip.position;
        }

        DrawRope();
    }


    // Switch firing modes using scroll wheel or number keys
    private void HandleModeSwitch()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            currentMode = (FiringMode)(((int)currentMode + 1) % 5); // Cycle modes forward
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            currentMode = (FiringMode)(((int)currentMode - 1 + 5) % 5); // Cycle modes backward
        }

        // Switch directly with number keys
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentMode = FiringMode.Swing;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentMode = FiringMode.Pull;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentMode = FiringMode.Zipline;
        if (Input.GetKeyDown(KeyCode.Alpha4)) currentMode = FiringMode.Connect;
        if (Input.GetKeyDown(KeyCode.Alpha5)) currentMode = FiringMode.Fire;

        Debug.Log("Current Mode: " + currentMode); // Debugging to see the active mode
    }

    private void HandleGrappleAction()
    {
        switch (currentMode)
        {
            case FiringMode.Swing:
                StartGrapple();
                break;
            case FiringMode.Pull:
                StartPull();
                break;
            case FiringMode.Zipline:
                Debug.Log("Zipline Mode Action Triggered");
                break;
            case FiringMode.Connect:
                Debug.Log("Connect Mode Action Triggered");
                break;
            case FiringMode.Fire:
                Debug.Log("Fire Mode Action Triggered");
                break;
        }
    }

    private void StartPull()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Object"))
            {
                // Hit a valid object, attach a spring joint and start pulling
                pulledObject = hit.collider.gameObject;

                // Add a SpringJoint to the object
                objectSpringJoint = pulledObject.AddComponent<SpringJoint>();
                objectSpringJoint.autoConfigureConnectedAnchor = false;

                // Initially set the connectedAnchor to the gun tip's position
                objectSpringJoint.connectedAnchor = gunTip.position;

                // Configure the spring joint for pulling
                objectSpringJoint.spring = 8f; // High spring force for stronger pull
                objectSpringJoint.damper = 5f;   // Moderate damping to smooth movement
                objectSpringJoint.massScale = 1f;

                // Set up the LineRenderer
                lr.positionCount = 2;
                grapplePoint = pulledObject.transform.position; // Update grapplePoint for the rope
            }
        }
    }






    private void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
        }
    }

    private void DrawRope()
    {
        if (pulledObject != null && currentMode == FiringMode.Pull)
        {
            // Continuously update the line between the gun and the object
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, pulledObject.transform.position);
        }
        else if (joint != null)
        {
            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, grapplePoint);
        }
        else
        {
            lr.positionCount = 0; // Clear the line if not in use
        }
    }

    private void StopGrapple()
    {
        lr.positionCount = 0;

        if (joint != null)
        {
            Destroy(joint);
        }

        if (pulledObject != null)
        {
            // Remove the SpringJoint from the object
            Destroy(objectSpringJoint);
            pulledObject = null;
        }
    }

    public bool isGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
