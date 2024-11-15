using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator MoveDoorUp()
    {
        Debug.Log("pressed");
        float targetHeight = gameObject.transform.position.y + 10f; 
        float speed = 2f; 

        while (gameObject.transform.position.y < targetHeight)
        {
            gameObject.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
            yield return null; 
        }

        // Ensuring precise final position
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, targetHeight, gameObject.transform.position.z);
    }
}
