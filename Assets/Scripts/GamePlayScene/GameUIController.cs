using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Player player;
    [SerializeField] private SkillCaster skillCaster;

    [Header("Timer UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Health UI")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Cooldown Gauges")]
    [SerializeField] private Image qCooldownFill;
    [SerializeField] private TextMeshProUGUI qCooldownText;
    [SerializeField] private Image wCooldownFill;
    [SerializeField] private TextMeshProUGUI wCooldownText;
    [SerializeField] private Image eCooldownFill;
    [SerializeField] private TextMeshProUGUI eCooldownText;
    [SerializeField] private Image rCooldownFill;
    [SerializeField] private TextMeshProUGUI rCooldownText;

    private void OnEnable()
    {
        if (player != null)
        {
            player.OnHealthChanged += UpdateHealthUI;
            UpdateHealthUI(player.Hp, player.MaxHp);
        }

        UpdateTimerUI();
        UpdateCooldownUI();
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void Awake()
    {
        if (player == null)
            player = FindObjectOfType<Player>();

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (skillCaster == null && player != null)
            skillCaster = player.GetComponent<SkillCaster>();
    }

    private void Update()
    {
        UpdateTimerUI();
        UpdateCooldownUI();
    }

    private void UpdateTimerUI()
    {
        if (gameManager == null || timerText == null)
            return;

        float remaining = Mathf.Max(0f, gameManager.RemainingTime);
        TimeSpan ts = TimeSpan.FromSeconds(remaining);
        timerText.text = $"{ts.Minutes:00}:{ts.Seconds:00}";
    }

    private void UpdateHealthUI(float currentHp, float maxHp)
    {
        if (healthFill != null && maxHp > 0f)
        {
            healthFill.fillAmount = Mathf.Clamp01(currentHp / maxHp);
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(currentHp)}/{Mathf.CeilToInt(maxHp)}";
        }
    }

    private void UpdateCooldownUI()
    {
        if (skillCaster == null)
            return;

        UpdateCooldownSlot(qCooldownFill, qCooldownText, skillCaster.GetQCooldownRatio(), skillCaster.GetQRemainingCooldown());
        UpdateCooldownSlot(wCooldownFill, wCooldownText, skillCaster.GetWCooldownRatio(), skillCaster.GetWRemainingCooldown());
        UpdateCooldownSlot(eCooldownFill, eCooldownText, skillCaster.GetECooldownRatio(), skillCaster.GetERemainingCooldown());
        UpdateCooldownSlot(rCooldownFill, rCooldownText, skillCaster.GetRCooldownRatio(), skillCaster.GetRRemainingCooldown());
    }

    private void UpdateCooldownSlot(Image fill, TextMeshProUGUI text, float ratio, float remainingSeconds)
    {
        if (fill != null)
        {
            fill.fillAmount = Mathf.Clamp01(ratio);
        }

        if (text != null)
        {
            text.text = remainingSeconds > 0.01f ? $"{remainingSeconds:0.0}" : string.Empty;
        }
    }
}