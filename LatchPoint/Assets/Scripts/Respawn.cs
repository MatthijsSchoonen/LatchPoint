using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject respawnUI;
    [SerializeField] private GameObject Player;
    private PlayerMovement PlayerMovement;
    
    void Start()
    {
      PlayerMovement = Player.GetComponent<PlayerMovement>();
    }


    public void Died()
    {
        hud.SetActive(false);
        respawnUI.SetActive(true);
        PlayerMovement.isAlive = false;
        Cursor.lockState = CursorLockMode.None;  
        Cursor.visible = true;                    

    }


    public void RespawnFunction()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
   
}
