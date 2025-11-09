using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance { get; private set; }
    private PlayerInputAction actions;

    // 외부에서 이벤트 구독할 수 있게 InputAction 그대로 공개
    public InputAction Q;
    public InputAction W;
    public InputAction E;
    public InputAction R;
    public InputAction RMC;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;

        actions = new PlayerInputAction(); // 여기서 생성
        Q   = actions.Player.QSkill;
        W   = actions.Player.WSkill;
        E   = actions.Player.ESkill;
        R   = actions.Player.RSkill;
        RMC = actions.Player.RClick;
    }

    private void OnEnable()
    {
        actions.Enable(); // 혹은 필요 액션만 Enable
        // Q.Enable(); W.Enable(); E.Enable(); R.Enable(); RMC.Enable();
    }

    private void OnDisable()
    {
        actions.Disable();
        // Q.Disable(); W.Disable(); E.Disable(); R.Disable(); RMC.Disable();
    }
}
