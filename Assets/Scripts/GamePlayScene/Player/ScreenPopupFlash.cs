using System.Collections;
using UnityEngine;

public class ScreenPopupFlash : MonoBehaviour
{
    [Header("투명도 설정")]
    [Range(0f, 1f)]
    [SerializeField] private float initialAlpha = 0.6f; // 처음부터 약간 반투명

    [Header("시간 설정")]
    [SerializeField] private float popDuration = 0.15f; // 팍! 커지는 시간
    [SerializeField] private float holdDuration = 0.1f; // 화면 꽉 찬 상태로 유지 시간
    [SerializeField] private float fadeDuration = 0.3f; // 페이드아웃 시간

    [Header("스케일 설정")]
    [SerializeField] private bool fitToCamera = true;   // 카메라 기준으로 화면 꽉 채우기
    [SerializeField] private float extraScaleMargin = 1.1f; // 화면보다 살짝 크게 (여백 방지)

    [Header("기타 옵션")]
    [SerializeField] private bool destroyOnEnd = true; // 효과 끝나면 오브젝트 삭제

    private SpriteRenderer spriteRenderer;
    private CanvasGroup canvasGroup; // UI용으로도 쓰고 싶으면 사용
    private Vector3 startScale;
    private Vector3 targetScale;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        canvasGroup = GetComponent<CanvasGroup>();
        startScale = transform.localScale;
    }

    private void Start()
    {
        // 화면 꽉 채우는 스케일 계산
        if (fitToCamera)
            CalculateTargetScaleFromCamera();
        else
            targetScale = startScale;

        // 생성되자마자 반투명 적용
        SetAlpha(initialAlpha);

        // 효과 실행
        StartCoroutine(EffectCoroutine());
    }

    private void CalculateTargetScaleFromCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("[ScreenPopupFlash] Camera.main 을 찾을 수 없음.");
            targetScale = startScale;
            return;
        }

        // 화면 중앙으로 위치 고정
        Vector3 pos = transform.position;
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, pos.z);

        if (spriteRenderer == null)
        {
            targetScale = startScale;
            return;
        }

        // 현재 스케일 기준 스프라이트의 월드 크기
        Vector2 spriteSize = spriteRenderer.bounds.size;

        // 카메라가 보여주는 월드 높이만 사용 (세로 기준)
        float worldHeight = cam.orthographicSize * 2f;

        // 🔸 세로(높이) 기준으로만 맞춤
        float scaleFactor = (worldHeight / spriteSize.y) * extraScaleMargin;

        targetScale = startScale * scaleFactor;
    }


    private IEnumerator EffectCoroutine()
    {
        // 1) 팍! 하고 커지는 구간
        float t = 0f;
        while (t < popDuration)
        {
            float normalized = t / popDuration;
            // 부드러운 이징 (EaseOutCubic 느낌)
            float eased = 1f - Mathf.Pow(1f - normalized, 3f);

            transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, eased);

            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;

        // 2) 꽉 찬 상태로 잠깐 정지
        if (holdDuration > 0f)
            yield return new WaitForSeconds(holdDuration);

        // 3) 페이드아웃
        t = 0f;
        while (t < fadeDuration)
        {
            float normalized = t / fadeDuration;
            float alpha = Mathf.Lerp(initialAlpha, 0f, normalized);
            SetAlpha(alpha);

            t += Time.deltaTime;
            yield return null;
        }
        SetAlpha(0f);

        if (destroyOnEnd)
            Destroy(gameObject);
    }

    private void SetAlpha(float a)
    {
        // SpriteRenderer용
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = a;
            spriteRenderer.color = c;
        }

        // UI(CanvasGroup)용
        if (canvasGroup != null)
        {
            canvasGroup.alpha = a;
        }
    }
}
