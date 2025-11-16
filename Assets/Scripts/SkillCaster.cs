// SkillCaster.cs (신규)
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

    private void Awake()
    {
        owner = GetComponent<Player>();

        // Q/R 고정 바인딩(델리게이트 생성)
        onQ = () => SkillLibrary.Q_Fixed(owner);
        onR = () => SkillLibrary.R_Fixed(owner);
    }

    private void OnEnable()
    {
        // 입력 이벤트 버스 구독
        Bus.OnQPressed += onQ;
        Bus.OnRPressed += onR;

        // 현재 element 기준으로 W/E 등록
        BindWEForElement(owner.Element);
    }

    private void OnDisable()
    {
        // 고정 해제
        Bus.OnQPressed -= onQ;
        Bus.OnRPressed -= onR;

        // W/E 해제
        UnbindWE();
    }

    // Player에서 element 변경 시 호출해줘
    public void RefreshLoadout() => BindWEForElement(owner.Element);

    private void BindWEForElement(int element)
    {
        // 기존 바인딩 제거
        UnbindWE();

        switch (element)
        {
            case 0:
                onW = () => SkillLibrary.W_Default(owner);
                onE = () => SkillLibrary.E_Default (owner);
                break;
            case 1:
                onW = () => SkillLibrary.W_Fire(owner);
                onE = () => SkillLibrary.E_Fire(owner);
                break;
            case 2:
                onW = () => SkillLibrary.W_Water(owner);
                onE = () => SkillLibrary.E_Water(owner);
                break;
            case 3:
                onW = () => SkillLibrary.W_Grass(owner);
                onE = () => SkillLibrary.E_Grass(owner);
                break;
            default:
                onW = null;
                onE = null;
                Debug.LogWarning($"[SkillCaster] element {element} 에 대한 W/E 매핑이 없습니다.");
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
}
