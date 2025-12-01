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

        player.ApplyRoot(rootDuration);
        hasHit = true;

        if (destroyOnHit)
            Destroy(gameObject);
    }
}
