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
            // Assuming 0 is the build index for the main menu or intro
            SceneManager.LoadSceneAsync(0);
        }
        else
        {
            SceneManager.LoadSceneAsync(currentSceneIndex + 1);
        }
    }

    public void LoadNextLevelWithDelay()
    {
        Invoke("LoadNextLevel", 0.5f);
    }

    public void RestartScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(currentSceneIndex);
    }

    public void RestartLevelWithDelay()
    {
        Invoke("RestartScene", 0.5f);
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
    public void StartCampaign()
    {
        SceneManager.LoadScene("1_Level 1"); // Load Level 1 for Start Campaign
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("1_Level 1"); // Load Level 1 for Start Level 1
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("2_Level 2"); // Load Level 1 for Start Level 2
    }

}
