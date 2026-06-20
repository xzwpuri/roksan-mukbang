using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToStart : MonoBehaviour
{
    [SerializeField] private string startSceneName = "Start"; 

    public void OnClickReturnToStart()
    {
        // 혹시 타임스케일이 멈춰있을 수도 있으니까 초기화
        Time.timeScale = 1f;
        
        SceneManager.LoadScene(startSceneName);
    }
}