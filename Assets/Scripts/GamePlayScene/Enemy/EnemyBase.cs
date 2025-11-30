using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IUnit
{
    [Header("공통 스탯 (IUnit)")]
    [SerializeField] protected float hp;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int element;
    [SerializeField] protected int stomach;

    [Header("AI 공통 옵션")]
    [SerializeField] protected float chaseRange = 10f;   // 추격 시작 거리
    [SerializeField] protected float stopDistance = 2f;  // 이동 멈추는 거리
    [SerializeField] protected float attackRange = 4f;   // ✅ 스킬을 쓰기 시작하는 거리

    [Header("스킬 쿨타임")]
    [SerializeField] protected float skill1Cooldown = 7f;
    [SerializeField] protected float skill2Cooldown = 8f;

    public Transform Target { get; set; } // 보통 Player Transform

    // IUnit 구현
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

    public int Stomach
    {
        get => stomach;
        set => stomach = value;
    }

    // AI 관련 프로퍼티 (State들이 공통으로 사용)
    public float ChaseRange => chaseRange;
    public float StopDistance => stopDistance;
    public float AttackRange => attackRange;          // ✅ 공격 시작/유지 거리
    public float Skill1Cooldown => skill1Cooldown;
    public float Skill2Cooldown => skill2Cooldown;

    public Dictionary<StateType, State<EnemyBase>> States;
    public StateMachine<EnemyBase> StateMachine;

    [HideInInspector] public Rigidbody2D Rigidbody2D;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public SpriteRenderer SpriteRenderer;

    protected virtual void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        // 타겟(플레이어) 자동 세팅
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            Target = player.transform;

        States = new Dictionary<StateType, State<EnemyBase>>
        {
            { StateType.Idle,   new EnemyIdleState() },
            { StateType.Move,   new EnemyMoveState() },
            { StateType.Attack, new EnemyAttackState() },
            { StateType.Dead,   new EnemyDeadState() },
        };

        StateMachine = new StateMachine<EnemyBase>();
        StateMachine.Setup(this, States[StateType.Idle]);
    }

    public void Init(float hp, float moveSpeed, int element, int stomach)
    {
        Hp = hp;
        MoveSpeed = moveSpeed;
        Element = element;
        Stomach = stomach;
    }

    protected virtual void Update()
    {
        StateMachine.Update();
    }

    public virtual void GetDamage(float damage, int attackerElement)
    {
        // 공격자 속성과 내(Element) 속성으로 상성 적용
        float finalDamage = ElementCalculate.ApplyElementModifier(damage, attackerElement, Element);

        Hp -= finalDamage;

        // 🔴 먼저 죽었는지부터 체크
        if (Hp <= 0f)
        {
            Hp = 0f;

            // 죽는 상태로 전환 (여기서 회색 처리)
            StateMachine.ChangeState(States[StateType.Dead]);
            return; // 👈 여기서 끝내버리기
        }

        // 살아있을 때만 피격 깜빡임
        DamageFlash flash = GetComponent<DamageFlash>();
        if (flash != null)
            flash.PlayFlash();
    }


    // ★ 몬스터별 공격 구현 포인트
    public abstract void Skill1();
    public abstract void Skill2();
}
