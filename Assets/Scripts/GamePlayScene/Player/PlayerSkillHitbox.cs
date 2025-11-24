using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillHitbox : MonoBehaviour
{
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private bool destroyOnHit = true;

    private IUnit playerUnit;
    private Collider2D hitboxCollider;
    private ContactFilter2D filter;
    private Collider2D[] results = new Collider2D[10];

    // ✅ 이미 맞은 IUnit 목록
    private HashSet<IUnit> damagedUnits = new HashSet<IUnit>();

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

            // ✅ 이미 맞은 적이면 스킵
            if (damagedUnits.Contains(enemyUnit))
                continue;

            // 💥 첫 타만 들어감
            enemyUnit.GetDamage(baseDamage, playerUnit.Element);
            damagedUnits.Add(enemyUnit);

            // 히트박스가 한 번만 맞고 사라지는 타입이면
            if (destroyOnHit)
            {
                Destroy(gameObject);
                return; // 이 프레임에서 더 이상 처리 안 함
            }
        }
    }
}