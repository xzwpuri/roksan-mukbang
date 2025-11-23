using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class SkillCaster : MonoBehaviour
{
    private Player owner;
    private PlayerInputManager Bus => PlayerInputManager.instance;

    // 고정 슬롯 핸들러
    private Action onQ;
    private Action onR;

    // 가변(W/E) 현재 핸들러
    private Action onW;
    private Action onE;

    // === 쿨타임 설정 ===
    [Header("쿨타임(초)")]
    [SerializeField] private float qCooldown = 2f;
    [SerializeField] private float wCooldown = 3f;
    [SerializeField] private float eCooldown = 4f;
    [SerializeField] private float rCooldown = 10f;

    // 다음 사용 가능 시간(Time.time 기준)
    private float nextQTime;
    private float nextWTime;
    private float nextETime;
    private float nextRTime;

    private void Awake()
    {
        owner = GetComponent<Player>();

        // Q/R 고정 바인딩(쿨타임 포함)
        onQ = () => TryCast(ref nextQTime, qCooldown, () => SkillLibrary.Q_Fixed(owner));
        onR = () => TryCast(ref nextRTime, rCooldown, () => SkillLibrary.R_Fixed(owner));
    }

    private void OnEnable()
    {
        Bus.OnQPressed += onQ;
        Bus.OnRPressed += onR;

        // 현재 stomach 기준으로 W/E 등록
        BindWEForElement(owner.stomach);
    }

    private void OnDisable()
    {
        Bus.OnQPressed -= onQ;
        Bus.OnRPressed -= onR;
        UnbindWE();
    }

    public void RefreshLoadout() => BindWEForElement(owner.stomach);

    // 공통 쿨타임 처리 로직
    private void TryCast(ref float nextTime, float cooldown, Action castAction)
    {
        if (Time.time < nextTime)
        {
            Debug.Log($"쿨타임 남음: {nextTime - Time.time:0.00}초");
            return;
        }

        castAction?.Invoke();
        nextTime = Time.time + cooldown;
    }

    private void BindWEForElement(int stomach)
    {
        UnbindWE();

        switch (stomach)
        {
            case 0:
                onW = () => TryCast(ref nextWTime, 0.5f, () => SkillLibrary.W_Default(owner));
                onE = () => TryCast(ref nextETime, 2f, () => SkillLibrary.E_Default(owner));
                break;
            case 1:
                onW = () => TryCast(ref nextWTime, wCooldown, () => SkillLibrary.W_Fire(owner));
                onE = () => TryCast(ref nextETime, eCooldown, () => SkillLibrary.E_Fire(owner));
                break;
            case 2:
                onW = () => TryCast(ref nextWTime, wCooldown, () => SkillLibrary.W_Water(owner));
                onE = () => TryCast(ref nextETime, eCooldown, () => SkillLibrary.E_Water(owner));
                break;
            case 3:
                onW = () => TryCast(ref nextWTime, wCooldown, () => SkillLibrary.W_Grass(owner));
                onE = () => TryCast(ref nextETime, eCooldown, () => SkillLibrary.E_Grass(owner));
                break;

            default:
                onW = null;
                onE = null;
                Debug.LogWarning($"[SkillCaster] stomach {stomach} 에 대한 W/E 매핑이 없습니다.");
                break;
        }

        if (onW != null) Bus.OnWPressed += onW;
        if (onE != null) Bus.OnEPressed += onE;
    }

    private void UnbindWE()
    {
        if (onW != null) { Bus.OnWPressed -= onW; onW = null; }
        if (onE != null) { Bus.OnEPressed -= onE; onE = null; }
    }

    public float GetWCooldownRatio()
    {
        float remain = nextWTime - Time.time;
        if (remain <= 0) return 0f;
        return remain / wCooldown;   // 0~1
    }
}
