using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private bool moving = false; // corrected typo (moveing -> moving)
    private float targetHeight;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<Rigidbody>();
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving && gameObject.transform.position.y < targetHeight)
        {
            // Move the door upwards with constant speed
            rb.velocity = new Vector3(0, 2f, 0); 
        }
        else
        {
            // Stop the movement once the target height is reached
            rb.velocity = Vector3.zero;
            moving = false; // Set moving to false to stop further updates
        }
    }

    public void MoveDoorUp()
    {
        Debug.Log("pressed");
        targetHeight = gameObject.transform.position.y + 2.5f; // Target height + 10 units
        moving = true;

        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        collider.enabled = false; // Disable the collider while the door is moving
    }
}
