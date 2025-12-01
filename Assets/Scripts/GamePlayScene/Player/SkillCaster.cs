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
    [SerializeField] private float rCooldown = 2f;

    // 다음 사용 가능 시간(Time.time 기준)
    private float nextQTime;
    private float nextWTime;
    private float nextETime;
    private float nextRTime;

    public float QCooldown => qCooldown;
    public float WCooldown => wCooldown;
    public float ECooldown => eCooldown;
    public float RCooldown => rCooldown;

    private void Awake()
    {
        owner = GetComponent<Player>();

        // Q/R 고정 바인딩(쿨타임 포함)
        onQ = () =>
        {
            if (owner.Stomach != 0)
            {
                Debug.Log("[Q] stomach이 0이 아니므로 스킬을 사용할 수 없습니다.");
                return;
            }

            TryCast(ref nextQTime, qCooldown, () => SkillLibrary.Q_Fixed(owner));
        };
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
            case 0: // Default
                onW = () => TryCast(ref nextWTime, 0.5f, () => SkillLibrary.W_Default(owner));
                onE = () => TryCast(ref nextETime, 1.5f, () => SkillLibrary.E_Default(owner));
                break;

            case 1: // 붕어빵
                onW = () => TryCast(ref nextWTime, 0.8f, () => SkillLibrary.W_Bungeobbang(owner));
                onE = () => TryCast(ref nextETime, 0.1f, () => SkillLibrary.E_Bungeobbang(owner));
                break;

            case 2: // 콜라
                onW = () => TryCast(ref nextWTime, 4.5f, () => SkillLibrary.W_Cola(owner));
                onE = () => TryCast(ref nextETime, 0.5f, () => SkillLibrary.E_Cola(owner));
                break;

            case 3: // 감자튀김
                onW = () => TryCast(ref nextWTime, 0.15f, () => SkillLibrary.W_Fries(owner));
                onE = () => TryCast(ref nextETime, 4.0f, () => SkillLibrary.E_Fries(owner));
                break;

            case 4: // 아이스크림
                onW = () => TryCast(ref nextWTime, 1.5f, () => SkillLibrary.W_IceCream(owner));
                onE = () => TryCast(ref nextETime, 5.0f, () => SkillLibrary.E_IceCream(owner));
                break;

            case 5: // 고기
                onW = () => TryCast(ref nextWTime, 0.65f, () => SkillLibrary.W_Meat(owner));
                onE = () => TryCast(ref nextETime, 4.5f, () => SkillLibrary.E_Meat(owner));
                break;

            case 6: // 버섯
                onW = () => TryCast(ref nextWTime, 2.0f, () => SkillLibrary.W_Mushroom(owner));
                onE = () => TryCast(ref nextETime, 8.0f, () => SkillLibrary.E_Mushroom(owner));
                break;

            case 7: // 물
                onW = () => TryCast(ref nextWTime, 1.0f, () => SkillLibrary.W_Water(owner));
                onE = () => TryCast(ref nextETime, 5.5f, () => SkillLibrary.E_Water(owner));
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
    public float GetWRemainingCooldown()
    {
        return Mathf.Max(0f, nextWTime - Time.time);
    }

    public float GetECooldownRatio()
    {
        float remain = nextETime - Time.time;
        if (remain <= 0) return 0f;
        return remain / eCooldown;
    }

    public float GetERemainingCooldown()
    {
        return Mathf.Max(0f, nextETime - Time.time);
    }


    public float GetQCooldownRatio()
    {
        float remain = nextQTime - Time.time;
        if (remain <= 0) return 0f;
        return remain / qCooldown;
    }

    public float GetQRemainingCooldown()
    {
        return Mathf.Max(0f, nextQTime - Time.time);
    }

    public float GetRCooldownRatio()
    {
        float remain = nextRTime - Time.time;
        if (remain <= 0) return 0f;
        return remain / rCooldown;
    }

    public float GetRRemainingCooldown()
    {
        return Mathf.Max(0f, nextRTime - Time.time);
    }
}