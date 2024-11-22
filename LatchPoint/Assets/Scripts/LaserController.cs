using UnityEngine;

public class LaserController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private BoxCollider boxCollider;

    public LayerMask detectableLayers; 
    private bool hasHitObject;


    private Vector3 startPoint;
    private Vector3 laserEndPoint;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 2;

        // Create a child GameObject for the collider
        GameObject colliderObject = new GameObject("LaserCollider");
        colliderObject.transform.parent = transform;
        colliderObject.AddComponent<LaserHit>();
        // Add a BoxCollider to the child object
        boxCollider = colliderObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        colliderObject.transform.localPosition = Vector3.zero;
        startPoint = gameObject.transform.position;

        DrawLaser();
        hasHitObject = false;
    }


    private void Update()
    {
        DrawLaser();
    }

    private void DrawLaser()
    {
       

        float maxLaserDistance = 1000f;

        RaycastHit hit;
        if (Physics.Raycast(startPoint, transform.up, out hit, maxLaserDistance, detectableLayers))
        {
            laserEndPoint = hit.point;
            hasHitObject = true;
        }
        else
        {
            laserEndPoint = startPoint + transform.up * maxLaserDistance;
            hasHitObject = false;
        }

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, laserEndPoint);

        UpdateBoxCollider();
    }
    private void UpdateBoxCollider()
    {
        float distance = Vector3.Distance(startPoint, laserEndPoint);

        //resize and relocate laser hitbox
        boxCollider.size = new Vector3(0.1f, distance, 0.1f);       
        boxCollider.center = new Vector3(0, distance / 2, 0); 
    }





  



   

    
}

