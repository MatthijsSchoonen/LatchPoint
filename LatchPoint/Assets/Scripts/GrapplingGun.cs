using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private GameObject zipline; // The zipline GameObject
    private Transform pos1, pos2; // Endpoints of the zipline
    private bool ziplineReady = false; // Whether the zipline is ready for use


    private GameObject pulledObject; // Reference to the pulled object
    private SpringJoint objectSpringJoint; // SpringJoint for the object

    private GameObject ziplineParent;
    private Transform ziplinePoint1, ziplinePoint2;
    private BoxCollider ziplineTrigger;
    private bool ziplineActive = false;


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

     

        if (Input.GetKeyDown(KeyCode.F) && zipline != null)
        {
            RemoveZipline();
        }
    }

    private void LateUpdate()
    {
        if (currentMode == FiringMode.Pull && objectSpringJoint != null)
        {
            // Dynamically update the connectedAnchor to follow the gunTip position
            objectSpringJoint.connectedAnchor = gunTip.position;
        }

        if (currentMode != FiringMode.Zipline)
        {
            DrawRope();
        }
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
                FireZipline();
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

    // Handle zipline logic (left mouse button fires both forward and backward)
    private void FireZipline()
    {
        if (ziplineActive)
        {
            return;
        }

        RaycastHit hit1, hit2;

        // First point (forward direction)
        if (Physics.Raycast(cam.position, cam.forward, out hit1, maxDistance))
        {
            // Create parent object for the zipline
            ziplineParent = new GameObject("ZiplineParent");
            LineRenderer lr = ziplineParent.AddComponent<LineRenderer>();
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.positionCount = 2;
            Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
            lineMaterial.color = Color.black;  // Set the material color to black
            lr.material = lineMaterial;

            // Add ZiplineManager to ziplineParent
            ziplineParent.AddComponent<ZiplineManager>();

            // Create first zipline point
            ziplinePoint1 = new GameObject("ZiplinePoint1").transform;
            ziplinePoint1.position = hit1.point;

            // Temporarily unparent to avoid inheritance of parent's position
            Transform originalParent1 = ziplinePoint1.transform.parent;
            ziplinePoint1.transform.parent = null; // Temporarily unparent
            ziplinePoint1.position = hit1.point;  // Set the world position explicitly
            ziplinePoint1.transform.parent = originalParent1; // Reparent back

            // Second point (backward direction)
            if (Physics.Raycast(cam.position, -cam.forward, out hit2, maxDistance))
            {
                // Create second zipline point
                ziplinePoint2 = new GameObject("ZiplinePoint2").transform;
                ziplinePoint2.position = hit2.point;

                // Temporarily unparent to avoid inheritance of parent's position
                Transform originalParent2 = ziplinePoint2.transform.parent;
                ziplinePoint2.transform.parent = null; // Temporarily unparent
                ziplinePoint2.position = hit2.point;  // Set the world position explicitly
                ziplinePoint2.transform.parent = originalParent2; // Reparent back

                // Draw the zipline
                DrawZipline();

                // Add BoxCollider to enable interaction
                AddZiplineTrigger();

                ziplineActive = true;
                Debug.Log("Zipline fired successfully.");
            }
            else
            {
                Destroy(ziplineParent);
                Debug.Log("Failed to find a second zipline point.");
            }

            Debug.Log(ziplinePoint1.position);
            Debug.Log(ziplinePoint2.position);
        }
    }


    private void DrawZipline()
    {
        if (ziplinePoint1 != null && ziplinePoint2 != null && ziplineParent != null)
        {
            LineRenderer lr = ziplineParent.GetComponent<LineRenderer>();
            lr.SetPosition(0, ziplinePoint1.position);
            lr.SetPosition(1, ziplinePoint2.position);
            lr.startColor = Color.black;  // Set the start color of the line
            lr.endColor = Color.black;    // Set the end color of the line
        
        }
    }

    private void AddZiplineTrigger()
    {
        if (ziplinePoint1 == null || ziplinePoint2 == null || ziplineParent == null)
        {
            Debug.LogError("Failed to add BoxCollider: Zipline points or parent are missing.");
            return;
        }

        // Calculate the center position and direction
        Vector3 point1 = ziplinePoint1.position;
        Vector3 point2 = ziplinePoint2.position;

        Vector3 center = (point1 + point2) / 2f; // Midpoint of the zipline
        Vector3 direction = (point2 - point1).normalized; // Direction from point1 to point2
        float length = Vector3.Distance(point1, point2); // Distance between the points

        // Add and configure the BoxCollider
        ziplineTrigger = ziplineParent.AddComponent<BoxCollider>();
        ziplineTrigger.isTrigger = true;


        // Set the BoxCollider size: length along Z, reasonable width and height
        ziplineTrigger.size = new Vector3(1.3f, 1.3f, length); // Adjust X and Y for thickness if needed
        ziplineTrigger.center = Vector3.zero;

        // Set position and rotation explicitly in world space
        ziplineTrigger.transform.position = center;
        ziplineTrigger.transform.rotation = Quaternion.LookRotation(direction);

        // To prevent rotation issues caused by the parent, unparent the collider temporarily, set it up, and reparent it
        Transform originalParent = ziplineTrigger.transform.parent;
        ziplineTrigger.transform.parent = null; // Temporarily unparent to avoid inheriting unwanted rotations
        ziplineTrigger.transform.position = center;
        ziplineTrigger.transform.rotation = Quaternion.LookRotation(direction);
        ziplineTrigger.transform.parent = originalParent; // Reparent after positioning

        Debug.Log($"BoxCollider added: Center={center}, Direction={direction}, Length={length}");
    }





    public void RemoveZipline()
    {
        if (ziplineActive && ziplineParent != null)
        {
            Destroy(ziplineParent);
            ziplineParent = null;
            ziplinePoint1 = null;
            ziplinePoint2 = null;
            ziplineTrigger = null;
            ziplineActive = false;

            Debug.Log("Zipline removed.");
        }
    }

    public Vector3 GetZiplinePoint1() => ziplinePoint1.position;
    public Vector3 GetZiplinePoint2() => ziplinePoint2.position;
    public bool IsZiplineActive() => ziplineActive;

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
