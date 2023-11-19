using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMainMenu : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    private void Update()
    {
        if (LevelComplete.gameFinished)
        {
            GameObject C = GameObject.Find("Canvas");
            GameObject mm = C.transform.Find("MainMenu").gameObject;
            mm.SetActive(false);
            GameObject credit = C.transform.Find("CreditsMenu").gameObject;
            credit.SetActive(true);
            LevelComplete.gameFinished = false;
        }
    }
}
