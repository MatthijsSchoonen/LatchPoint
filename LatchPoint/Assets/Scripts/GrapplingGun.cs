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


    private GameObject zipline; // The zipline GameObject
    private Transform pos1, pos2; // Endpoints of the zipline
    private bool ziplineReady = false; // Whether the zipline is ready for use
    private bool onZipline = false; // Whether the player is on the zipline


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
        if (currentMode == FiringMode.Zipline)
        {
            HandleZiplineActions();
        }

        if (Input.GetKeyDown(KeyCode.F) && zipline != null)
        {
            PickupZipline();
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

    private void HandleZiplineActions()
    {
        if (Input.GetMouseButton(0)) // Forward direction (left click)
        {
            AttachZiplineEndpoint(ref pos2, true); // True = Forward
        }

        if (Input.GetMouseButton(1)) // Backward direction (right click)
        {
            AttachZiplineEndpoint(ref pos1, false); // False = Backward
        }

        if (Input.GetKeyDown(KeyCode.E) && ziplineReady && pos1 != null && pos2 != null)
        {
            StartZipline();
        }
    }


    private void AttachZiplineEndpoint(ref Transform pos, bool isForward)
    {
        Vector3 direction = isForward ? cam.forward : -cam.forward; // Determine the direction
        RaycastHit hit;

        if (Physics.Raycast(cam.position, direction, out hit, maxDistance))
        {
            if (pos == null)
            {
                if (zipline == null)
                {
                    // Create the zipline GameObject
                    zipline = new GameObject("Zipline");
                    LineRenderer ziplineRenderer = zipline.AddComponent<LineRenderer>();
                    ziplineRenderer.startWidth = 0.1f;
                    ziplineRenderer.endWidth = 0.1f;
                    ziplineRenderer.positionCount = 2;

                    // Create pos1 and pos2 as child objects
                    pos1 = new GameObject("Pos1").transform;
                    pos2 = new GameObject("Pos2").transform;
                    pos1.parent = zipline.transform;
                    pos2.parent = zipline.transform;
                }

                // Assign or update the zipline endpoint
                pos = new GameObject("ZiplinePoint").transform;
                pos.position = hit.point;
                pos.parent = zipline.transform;

                CheckZiplineReady();
            }
            else
            {
                // Update the position of the existing endpoint
                pos.position = hit.point;
                CheckZiplineReady();
            }
        }
    }


    private void CheckZiplineReady()
    {
        ziplineReady = pos1 != null && pos2 != null;
        if (ziplineReady)
        {
            LineRenderer ziplineRenderer = zipline.GetComponent<LineRenderer>();
            ziplineRenderer.SetPositions(new Vector3[] { pos1.position, pos2.position });
        }
    }


    private void StartZipline()
    {
        if (onZipline || zipline == null) return;

        StartCoroutine(MoveAlongZipline());
    }

    private IEnumerator MoveAlongZipline()
    {
        onZipline = true;

        // Determine closest point
        float distanceToPos1 = Vector3.Distance(player.position, pos1.position);
        float distanceToPos2 = Vector3.Distance(player.position, pos2.position);

        Transform start = distanceToPos1 < distanceToPos2 ? pos1 : pos2;
        Transform end = start == pos1 ? pos2 : pos1;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime; // Adjust speed here
            player.position = Vector3.Lerp(start.position, end.position, t);
            yield return null;
        }

        onZipline = false;
    }


    private void PickupZipline()
    {
        if (zipline != null)
        {
            Destroy(zipline);
            zipline = null;
            pos1 = null;
            pos2 = null;
            ziplineReady = false;
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
