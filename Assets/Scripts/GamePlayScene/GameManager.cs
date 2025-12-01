using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}

[Serializable]
public struct DifficultySetting
{
    public Difficulty difficulty;
    public float survivalTime;
    public float spawnInterval;
    public int baseSpawnCount;
    public float enemyHpMultiplier;
    public float enemyMoveSpeedMultiplier;
}

public class GameManager : MonoBehaviour
{
    [Header("Difficulty Settings")]
    [SerializeField] private Difficulty difficulty = Difficulty.Normal;
    [SerializeField] private DifficultySetting[] difficultySettings;

    [Header("References")]
    [SerializeField] private EnemyGenerator enemyGenerator;
    [SerializeField] private Player player;
    [SerializeField] private ScreenFadeTransition screenFadeTransition;

    [Header("Scene Transitions")]
    [SerializeField] private string gameOverSceneName = "GamePlayScene";
    [SerializeField] private string gameClearSceneName = "GamePlayScene";
    [SerializeField] private Color gameOverFadeColor = Color.black;
    [SerializeField] private Color gameClearFadeColor = Color.white;

    [Header("Game State")]
    [SerializeField] private float remainingTime;
    private DifficultySetting? activeSetting;
    private bool isGameActive;
    private bool isGameOver;

    public float RemainingTime => remainingTime;
    public float SurvivalTime => activeSetting?.survivalTime ?? remainingTime;
    public bool IsGameActive => isGameActive;

    private void Awake()
    {
        if (screenFadeTransition == null)
            screenFadeTransition = FindObjectOfType<ScreenFadeTransition>();
    }

    private void Start()
    {
        ApplyDifficulty(difficulty);
        BeginGame();
    }

    private void Update()
    {
        if (!isGameActive || isGameOver)
            return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            HandleGameClear();
        }
        else if (player == null || player.Hp <= 0f)
        {
            HandleGameOver();
        }
    }

    public void ApplyDifficulty(Difficulty newDifficulty)
    {
        difficulty = newDifficulty;
        activeSetting = difficultySettings.FirstOrDefault(x => x.difficulty == difficulty);
        if (activeSetting.HasValue)
        {
            remainingTime = activeSetting.Value.survivalTime;
            if (enemyGenerator != null)
            {
                enemyGenerator.ConfigureDifficulty(
                    activeSetting.Value.spawnInterval,
                    activeSetting.Value.baseSpawnCount,
                    activeSetting.Value.enemyHpMultiplier,
                    activeSetting.Value.enemyMoveSpeedMultiplier
                );
            }
        }
        else
        {
            Debug.LogWarning($"Difficulty setting for {difficulty} not found.");
        }
    }

    public void BeginGame()
    {
        if (!activeSetting.HasValue)
        {
            Debug.LogWarning("No difficulty configured. Call ApplyDifficulty first.");
            return;
        }

        isGameActive = true;
        isGameOver = false;
        if (enemyGenerator != null)
        {
            enemyGenerator.BeginSpawn();
        }
    }

    private void HandleGameOver()
    {
        if (isGameOver)
           return;

        isGameOver = true;
        isGameActive = false;
        if (enemyGenerator != null)
        {
            enemyGenerator.StopSpawn();
        }
        Debug.Log("Game Over");
        // TODO: 게임 오버 UI/씬 전환 처리
        string targetScene = string.IsNullOrEmpty(gameOverSceneName)
                ? SceneManager.GetActiveScene().name
                : gameOverSceneName;

        if (screenFadeTransition != null)
        {
            screenFadeTransition.FadeToScene(gameOverFadeColor, targetScene);
        }
        else
        {
            SceneManager.LoadScene(targetScene);
        }
    }

    private void HandleGameClear()
    {
        if (isGameOver)
            return;
        
        isGameOver = true;
        isGameActive = false;
        if (enemyGenerator != null)
        {
            enemyGenerator.StopSpawn();
        }
        Debug.Log("Game Clear");
        // TODO: 클리어 연출 및 다음 단계 처리
        string targetScene = string.IsNullOrEmpty(gameClearSceneName)
            ? SceneManager.GetActiveScene().name
            : gameClearSceneName;

        if (screenFadeTransition != null)
        {
            screenFadeTransition.FadeToScene(gameClearFadeColor, targetScene);
        }
        else
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}