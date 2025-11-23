using UnityEngine;

public class SkillHitbox : MonoBehaviour
{
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private bool destroyOnHit = true;

    // 이 스킬을 사용한 쪽의 속성(플레이어 Element)
    [HideInInspector] public int attackerElement;

    public void Init(int attackerElement, float damage = -1f)
    {
        this.attackerElement = attackerElement;
        if (damage >= 0f)
            this.baseDamage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        IUnit unit = other.GetComponent<IUnit>();
        if (unit == null)
            unit = other.GetComponentInParent<IUnit>();

        if (unit == null)
            return;

        // ✅ 상성 계산은 IUnit 쪽 GetDamage에서!
        unit.GetDamage(baseDamage, attackerElement);

        if (destroyOnHit)
            Destroy(gameObject);
    }
}
