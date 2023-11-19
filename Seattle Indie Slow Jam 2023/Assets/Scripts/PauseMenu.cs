using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingsMenuUI;


    // Update is called once per frame
    void Update()
    {
        if(!GameManager.GameHasEnded)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameIsPaused)
                {
                    Resume(true);
                }
                else
                {
                    Pause();
                }
            }
        }
    }

    public void Resume(bool cursorLock)
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        PlayerManager.CameraCanMove = true;
        if (cursorLock == true) 
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        settingsMenuUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        PlayerManager.CameraCanMove = false;
    }

    public void RestartLevel()
    {
        Resume(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadTitle()
    {
        Resume(false);
        SceneManager.LoadScene("TitleScreen");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
