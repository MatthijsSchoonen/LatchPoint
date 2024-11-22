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
        pauseMenu.SetActive(false);
    }




    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            print("pressed");
            pauseMenuActive = !pauseMenuActive;

            if (pauseMenuActive)
            {
                playerMovement.isAlive = false;
                pauseMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                playerMovement.isAlive = true;
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
