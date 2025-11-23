using UnityEngine;

public class PlayerSkillHitbox : MonoBehaviour
{
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private bool destroyOnHit = true;

    private IUnit playerUnit;

    private void Awake()
    {
        // 단일 플레이어 가정
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerUnit = playerObj.GetComponent<IUnit>(); // Player : IUnit
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Enemy 태그 가진 애만 맞게
        if (!other.CompareTag("Enemy"))
            return;

        // ✅ 여기에서 EnemyBase 상속받는 애들 전부 잡힘
        IUnit enemyUnit = other.GetComponent<IUnit>();          // EnemyWater : EnemyBase : IUnit
        if (enemyUnit == null)
            enemyUnit = other.GetComponentInParent<IUnit>();    // 콜라이더가 자식에 붙어있을 경우

        if (enemyUnit == null || playerUnit == null)
            return;

        // 💥 GetDamage 안에 속성 계산 다 들어있다고 했으니까,
        // 여기서는 "데미지 + 공격자 속성"만 넘겨주면 끝
        enemyUnit.GetDamage(baseDamage, playerUnit.Element);

        if (destroyOnHit)
            Destroy(gameObject);
    }
}
