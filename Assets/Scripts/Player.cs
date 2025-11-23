using System.Collections.Generic;
using UnityEngine;
public enum StateType
{
    Idle,
    Move
}

public class Player : MonoBehaviour, IUnit
{
    // === IUnit 백킹 필드 ===
    [SerializeField] private float hp;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int element; // 1 -> 2 -> 3 -> 1 (f(x) = (x % 3) + 1)
    [SerializeField] public int stomach;
    private int previousStomach;

    // === IUnit 프로퍼티 구현 ===
    public float Hp
    {
        get => hp;
        set => hp = value;
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public int Element
    {
        get => element;
        set => element = value;
    }

    // === 기존 필드 ===
    public Dictionary<StateType, State<Player>> States;
    public StateMachine<Player> StateMachine;

    [HideInInspector] public Rigidbody2D Rigidbody2D;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public SpriteRenderer SpriteRenderer;
    [HideInInspector] public Vector2 MoveTarget;
    [HideInInspector] public float mspeed;

    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        // Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // 초기 값 세팅 (원하면 인스펙터에서 직접 넣고 Init 생략해도 됨)
        Init(100f, 5f, 0);
        previousStomach = stomach;

        // 기존에 this.movespeed 참조하던 부분을 프로퍼티로 변경
        mspeed = MoveSpeed;

        States = new Dictionary<StateType, State<Player>>();
        StateMachine = new StateMachine<Player>();

        States.Add(StateType.Idle, new PlayerIdleState());
        States.Add(StateType.Move, new PlayerMoveState());

        StateMachine.Setup(this, States[StateType.Idle]);
    }

    private void Update()
    {
        StateMachine.Update();
        CheckFlipX();
        if (stomach !=  previousStomach) Setstomach(stomach);
    }

    private void CheckFlipX()
    {
        // 프로젝트에 linearVelocityX 확장 프로퍼티를 쓰는 것으로 보임 (그대로 유지)
        if (Rigidbody2D.linearVelocityX > 0.01f) SpriteRenderer.flipX = false;
        else if (Rigidbody2D.linearVelocityX < -0.01f) SpriteRenderer.flipX = true;
    }

    // === IUnit 메서드 구현 ===
    public void Init(float hp, float moveSpeed, int element)
    {
        Hp = hp;
        MoveSpeed = moveSpeed;
        Element = element;
        Setstomach(element);
    }

    public void GetDamage(float damage)
    {
        Hp -= damage;
        if (Hp <= 0f)
        {
            Debug.Log("플레이어 사망!");
            // Destroy(gameObject);
        }
    }

    // 임시로 넣어둔거
    public void Setstomach(int newstomach)
    {
        stomach = newstomach; // 1/2/3...
        GetComponent<SkillCaster>()?.RefreshLoadout();
    }

}
