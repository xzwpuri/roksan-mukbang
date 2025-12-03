using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void OnClickStart()
    {
        SceneManager.LoadScene("ChooseDifficulty");
    }

    public void OnClickStory()
    {
        SceneManager.LoadScene("Story1");
    }

    public void OnClickQuitGame()
    {
        // 에디터에서 테스트할 때는 플레이 모드 해제
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서는 앱 종료
        Application.Quit();
#endif
    }
}
