using System.Collections.Generic;
using UnityEngine;

public class EnemySkillHitbox : MonoBehaviour
{
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private bool destroyOnHit = true;
    [SerializeField] private float damageInterval = 0f; // 0이면 한 번만 데미지

    // ✅ 이 투사체를 쏜 Enemy (공격자)
    private IUnit ownerUnit;

    private Collider2D hitboxCollider;
    private ContactFilter2D filter;
    private Collider2D[] results = new Collider2D[10];

    // ✅ 이미 맞은 대상(플레이어) 목록
    private Dictionary<IUnit, float> damagedUnits = new Dictionary<IUnit, float>();

    private void Awake()
    {
        // 이 스킬 프리팹이 Enemy 오브젝트(또는 그 자식)에 붙어있다고 가정
        ownerUnit = GetComponentInParent<IUnit>(); // EnemyWater : EnemyBase : IUnit 등

        hitboxCollider = GetComponent<Collider2D>();

        filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
    }

    private void Update()
    {
        if (ownerUnit == null || hitboxCollider == null)
            return;

        int count = hitboxCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            var other = results[i];
            if (other == null) continue;

            // ✅ 적 투사체는 Player만 맞추게
            if (!other.CompareTag("Player"))
                continue;

            IUnit targetUnit = other.GetComponent<IUnit>();
            if (targetUnit == null)
                targetUnit = other.GetComponentInParent<IUnit>();

            if (targetUnit == null)
                continue;

            // ✅ 데미지 간격 체크
            if (damagedUnits.TryGetValue(targetUnit, out var lastHitTime))
            {
                if (damageInterval <= 0f || Time.time - lastHitTime < damageInterval)
                    continue;
            }

            // 💥 데미지 적용 (공격자: ownerUnit, 피격자: targetUnit)
            targetUnit.GetDamage(baseDamage, ownerUnit.Element);
            damagedUnits[targetUnit] = Time.time;

            // 추가 효과(예: 플레이어 속박) 있으면 호출
            var rootHitbox = GetComponent<IceCreamEnemyRootHitbox>();
            if (rootHitbox != null)
                rootHitbox.ApplyRootFromCollider(other);

            // 히트박스가 한 번만 맞고 사라지는 타입이면
            if (destroyOnHit)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}
