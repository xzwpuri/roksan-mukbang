using UnityEngine;
using UnityEngine.InputSystem;

//ΩÃ±€≈Ê ∆–≈œ

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance {  get; private set; }
    private PlayerInputAction actions;

    public InputAction Q;
    public InputAction W;
    public InputAction E;
    public InputAction R;
    public InputAction RMC;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        actions = new PlayerInputAction();
        actions.Enable();

        Q = actions.Player.QSkill;
        W = actions.Player.WSkill;
        E = actions.Player.ESkill;
        R = actions.Player.RSkill;
        RMC = actions.Player.RClick;
    }
}
