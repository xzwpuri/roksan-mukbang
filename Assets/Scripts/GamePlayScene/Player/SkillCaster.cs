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

            TryCast(ref nextQTime, qCooldown, () =>
            {
                // ✅ Q 애니메이션 한 번 재생
                owner.PlayQAnimationOnce();

                // 실제 Q 스킬 로직 (시체 삼키기)
                SkillLibrary.Q_Fixed(owner);
            });
        };
        onR = () =>
        {
            if (owner.Stomach == 0)
            {
                Debug.Log("[Q] stomach이 0이므로 스킬을 사용할 수 없습니다.");
                return;
            }
            TryCast(ref nextRTime, rCooldown, () => SkillLibrary.R_Fixed(owner));
        };
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

        Action wAction = null;
        Action eAction = null;

        switch (stomach)
        {
            case 0: // Default
                wCooldown = 0.5f;
                eCooldown = 1.5f;
                wAction = () => SkillLibrary.W_Default(owner);
                eAction = () => SkillLibrary.E_Default(owner);
                break;

            case 1: // 붕어빵
                wCooldown = 0.2f;
                eCooldown = 0.1f;
                wAction = () => SkillLibrary.W_Bungeobbang(owner);
                eAction = () => SkillLibrary.E_Bungeobbang(owner);
                break;

            case 2: // 콜라
                wCooldown = 4.5f;
                eCooldown = 0.5f;
                wAction = () => SkillLibrary.W_Cola(owner);
                eAction = () => SkillLibrary.E_Cola(owner);
                break;

            case 3: // 감자튀김
                wCooldown = 0.15f;
                eCooldown = 1.0f;
                wAction = () => SkillLibrary.W_Fries(owner);
                eAction = () => SkillLibrary.E_Fries(owner);
                break;

            case 4: // 아이스크림
                wCooldown = 0.5f;
                eCooldown = 3.0f;
                wAction = () => SkillLibrary.W_IceCream(owner);
                eAction = () => SkillLibrary.E_IceCream(owner);
                break;

            case 5: // 고기
                wCooldown = 0.35f;
                eCooldown = 2.5f;
                wAction = () => SkillLibrary.W_Meat(owner);
                eAction = () => SkillLibrary.E_Meat(owner);
                break;

            case 6: // 버섯
                wCooldown = 3.0f;
                eCooldown = 8.0f;
                wAction = () => SkillLibrary.W_Mushroom(owner);
                eAction = () => SkillLibrary.E_Mushroom(owner);
                break;

            case 7: // 물
                wCooldown = 0.5f;
                eCooldown = 2.5f;
                wAction = () => SkillLibrary.W_Water(owner);
                eAction = () => SkillLibrary.E_Water(owner);
                break;

            default:
                Debug.LogWarning($"[SkillCaster] stomach {stomach} 에 대한 W/E 매핑이 없습니다.");
                break;
        }

        if (wAction != null)
        {
            onW = () => TryCast(ref nextWTime, wCooldown, wAction);
            Bus.OnWPressed += onW;
        }

        if (eAction != null)
        {
            onE = () => TryCast(ref nextETime, eCooldown, eAction);
            Bus.OnEPressed += onE;
        }
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