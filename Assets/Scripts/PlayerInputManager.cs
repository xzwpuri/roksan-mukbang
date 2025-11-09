using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-500)]
//싱글톤 패턴
public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance { get; private set; }

    private PlayerInputAction actions;

    public InputAction Q;
    public InputAction W;
    public InputAction E;
    public InputAction R;
    public InputAction RMC;

    public event Action OnQPressed;
    public event Action OnWPressed;
    public event Action OnEPressed;
    public event Action OnRPressed;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;

        // 여기서 생성/바인딩(존재 보장 단계)
        actions = new PlayerInputAction();
        Q = actions.Player.QSkill;
        W = actions.Player.WSkill;
        E = actions.Player.ESkill;
        R = actions.Player.RSkill;
        RMC = actions.Player.RClick;
    }

    private void OnEnable()
    {
        //  여기서 Enable + 구독
        actions.Enable();
        Q.Enable(); W.Enable(); E.Enable(); R.Enable(); RMC.Enable();

        Q.performed += HandleQ;
        W.performed += HandleW;
        E.performed += HandleE;
        R.performed += HandleR;
    }

    private void OnDisable()
    {
        // 여기서 해제
        Q.performed -= HandleQ;
        W.performed -= HandleW;
        E.performed -= HandleE;
        R.performed -= HandleR;

        Q.Disable(); W.Disable(); E.Disable(); R.Disable(); RMC.Disable();
        actions.Disable();
    }

    private void HandleQ(InputAction.CallbackContext _) => OnQPressed?.Invoke();
    private void HandleW(InputAction.CallbackContext _) => OnWPressed?.Invoke();
    private void HandleE(InputAction.CallbackContext _) => OnEPressed?.Invoke();
    private void HandleR(InputAction.CallbackContext _) => OnRPressed?.Invoke();
}
