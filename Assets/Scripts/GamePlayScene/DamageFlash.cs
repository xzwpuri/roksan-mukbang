using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [Header("세팅")]
    [SerializeField] private Color flashColor = Color.red; // 맞을 때 색
    [SerializeField] private float flashDuration = 0.1f;   // 빨간색 유지 시간
    [SerializeField] private SpriteRenderer[] spriteRenderers; // 비워두면 자동으로 자식까지 찾음

    private Color[] originalColors;
    private Coroutine flashRoutine;

    private void Awake()
    {
        // spriteRenderers 수동 세팅 안 했으면, 자식 포함해서 전부 찾기
        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        }

        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                originalColors[i] = spriteRenderers[i].color;
        }
    }

    /// <summary>
    /// 외부에서 이 함수만 호출해주면 됨 (데미지 입었을 때)
    /// </summary>
    public void PlayFlash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        // 빨간색으로 변경
        SetColor(flashColor);

        yield return new WaitForSeconds(flashDuration);

        // 원래 색으로 복구
        RestoreColors();

        flashRoutine = null;
    }

    private void SetColor(Color c)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                spriteRenderers[i].color = c;
        }
    }

    private void RestoreColors()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] != null)
                spriteRenderers[i].color = originalColors[i];
        }
    }
}
