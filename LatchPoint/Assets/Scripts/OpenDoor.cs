using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private bool moving = false;
    private float targetHeight;
    private Rigidbody rb;
    private bool up = true;
    private float originHeight;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        originHeight = gameObject.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            // Moving up
            if (up && gameObject.transform.position.y < targetHeight)
            {
                rb.velocity = new Vector3(0, 2f, 0); // Move upwards
            }
            // Moving down
            else if (!up && gameObject.transform.position.y > originHeight)
            {
                rb.velocity = new Vector3(0, -2f, 0); // Move downwards
            }
            else
            {
                // Stop movement when the target height is reached
                rb.velocity = Vector3.zero;
                moving = false;
                BoxCollider collider = gameObject.GetComponent<BoxCollider>();
                collider.enabled = true; // Enable the collider once movement stops
            }
        }
    }

    public void MoveDoorUp()
    {
        targetHeight = gameObject.transform.position.y + 2.5f; // Set target height for moving up
        moving = true;
        up = true;

        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        collider.enabled = false; // Disable the collider while the door is moving
    }

    public void MoveDoorDown()
    {
        targetHeight = gameObject.transform.position.y - 2.5f; // Set target height for moving down
        moving = true;
        up = false;

        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        collider.enabled = false; // Disable the collider while the door is moving
    }
}
