using System.Collections;
using UnityEngine;
// TextMeshPro 쓸 거면 주석 해제
// using TMPro;

public class BuffEffect : MonoBehaviour
{
    [Header("기본 설정")]
    [SerializeField] private float lifeTime = 0.8f;   // 전체 재생 시간
    [SerializeField] private float moveUpSpeed = 1.5f; // 위로 올라가는 속도 (유닛/초)

    [Header("페이드 아웃 옵션")]
    [SerializeField] private bool fadeOut = true;     // 알파값 점점 줄이기

    private SpriteRenderer[] spriteRenderers;
    // TextMeshProUGUI / TextMeshPro 둘 중 쓰는 거에 맞게 사용
    // private TMP_Text[] tmpTexts;

    private void Awake()
    {
        // 자식까지 포함해서 스프라이트 렌더러 전부 가져오기
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        // tmpTexts = GetComponentsInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        StartCoroutine(FloatingRoutine());
    }

    private IEnumerator FloatingRoutine()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < lifeTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lifeTime; // 0 ~ 1

            // 위로 조금씩 이동
            transform.position = startPos + Vector3.up * (moveUpSpeed * elapsed);

            if (fadeOut)
            {
                float alpha = Mathf.Lerp(1f, 0f, t);

                // SpriteRenderer 알파 줄이기
                foreach (var sr in spriteRenderers)
                {
                    if (sr == null) continue;
                    var c = sr.color;
                    c.a = alpha;
                    sr.color = c;
                }

                // TMP 텍스트도 쓰면 같이 페이드아웃
                /*
                foreach (var tmp in tmpTexts)
                {
                    if (tmp == null) continue;
                    var c = tmp.color;
                    c.a = alpha;
                    tmp.color = c;
                }
                */
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
