using UnityEngine;

public class IceCreamRootHitbox : MonoBehaviour
{
    [SerializeField] private float rootDuration = 1f;
    [SerializeField] private bool destroyOnHit = true;

    private Collider2D hitboxCollider;
    private ContactFilter2D filter;
    private Collider2D[] results = new Collider2D[10];
    private bool hasHit = false;

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();

        filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
    }

    private void Update()
    {
        if (hasHit || hitboxCollider == null)
            return;

        int count = hitboxCollider.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            var other = results[i];
            if (other == null) continue;

            if (!other.CompareTag("Enemy"))
                continue;

            // Enemy 콜라이더가 자식에 있을 수 있으니까 부모까지
            EnemyBase enemyBase = other.GetComponent<EnemyBase>();
            if (enemyBase == null)
                enemyBase = other.GetComponentInParent<EnemyBase>();

            if (enemyBase == null)
                continue;

            // 속박 적용
            StartCoroutine(RootEnemy(enemyBase));
            hasHit = true;

            if (destroyOnHit)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private System.Collections.IEnumerator RootEnemy(EnemyBase enemy)
    {
        float originalSpeed = enemy.MoveSpeed;
        enemy.MoveSpeed = 0f;

        if (enemy.Rigidbody2D != null)
            enemy.Rigidbody2D.linearVelocity = Vector2.zero;

        Debug.Log($"[IceCreamRootHitbox] {enemy.name} 속박! {rootDuration}초");

        yield return new WaitForSeconds(rootDuration);

        enemy.MoveSpeed = originalSpeed;
        Debug.Log($"[IceCreamRootHitbox] {enemy.name} 속박 해제");
    }
}