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

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
