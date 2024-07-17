using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private SceneTransition sceneManager;
    // Start is called before the first frame update
    void Start()
    {
        sceneManager = GameObject.Find("Scene Manager").GetComponent<SceneTransition>();
    }


    public void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalSceneCount = SceneManager.sceneCountInBuildSettings;

        if (currentSceneIndex == totalSceneCount - 1)
        {
            SceneManager.LoadSceneAsync(0);
        }
        else
        {
            SceneManager.LoadSceneAsync(currentSceneIndex + 1);
        }
    }

    public void RestartScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(currentSceneIndex);
    }

    public void PauseUnpauseScene()
    {
        if(Time.timeScale == 1)
        {
            Time.timeScale = 0;
            // create a screen overlay for paused game in Unity Editor.
        }
        else if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        
    }

}
