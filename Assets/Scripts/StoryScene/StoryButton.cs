using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryButton : MonoBehaviour
{
    IEnumerator Async(string async)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(async);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void LoadStory(string scene)
    {
        StartCoroutine(Async(scene));
    }

    public void QuitStory()
    {
        SceneManager.LoadScene("Start");
    }
}
