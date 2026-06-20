using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillHitbox : MonoBehaviour
{
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private bool destroyOnHit = true;
    [SerializeField] private float damageInterval = 0f; // 0이면 한 번만 데미지

    private IUnit playerUnit;
    private Collider2D hitboxCollider;
    private ContactFilter2D filter;
    private Collider2D[] results = new Collider2D[10];

    // ✅ 이미 맞은 IUnit 목록
    private Dictionary<IUnit, float> damagedUnits = new Dictionary<IUnit, float>();

    private void Awake()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerUnit = playerObj.GetComponent<IUnit>();

        hitboxCollider = GetComponent<Collider2D>();

        filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
    }

    private void Update()
    {
        if (playerUnit == null || hitboxCollider == null)
            return;

        int count = hitboxCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            var other = results[i];
            if (other == null) continue;

            if (!other.CompareTag("Enemy"))
                continue;

            // Enemy 콜라이더가 자식에 있을 수 있으니까 부모까지
            IUnit enemyUnit = other.GetComponent<IUnit>();
            if (enemyUnit == null)
                enemyUnit = other.GetComponentInParent<IUnit>();

            if (enemyUnit == null)
                continue;

            // ✅ 데미지 간격 체크
            if (damagedUnits.TryGetValue(enemyUnit, out var lastHitTime))
            {
                if (damageInterval <= 0f || Time.time - lastHitTime < damageInterval)
                    continue;
            }

            // 💥 데미지 적용
            enemyUnit.GetDamage(baseDamage, playerUnit.Element);
            damagedUnits[enemyUnit] = Time.time;

            // 추가 효과가 있으면 호출 (예: 아이스크림 속박)
            var rootHitbox = GetComponent<IceCreamRootHitbox>();
            if (rootHitbox != null)
                rootHitbox.ApplyRootFromCollider(other);

            // 히트박스가 한 번만 맞고 사라지는 타입이면
            if (destroyOnHit)
            {
                Destroy(gameObject);
                return; // 이 프레임에서 더 이상 처리 안 함
            }
        }
    }
}