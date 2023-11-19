using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-900)]
public class GameManager : MonoBehaviour
{
    bool gameHasEnded = false;

    public float restartDelay = 1f;

    public UnityEvent OnGameOver;
    public UnityEvent OnLevelComplete;
    public static UnityEvent s_OnGameOver;
    public static UnityEvent s_OnLevelComplete;

    public void EndGame()
    {
        if (gameHasEnded == false)
        {
            gameHasEnded = true;
            Invoke("Restart", restartDelay);
        }
    }

    //git shenanigans
    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #region skeletor

    // allows scripts to access game manager in any context
    private static GameManager s_instance;
    // public read only accessor for if game has ended
    public static bool GameHasEnded => s_instance.gameHasEnded;
    public static float VolumeLevel => s_instance._volumeLevel;
    // reference to the current scene loaded
    private Scene _activeScene;
    // if set to true, this scene will call object loaders. Should only be set to false for menu scenes
    [SerializeField] private bool _loadPrefabs;
    private float _volumeLevel = 1;
    

    // called before first frame
    public void Awake()
    {
        s_OnGameOver = OnGameOver;
        s_OnLevelComplete = OnLevelComplete;
        if(_loadPrefabs)
        {
            ObjectLoader.InstantiateLoaders();
        }
        // if a game manager is already been assigned, I should remove myself
        if(s_instance != null)
        {
            gameObject.SetActive(false);
        }
        // if no game manager has been assigned, then I should assign myself as game manager and presist between scenes
        else
        {
            s_instance = this;
            SceneManager.sceneLoaded += OnSceneChanged;
            DontDestroyOnLoad(gameObject);
        }
    }

    // called when some trigger would have ended the game, handles the end of the game
    public static void TriggerGameOver()
    {
        if(!s_instance.gameHasEnded)
        {
            s_instance.EndGame();
            s_OnGameOver!.Invoke();
        }
    }

    // called when some trigger would complete the level
    public static void TriggerLevelComplete()
    {
        s_instance.gameHasEnded = true;
        s_OnLevelComplete!.Invoke();
    }

    // called whenever the scene is changed, think of it as a replacement for Start()
    public void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        gameHasEnded = false;
        _activeScene = scene;
    }

    public static void SetAudioLevel(float level)
    {
        s_instance._volumeLevel = level;
        AudioListener.volume = level;
    }


    #endregion

}
