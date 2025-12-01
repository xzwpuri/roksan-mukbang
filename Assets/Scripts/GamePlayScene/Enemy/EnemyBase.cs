using System.Collections;
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
    [SerializeField] protected float chaseRange = 1000f;  // 추격 시작/유지 거리
    [SerializeField] protected float stopDistance = 2f;   // ✅ 유지하고 싶은 거리(링 중심)
    
    [Header("스킬 사거리")]
    [SerializeField] protected float skill1Range = 4f;    // 1번 스킬 사거리
    [SerializeField] protected float skill2Range = 6f;    // 2번 스킬 사거리

    [Header("스킬 쿨타임")]
    [SerializeField] protected float skill1Cooldown = 7f;
    [SerializeField] protected float skill2Cooldown = 10f;

    [Header("버프 FX 위치")]
    [SerializeField] protected Vector3 buffEffectOffset = new Vector3(0f, 1.2f, 0f);

    public Transform Target { get; set; }

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

    // AI 관련 프로퍼티
    public float ChaseRange     => chaseRange;
    public float StopDistance   => stopDistance;              // ✅ 원하는 거리
    public float Skill1Range    => skill1Range;
    public float Skill2Range    => skill2Range;
    public float Skill1Cooldown => skill1Cooldown;
    public float Skill2Cooldown => skill2Cooldown;

    // 기존 AttackRange는 "두 스킬 중 더 먼 사거리"로 정의해둘 수도 있음 (안 써도 됨)
    public float AttackRange => Mathf.Max(skill1Range, skill2Range);

    // Crowd control 상태 관리
    private bool isRooted = false;
    [Header("CC 옵션")]
    [SerializeField] private float rootImmunityDuration = 0.5f;

    private float rootEndTime = 0f;
    private float rootImmunityEndTime = 0f;
    private float rootedOriginalSpeed = 0f;

    public Dictionary<StateType, State<EnemyBase>> States;
    public StateMachine<EnemyBase> StateMachine;

    [HideInInspector] public Rigidbody2D Rigidbody2D;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public SpriteRenderer SpriteRenderer;

    protected virtual void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator     = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
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
        Hp       = hp;
        MoveSpeed = moveSpeed;
        Element  = element;
        Stomach  = stomach;
    }

    /// ===================================================
    /// 속박 적용. 이미 속박 중이거나 직후 무적 시간에는 무시한다.
    /// ===================================================
    /// <param name="duration">속박 지속 시간</param>
    public void ApplyRoot(float duration)
    {
        if (duration <= 0f)
            return;

        // 해제 직후 무적 구간에는 속박이 적용되지 않는다.
        if (Time.time < rootImmunityEndTime)
            return;

        // 이미 속박 중이면 추가 적용을 막아 무한 속박을 방지한다.
        if (isRooted)
        {
            return;
        }

        rootedOriginalSpeed = MoveSpeed;
        rootEndTime = Time.time + duration;
        StartCoroutine(RootRoutine());
    }

    private IEnumerator RootRoutine()
    {
        isRooted = true;
        MoveSpeed = 0f;

        if (Rigidbody2D != null)
            Rigidbody2D.linearVelocity = Vector2.zero;

        while (Time.time < rootEndTime)
        {
            yield return null;
        }

        MoveSpeed = rootedOriginalSpeed;
        isRooted = false;
        rootImmunityEndTime = Time.time + rootImmunityDuration;
    }
    protected virtual void Update()
    {
        StateMachine.Update();
    }

    public virtual void GetDamage(float damage, int attackerElement)
    {
        float finalDamage = ElementCalculate.ApplyElementModifier(damage, attackerElement, Element);

        Hp -= finalDamage;

        if (Hp <= 0f)
        {
            Hp = 0f;
            StateMachine.ChangeState(States[StateType.Dead]);
            return;
        }

        DamageFlash flash = GetComponent<DamageFlash>();
        if (flash != null)
            flash.PlayFlash();
    }

    public abstract void Skill1();
    public abstract void Skill2();

    protected void SpawnBuffEffect(GameObject effectPrefab)
    {
        if (effectPrefab == null)
            return;

        Instantiate(effectPrefab, transform.position + buffEffectOffset, Quaternion.identity, transform);
    }
}
