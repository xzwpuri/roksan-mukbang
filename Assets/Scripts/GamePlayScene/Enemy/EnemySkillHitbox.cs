using System.Collections.Generic;
using UnityEngine;

public class EnemySkillHitbox : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private bool destroyOnHit = true;
    [SerializeField] private float damageInterval = 0f; // 0이면 한 번만 데미지

    [Header("Debug")]
    [SerializeField] private bool enableDebugLog = true;

    private IUnit ownerUnit;                 // ✅ 공격자
    private Collider2D hitboxCollider;
    private ContactFilter2D filter;
    private readonly Collider2D[] results = new Collider2D[10];
    private readonly Dictionary<IUnit, float> damagedUnits = new Dictionary<IUnit, float>();

    private bool loggedNullOwnerOnce = false; // 로그 도배 방지용

    // ===========================
    //  초기화
    // ===========================
    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();
        if (hitboxCollider == null)
        {
            Debug.LogError("[EnemySkillHitbox] ⚠ Collider2D가 없습니다. 콜라이더(Trigger)를 반드시 붙이세요.", this);
        }
        else
        {
            DebugLog($"Collider2D 할당 완료: {hitboxCollider.GetType().Name}, isTrigger = {hitboxCollider.isTrigger}");
        }

        filter = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true
        };

        LayerMask mask = Physics2D.GetLayerCollisionMask(gameObject.layer);
        filter.SetLayerMask(mask);

        DebugLog($"ContactFilter 초기화: gameObject.layer={LayerMask.LayerToName(gameObject.layer)}, mask value={mask.value}");
    }

    /// <summary>
    /// ❗ Enemy가 Instantiate 직후에 꼭 호출해야 하는 함수
    /// </summary>
    public void Init(IUnit owner)
    {
        ownerUnit = owner;
        loggedNullOwnerOnce = false; // 다시 초기화

        if (ownerUnit == null)
        {
            Debug.LogError("[EnemySkillHitbox] Init(owner) 호출했는데 owner == null 입니다.", this);
        }
        else
        {
            DebugLog($"Init()으로 ownerUnit 설정: {ownerUnit} (Element={ownerUnit.Element}, Hp={ownerUnit.Hp})");
        }
    }

    // ===========================
    //  매 프레임 충돌 검사
    // ===========================
    private void Update()
    {
        if (hitboxCollider == null)
        {
            Debug.LogError("[EnemySkillHitbox] ⚠ hitboxCollider == null 입니다. Awake에서 콜라이더를 못 찾았어요.", this);
            return;
        }

        if (ownerUnit == null)
        {
            // 로그 한 번만 찍고 조용히 있게
            if (!loggedNullOwnerOnce)
            {
                loggedNullOwnerOnce = true;
                DebugLog("ownerUnit == null 이라서 데미지를 적용하지 않습니다. " +
                         "⚠ 이 투사체를 생성한 Enemy 코드에서 hitbox.Init(this)를 호출했는지 확인하세요.");
            }
            return;
        }

        int count = hitboxCollider.Overlap(filter, results);
        if (count == 0) return;

        DebugLog($"OverlapCollider 결과 count = {count}");

        for (int i = 0; i < count; i++)
        {
            var other = results[i];
            if (other == null)
            {
                DebugLog($"results[{i}] 가 null 입니다.");
                continue;
            }

            DebugLog($"results[{i}] = {other.name}, tag={other.tag}, layer={LayerMask.LayerToName(other.gameObject.layer)}");

            // ----- Player 판정 -----
            bool isPlayer = other.CompareTag("Player");
            if (!isPlayer)
            {
                var playerComponent = other.GetComponentInParent<Player>();
                if (playerComponent != null)
                {
                    isPlayer = true;
                    DebugLog($"Collider {other.name} 의 부모에서 Player 컴포넌트 발견: {playerComponent.name}");
                }
            }

            if (!isPlayer)
            {
                DebugLog($"Collider {other.name} 는 Player가 아니라서 무시합니다.");
                continue;
            }

            // ----- IUnit 가져오기 -----
            IUnit targetUnit = other.GetComponent<IUnit>();
            if (targetUnit == null)
                targetUnit = other.GetComponentInParent<IUnit>();

            if (targetUnit == null)
            {
                DebugLog($"Collider {other.name} 는 Player로 판정되지만 IUnit 컴포넌트를 찾지 못했습니다.");
                continue;
            }

            // ----- 데미지 간격 체크 -----
            if (damagedUnits.TryGetValue(targetUnit, out float lastHitTime))
            {
                float delta = Time.time - lastHitTime;

                if (damageInterval <= 0f)
                {
                    DebugLog($"대상 {targetUnit} 은 이미 한 번 맞았고, damageInterval <= 0 이므로 추가 데미지는 주지 않습니다.");
                    continue;
                }

                if (delta < damageInterval)
                {
                    DebugLog($"대상 {targetUnit} 은 {delta:F2}초 전에 맞았고, damageInterval={damageInterval:F2} 보다 짧아서 스킵합니다.");
                    continue;
                }
            }

            // ----- 데미지 적용 -----
            DebugLog($"데미지 적용! 대상={targetUnit}, baseDamage={baseDamage}, 공격자 Element={ownerUnit.Element}");
            targetUnit.GetDamage(baseDamage, ownerUnit.Element);
            damagedUnits[targetUnit] = Time.time;

            // ----- 추가 효과 (속박 등) -----
            var rootHitbox = GetComponent<IceCreamEnemyRootHitbox>();
            if (rootHitbox != null)
            {
                DebugLog("IceCreamEnemyRootHitbox 발견 → ApplyRootFromCollider(other) 호출");
                rootHitbox.ApplyRootFromCollider(other);
            }

            if (destroyOnHit)
            {
                DebugLog("destroyOnHit = true → 투사체 파괴");
                Destroy(gameObject);
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DebugLog($"OnTriggerEnter2D: {other.name}, tag={other.tag}, layer={LayerMask.LayerToName(other.gameObject.layer)}");
    }

    private void DebugLog(string msg)
    {
        if (!enableDebugLog) return;
        Debug.Log($"[EnemySkillHitbox] {msg}", this);
    }
}
