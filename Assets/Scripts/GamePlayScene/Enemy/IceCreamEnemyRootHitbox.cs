using UnityEngine;

public class IceCreamEnemyRootHitbox : MonoBehaviour
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

            // ✅ Enemy 버전은 Player만 속박
            if (!other.CompareTag("Player"))
                continue;

            ApplyRootFromCollider(other);
        }
    }

    public void ApplyRootFromCollider(Collider2D other)
    {
        if (hasHit)
            return;

        Player player = other.GetComponent<Player>();
        if (player == null)
            player = other.GetComponentInParent<Player>();

        if (player == null)
            return;

        StartCoroutine(RootPlayer(player));
        hasHit = true;
    }

    private System.Collections.IEnumerator RootPlayer(Player player)
    {
        if (player == null)
            yield break;

        float originalSpeed = player.MoveSpeed;
        player.MoveSpeed = 0f;

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero; // velocity 써도 됨

        Debug.Log($"[IceCreamEnemyRootHitbox] {player.name} 속박! {rootDuration}초");

        yield return new WaitForSeconds(rootDuration);

        if (player != null)
        {
            player.MoveSpeed = originalSpeed;
            Debug.Log($"[IceCreamEnemyRootHitbox] {player.name} 속박 해제");
        }

        if (destroyOnHit)
            Destroy(gameObject);   // ✅ 속박 끝나고 투사체/히트박스 제거
    }
}
