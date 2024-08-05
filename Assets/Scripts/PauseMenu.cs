using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuCanvas; // Reference to the Pause Menu Canvas
    public GameObject pauseMenuPanel; // Reference to the Pause Menu Panel
    public Button resumeButton; // Reference to the Resume Button
    public Button quitButton; // Reference to the Quit Button

    private bool isPaused = false;

    void Start()
    {
        // Ensure the pause menu UI is initially inactive
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("PauseMenuCanvas is not assigned in the inspector!");
        }

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("PauseMenuPanel is not assigned in the inspector!");
        }

        // Assign button listeners
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(Resume);
        }
        else
        {
            Debug.LogError("ResumeButton is not assigned in the inspector!");
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitToMainMenu);
        }
        else
        {
            Debug.LogError("QuitButton is not assigned in the inspector!");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(true);
        }

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        Time.timeScale = 0f;
        isPaused = true;
    }


    public void QuitToMainMenu()
    {
        // Deactivate pause menu UI
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        Time.timeScale = 1f; // Reset time scale
        isPaused = false; // Reset pause state

        // Load the main menu scene
        SceneManager.LoadScene("0_IntroScreen");
    }
}