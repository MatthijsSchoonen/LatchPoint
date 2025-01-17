using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelSelect : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject levelSelect;
    [SerializeField] private GameObject StartMenu;

  
    public void Back()
    {
        levelSelect.SetActive(false);
        StartMenu.SetActive(true);
    }

    public void ToLevelSelect()
    {
        levelSelect.SetActive(true);
        StartMenu.SetActive(false);
    }

    public void StarLevel()
    {
        SceneManager.LoadScene("Level1Alpha");
    }


    public void StartLevel1()
    {
        SceneManager.LoadScene("Level1Alpha");
    }

    public void StartLevel2()
    {
        SceneManager.LoadScene("Level2Alpha");
    }

    public void StartLevel3()
    {
        SceneManager.LoadScene("Level3Alpha");
    }

    public void StartLevel4()
    {
        SceneManager.LoadScene("Level4Alpha");
    }
    public void QuitGameApplication()
    {
        #if UNITY_EDITOR
                // If we are in the Unity editor, stop the play mode instead of quitting the game
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                // If the game is running outside of the Unity Editor, quit the application
                Application.Quit();
        #endif
    }
}
