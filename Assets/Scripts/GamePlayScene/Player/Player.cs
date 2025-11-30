using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    Idle,
    Move,
    Dead,
    Attack
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

    public int Stomach
    {
        get => stomach;
        set => stomach = value;
    }

    // === 기존 필드 ===
    public Dictionary<StateType, State<Player>> States;
    public StateMachine<Player> StateMachine;

    [HideInInspector] public Rigidbody2D Rigidbody2D;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public SpriteRenderer SpriteRenderer;
    [HideInInspector] public Vector2 MoveTarget;
    [HideInInspector] public float mspeed;

    // ===============================
    //  Default Skill (Jab / Swing)
    // ===============================
    [Header("Default Skill - Swing")]
    public float swingSpeed = 180f;
    public float swingAngle1 = 60f;
    public float swingAngle2 = -60f;
    public float swingReach = 1.2f;
    public float swingWidth = 0.3f;
    public GameObject swingPivotPrefab;

    [Header("Default Skill - Jab")]
    public float jabSpeed = 5f;
    public float jabReach = 1.7f;
    public float jabWidth = 0.5f;
    public GameObject jabPivotPrefab;

    // ===============================
    //  붕어빵 스킬 프리팹 & 파라미터
    // ===============================
    [Header("Bungeobbang Skill Prefabs")]
    public GameObject bungeobbangWPrefab;          // 일반 팥 붕어빵 (Redbeans)
    public GameObject bungeobbangWUpgradedPrefab;  // 크림 붕어빵 (Custardcream)
    public GameObject bungeobbangEPrefab;

    [Header("Bungeobbang Skill Parameters")]
    public float bungeobbangWSpeed = 10f;
    public float bungeobbangWReach = 20f;
    public float bungeobbangWRadius = 0.7f;
    public float bungeobbangCustardHpCost = 5f; // 한 번 쓸 때 소모 HP


    // ===============================
    //  콜라 스킬 프리팹 & 파라미터
    // ===============================
    [Header("Cola Skill Prefabs")]
    public GameObject colaWPrefab;
    public GameObject colaEPrefab;

    [Header("Cola Skill Parameters")]
    public float colaWDuration = 3f;
    public float colaWSpeedMultiplier = 1.5f;
    public float colaEHpCost = 5f;
    public float colaESpeed = 3f;
    public float colaEStartScale = 0f;
    public float colaEEndScale = 8f;

    // ===============================
    //  감자튀김 스킬 프리팹 & 파라미터
    // ===============================
    [Header("Fries Skill Prefabs")]
    public GameObject friesWPrefab;           // 일반 감자튀김 (Potato)
    public GameObject friesWUpgradedPrefab;   // 강화 감자튀김 (Tomato)
    public GameObject friesEPrefab;

    [Header("Fries Skill Parameters")]
    public float friesWSpeed = 15f;
    public float friesWReach = 20f;

    // ===============================
    //  아이스크림 스킬 프리팹 & 파라미터
    // ===============================
    [Header("IceCream Skill Prefabs")]
    public GameObject iceCreamWPrefab;
    public GameObject iceCreamEPrefab;

    [Header("IceCream Skill Parameters")]
    public float iceCreamWSpeed = 12f;
    public float iceCreamWReach = 20f;
    public float iceCreamWRadius = 0.6f;
    public float iceCreamESlowDuration = 4f;
    public float iceCreamESlowMultiplier = 0.5f;
    public float iceCreamEHealAmount = 30f;

    // ===============================
    //  고기 스킬 프리팹 & 파라미터
    // ===============================
    [Header("Meat Skill Prefabs")]
    public GameObject meatWPrefab;
    public GameObject meatEPrefab;

    [Header("Meat Skill Parameters")]
    public float meatWSpeed = 720f;
    public float meatWAngle1 = 70f;
    public float meatWAngle2 = -70f;
    public float meatWWidth = 2f;
    public float meatWHeight = 0.5f;
    public float meatESpeed = 10f;
    public float meatEDistance = 3f;

    // ===============================
    //  버섯 스킬 프리팹 & 파라미터
    // ===============================
    [Header("Mushroom Skill Prefabs")]
    public GameObject mushroomWPrefab;
    public GameObject mushroomEPrefab;

    [Header("Mushroom Skill Parameters")]
    public float mushroomWDuration = 6f;
    public float mushroomWRadius = 4.5f;
    public float mushroomEHealDuration = 6f;
    public float mushroomEHealInterval = 0.5f;
    public float mushroomEHealPerTick = 3f;

    // ===============================
    //  물 스킬 프리팹 & 파라미터
    // ===============================
    [Header("Water Skill Prefabs")]
    public GameObject waterWPrefab;
    public GameObject waterEPrefab;

    [Header("Water Skill Parameters")]
    public float waterWSpeed = 10f;
    public float waterWReach = 20f;
    public float waterWRadius = 0.5f;
    public float waterWInterval = 0.1f; // 간격
    public int waterWCount = 3; // 발사 개수
    public float waterESpeed = 1f;
    public float waterEStartScale = 0f;
    public float waterEEndScale = 5f;

    // 코루틴 중복 방지 플래그
    [HideInInspector] public bool isE = false;
    [HideInInspector] public bool isW = false;

    // ===============================
    //  스킬 상태 플래그
    // ===============================
    [Header("Skill States")]
    [HideInInspector] public bool isCustardCream = false;  // 붕어빵 E 스킬 활성화 여부
    [HideInInspector] public bool isFriesUpgraded = false; // 감자튀김 강화 여부

    // 버프/디버프 관리용 코루틴
    private Coroutine colaSpeedBuffCoroutine;
    private Coroutine iceCreamSlowCoroutine;
    private Coroutine mushroomHealCoroutine;

    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // 초기 값 세팅
        Init(100f, 5f, 0, 0);
        previousStomach = stomach;

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

        if (stomach != previousStomach)
        {
            Setstomach(stomach);
            previousStomach = stomach;
        }
    }

    private void CheckFlipX()
    {
        if (Rigidbody2D.linearVelocityX > 0.01f) SpriteRenderer.flipX = false;
        else if (Rigidbody2D.linearVelocityX < -0.01f) SpriteRenderer.flipX = true;
    }

    // === IUnit 메서드 구현 ===
    public void Init(float hp, float moveSpeed, int element, int stomach)
    {
        Hp = hp;
        MoveSpeed = moveSpeed;
        Element = element;
        Stomach = stomach;
        Setstomach(stomach);
    }

    public void GetDamage(float damage, int attackerElement)
    {
        // 공격자 속성과 내(Element) 속성으로 상성 적용
        float finalDamage = ElementCalculate.ApplyElementModifier(damage, attackerElement, Element);

        Hp -= finalDamage;

        if (Hp <= 0f)
        {
            Debug.Log("플레이어 사망!");
            // TODO: 죽는 상태로 State 변경 등
            // StateMachine.ChangeState(States[StateType.Dead]);
        }
    }

    public void Setstomach(int newstomach)
    {
        stomach = newstomach;
        GetComponent<SkillCaster>()?.RefreshLoadout();
    }

    // ===============================
    //  버프/디버프 관리 메서드
    // ===============================

    public void StartColaSpeedBuff(IEnumerator coroutine)
    {
        if (colaSpeedBuffCoroutine != null)
            StopCoroutine(colaSpeedBuffCoroutine);
        colaSpeedBuffCoroutine = StartCoroutine(coroutine);
    }

    public void StartIceCreamSlow(IEnumerator coroutine)
    {
        if (iceCreamSlowCoroutine != null)
            StopCoroutine(iceCreamSlowCoroutine);
        iceCreamSlowCoroutine = StartCoroutine(coroutine);
    }

    public void StartMushroomHeal(IEnumerator coroutine)
    {
        if (mushroomHealCoroutine != null)
            StopCoroutine(mushroomHealCoroutine);
        mushroomHealCoroutine = StartCoroutine(coroutine);
    }
}