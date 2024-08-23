using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private TextMeshProUGUI coinsCollectedText;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI livesLeftText;
    [SerializeField] private TextMeshProUGUI bossHealthText;

    [SerializeField] private GameObject transitionScreenCanvas;
    [SerializeField] private TextMeshProUGUI transitionEnemiesKilledText;
    [SerializeField] private TextMeshProUGUI transitionCoinsCollectedText;
    [SerializeField] private TextMeshProUGUI transitionLivesLeftText;
    [SerializeField] private Button proceedButton;
    [SerializeField] private Button returnToMainMenuButton;

    private int enemiesKilled = 0;
    private int coinsCollected = 0;
    private int playerHealth = 50;
    private int livesLeft = 3;
    private int bossHealth = 100;
    private bool isCampaignMode = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize UI text elements
        UpdateEnemiesKilledUI();
        UpdateCoinsCollectedUI();
        UpdatePlayerHealthUI();
        UpdateLivesLeftUI();
        bossHealthText.gameObject.SetActive(false); // Hide boss health by default

        transitionScreenCanvas.SetActive(false); // Hide transition screen by default
    }

    // Methods to update game stats and UI
    public void AddEnemyKill()
    {
        enemiesKilled++;
        UpdateEnemiesKilledUI();
    }

    public void AddCoin()
    {
        coinsCollected++;
        UpdateCoinsCollectedUI();
    }

    public void SetPlayerHealth(int health)
    {
        playerHealth = health;
        UpdatePlayerHealthUI();
    }

    public void SetLivesLeft(int lives)
    {
        livesLeft = lives;
        UpdateLivesLeftUI();
    }

    public void SetCampaignMode(bool isCampaign)
    {
        isCampaignMode = isCampaign;
    }

    public int GetLivesLeft()
    {
        return livesLeft;
    }

    public void ShowBossHealth(int health)
    {
        bossHealth = health;
        bossHealthText.gameObject.SetActive(true);
        UpdateBossHealthUI();
    }

    public void UpdateBossHealth(int health)
    {
        bossHealth = health;
        UpdateBossHealthUI();
    }

    public void HideBossHealth()
    {
        bossHealthText.gameObject.SetActive(false);
    }

    // UI update methods
    public void UpdateEnemiesKilledUI()
    {
        enemiesKilledText.text = "Enemies: " + enemiesKilled;
    }

    public void UpdateCoinsCollectedUI()
    {
        coinsCollectedText.text = "Coins: " + coinsCollected;
    }

    public void UpdatePlayerHealthUI()
    {
        playerHealthText.text = "Health: " + playerHealth;
    }

    public void UpdateLivesLeftUI()
    {
        livesLeftText.text = "Lives: " + livesLeft;
    }

    public void UpdateBossHealthUI()
    {
        bossHealthText.text = "Boss: " + bossHealth;
    }

    public void ShowTransitionScreen(bool playerDied)
    {
        transitionScreenCanvas.SetActive(true);
        transitionEnemiesKilledText.text = "Enemies Killed: " + enemiesKilled;
        transitionCoinsCollectedText.text = "Coins Collected: " + coinsCollected;
        transitionLivesLeftText.text = "Lives Left: " + livesLeft;

        if (playerDied)
        {
            proceedButton.gameObject.SetActive(false);
            returnToMainMenuButton.gameObject.SetActive(true);
            returnToMainMenuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Return to Main Menu";
        }
        else if (isCampaignMode)
        {
            proceedButton.gameObject.SetActive(true);
            proceedButton.GetComponentInChildren<TextMeshProUGUI>().text = "Proceed to Next Level";
            proceedButton.onClick.AddListener(LoadNextLevel);

            returnToMainMenuButton.gameObject.SetActive(true);
            returnToMainMenuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Quit to Main Menu";
            returnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
        else
        {
            proceedButton.gameObject.SetActive(false);
            returnToMainMenuButton.gameObject.SetActive(true);
            returnToMainMenuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Return to Main Menu";
            returnToMainMenuButton.onClick.AddListener(ReturnToMainMenu);
        }
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("0_IntroScreen");
    }
}