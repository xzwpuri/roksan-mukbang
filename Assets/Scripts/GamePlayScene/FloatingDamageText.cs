using TMPro;
using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float initialUpVelocity = 4f;
    [SerializeField] private float horizontalJitter = 0.4f;
    [SerializeField] private float gravity = 16f;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 1.2f;
    [SerializeField] private float fadeStartTime = 0.25f;

    [Header("Visuals")]
    [SerializeField] private Vector2 spawnOffset = new Vector2(0f, 1.1f);
    [SerializeField] private float fontSize = 10f;
    [SerializeField] private float worldScale = 0.3f;
    [SerializeField] private int sortingOrder = 100;

    private TextMeshPro textMesh;
    private Vector3 velocity;
    private float elapsed;

    public static void Spawn(float damageValue, Vector3 worldPosition, Color? colorOverride = null)
    {
        var go = new GameObject("FloatingDamageText");

        var tmp = go.AddComponent<TextMeshPro>();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = false;   // ✅ 자동 사이즈 끄기

        var effect = go.AddComponent<FloatingDamageText>();
        effect.textMesh = tmp;
        effect.Setup(damageValue, worldPosition, colorOverride ?? Color.white);
    }


    private void Awake()
    {
        textMesh ??= GetComponent<TextMeshPro>();
    }

    public void Setup(float damageValue, Vector3 worldPosition, Color color)
    {
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();

        transform.position = worldPosition + (Vector3)spawnOffset;

        // ✅ 크기 관련
        transform.localScale = Vector3.one * worldScale;  // 전체 스케일 줄이기
        textMesh.fontSize = fontSize;                     // 인스펙터에서 0.5 ~ 1 정도로

        textMesh.text = Mathf.CeilToInt(damageValue).ToString();
        textMesh.color = color;

        var renderer = textMesh.GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.sortingOrder = sortingOrder;

        float randomX = Random.Range(-horizontalJitter, horizontalJitter);
        velocity = new Vector3(randomX, initialUpVelocity, 0f);
        elapsed = 0f;
    }


    private void Update()
    {
        if (textMesh == null)
            return;

        elapsed += Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        velocity += Vector3.down * gravity * Time.deltaTime;

        float fadeT = Mathf.InverseLerp(fadeStartTime, lifetime, elapsed);
        Color c = textMesh.color;
        c.a = Mathf.Lerp(1f, 0f, fadeT);
        textMesh.color = c;

        if (elapsed >= lifetime)
            Destroy(gameObject);
    }
}