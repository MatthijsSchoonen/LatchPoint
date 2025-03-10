using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject pauseMenu;
    private bool pauseMenuActive = false;
    [SerializeField] private PlayerMovement playerMovement;
    void Start()
    {
        
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu").gameObject;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        
    }




    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            pauseMenuActive = !pauseMenuActive;

            if (pauseMenuActive)
            {
                playerMovement.isPaused = true;
                pauseMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
               
                playerMovement.isPaused = false;
                pauseMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

    }

    public void ReturnHome()
    {
        SceneManager.LoadScene("StartMenu");
    }

}
