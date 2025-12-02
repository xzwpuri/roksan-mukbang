using UnityEngine;

public class FadeManager : MonoBehaviour
{
    private static FadeManager instance;
    public static FadeManager Instance => instance;

    public CanvasGroup fade;
    public float fadeDuration = 2f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void FadeOut(System.Action onComplete)
    {

    }
}
