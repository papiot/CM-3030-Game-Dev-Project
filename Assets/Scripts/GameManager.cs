using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextMeshProUGUI enemiesKilledText;
    [SerializeField] private TextMeshProUGUI coinsCollectedText;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI livesLeftText;
    [SerializeField] private TextMeshProUGUI bossHealthText;

    private int enemiesKilled = 0;
    private int coinsCollected = 0;
    private int playerHealth = 30;
    private int livesLeft = 3;
    private int bossHealth = 100;

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
}