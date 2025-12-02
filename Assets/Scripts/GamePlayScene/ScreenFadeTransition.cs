using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScreenFadeTransition : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Canvas overlayCanvas;
    private Image overlayImage;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        EnsureOverlay();
    }

    public void FadeToScene(Color overlayColor, string sceneName)
    {
        EnsureOverlay();

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeRoutine(overlayColor, sceneName));
    }

    private IEnumerator FadeRoutine(Color overlayColor, string sceneName)
    {
        Color transparent = overlayColor;
        transparent.a = 0f;
        overlayImage.color = transparent;
        overlayImage.enabled = true;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float t = fadeCurve != null ? fadeCurve.Evaluate(elapsed / fadeDuration) : elapsed / fadeDuration;
            float alpha = Mathf.Lerp(0f, 1f, t);
            overlayImage.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        overlayImage.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, 1f);
        fadeRoutine = null;

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void EnsureOverlay()
    {
        if (overlayCanvas != null && overlayImage != null)
            return;

        GameObject canvasObj = new GameObject("ScreenFadeCanvas");
        canvasObj.transform.SetParent(transform, false);
        overlayCanvas = canvasObj.AddComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        overlayCanvas.sortingOrder = 5000;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("FadeOverlay");
        imageObj.transform.SetParent(canvasObj.transform, false);
        overlayImage = imageObj.AddComponent<Image>();
        overlayImage.raycastTarget = false;
        overlayImage.rectTransform.anchorMin = Vector2.zero;
        overlayImage.rectTransform.anchorMax = Vector2.one;
        overlayImage.rectTransform.offsetMin = Vector2.zero;
        overlayImage.rectTransform.offsetMax = Vector2.zero;
        overlayImage.enabled = false;
    }
}