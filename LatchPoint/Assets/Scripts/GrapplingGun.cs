using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleble;
    public Transform gunTip;
    public Transform cam;
    public Transform player;
    [SerializeField] private float maxDistance = 100f;
    private SpringJoint joint;
    [SerializeField] private PlayerMovement playerMovement;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
     
        if (Input.GetMouseButtonDown(0) && playerMovement.isAlive)
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0) && playerMovement.isAlive)
        {
            StopGrapple();
        }
    }
    private void LateUpdate()
    {
        DrawRope();
    }

    private void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxDistance ))
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
        if (!joint)
        {
            return;
        }
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }
    private void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
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
