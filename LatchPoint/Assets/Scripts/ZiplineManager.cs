using System;
using UnityEngine;

public class ZiplineManager : MonoBehaviour
{
    private Transform player;
    private Rigidbody playerRb; // Rigidbody attached to the player
    private Vector3 point1, point2;
    private Vector3 currentTarget;
    private bool isTraversing = false;

    private bool nearby = false;
    GrapplingGun grapplingGun;

    public float traverseSpeed = 20f;
    private Transform playerOrientation; // Player's orientation (found by tag)

    private void Start()
    {
        grapplingGun = FindObjectOfType<GrapplingGun>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (grapplingGun != null)
        {
            // Directly get the zipline points in world space from grapplingGun
            point1 = grapplingGun.GetZiplinePoint1(); // This already returns the world position of ziplinePoint1
            point2 = grapplingGun.GetZiplinePoint2(); // This already returns the world position of ziplinePoint2

            // Log the initial world positions of point1 and point2
            Debug.Log("Initial Zipline Points: Point1 = " + point1 + ", Point2 = " + point2);
        }

        // Try to find the Orientation object using its tag
        GameObject orientationObj = GameObject.FindGameObjectWithTag("Orientation");
        if (orientationObj != null)
        {
            playerOrientation = orientationObj.transform;
        }
        else
        {
            Debug.LogError("Orientation object not found. Make sure it has the 'Orientation' tag.");
        }

        // Get the Rigidbody of the player
        playerRb = player.GetComponent<Rigidbody>();
        if (playerRb == null)
        {
            playerRb = player.parent.GetComponent<Rigidbody>();
        }
        if (playerRb == null)
        {
            Debug.LogError("Rigidbody not found on player.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player entered zipline trigger zone.");
            nearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player exited zipline trigger zone.");
            nearby = false;
        }
    }

    private void StartTraversal()
    {
        if (point1 != null && point2 != null && playerRb != null)
        {
            currentTarget = GetTargetBasedOnOrientation(); // Set the current target based on where the player is looking
            isTraversing = true;
            playerRb.useGravity = false; // Disable gravity for smooth zipline traversal
            Debug.Log("Started traversing zipline.");
        }
        else
        {
            Debug.LogWarning("Rigidbody or zipline points are missing.");
        }
    }

    private Vector3 GetTargetBasedOnOrientation()
    {
        if (playerOrientation == null)
        {
            Debug.LogError("Orientation object is missing.");
            return new Vector3();
        }

        // We calculate the direction in which the player is facing (forward)
        Vector3 playerForward = playerOrientation.forward;

        // Calculate the vector from the player to both zipline points
        Vector3 directionToPoint1 = (point1 - player.position).normalized;
        Vector3 directionToPoint2 = (point2 - player.position).normalized;

        // Now we calculate which point is more aligned with the player's facing direction
        float dotProduct1 = Vector3.Dot(playerForward, directionToPoint1); // Positive if facing point1
        float dotProduct2 = Vector3.Dot(playerForward, directionToPoint2); // Positive if facing point2

        // Choose the point that the player is facing the most (greater dot product)
        if (dotProduct1 > dotProduct2)
        {
            return point1;
        }
        else
        {
            return point2;
        }
    }

    private void StopTraversal()
    {
        if (playerRb != null)
        {
            isTraversing = false;
            playerRb.velocity = Vector3.zero; // Stop any residual velocity
            playerRb.useGravity = true; // Re-enable gravity after traversal
            Debug.Log("Stopped traversing zipline.");
        }
    }

    private void Update()
    {
        if (isTraversing && currentTarget != null)
        {
            TraverseZipline();
        }

        if (nearby)
        {
            if (Input.GetKeyDown(KeyCode.E) && !isTraversing)
            {
                StartTraversal();
            }

            // Remove the zipline
            if (Input.GetKeyDown(KeyCode.F))
            {
                GrapplingGun grapplingGun = FindObjectOfType<GrapplingGun>();
                grapplingGun.RemoveZipline();
            }
        }

        // Stop traversal if WASD or Space is pressed
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Space))
        {
            StopTraversal();
        }
    }

    private void TraverseZipline()
    {
        if (playerRb != null)
        {
            // Move towards the current target (either point1 or point2)
            Vector3 direction = (currentTarget - player.position).normalized;

            // Apply velocity to the Rigidbody for traversal
            Vector3 newVelocity = direction * traverseSpeed;
            playerRb.velocity = newVelocity;

            // Check if the Rigidbody has reached the current target
            if (Vector3.Distance(player.position, currentTarget) < 0.1f)
            {
                // Switch to the other endpoint (toggle between point1 and point2)
                currentTarget = currentTarget == point1 ? point2 : point1;

                // Stop the Rigidbody momentarily to avoid overshooting
                playerRb.velocity = Vector3.zero;
            }
        }
        else
        {
            Debug.LogWarning("Rigidbody not found on player. Unable to traverse zipline.");
        }
    }
}
