using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelectUI : MonoBehaviour
{
    [SerializeField] private string gamePlaySceneName = "GamePlayScene";

    public void OnEasyButton()
    {
        GameConfig.SelectedDifficulty = Difficulty.Easy;
        SceneManager.LoadScene(gamePlaySceneName);
    }

    public void OnNormalButton()
    {
        GameConfig.SelectedDifficulty = Difficulty.Normal;
        SceneManager.LoadScene(gamePlaySceneName);
    }

    public void OnHardButton()
    {
        GameConfig.SelectedDifficulty = Difficulty.Hard;
        SceneManager.LoadScene(gamePlaySceneName);
    }
}