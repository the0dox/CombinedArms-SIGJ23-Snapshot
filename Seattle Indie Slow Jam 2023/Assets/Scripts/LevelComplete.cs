using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
 public static bool gameFinished = false;
 public void LoadNextLevel()
    {
        int last = SceneManager.GetActiveScene().buildIndex;
        int next = last + 1 >= SceneManager.sceneCountInBuildSettings ? 0 : last + 1;
        SceneManager.LoadScene(next);
        if (next - last < 0) gameFinished = true;
    }
}
