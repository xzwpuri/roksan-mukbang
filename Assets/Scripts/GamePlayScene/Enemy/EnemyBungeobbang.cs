using System.Collections;
using UnityEngine;

public class EnemyBungeobbang : EnemyBase
{
    [Header("Bungeobbang Skill Prefabs")]
    [SerializeField] private GameObject bungeobbangWPrefab;
    [SerializeField] private GameObject bungeobbangWUpgradedPrefab;
    [SerializeField] private GameObject bungeobbangEPrefab;

    [Header("Bungeobbang Skill Parameters")]
    [SerializeField] private float bungeobbangWSpeed = 10f;
    [SerializeField] private float bungeobbangWReach = 20f;
    [SerializeField] private float bungeobbangWRadius = 0.7f;

    private bool custardMode;
    private bool usingSkill1;
    private bool usingSkill2;

    protected override void Start()
    {
        base.Start();
        Init(60f, 2.0f, 3, 1);
    }

    public override void Skill1()
    {
        if (usingSkill1) return;
        usingSkill1 = true;

        StartCoroutine(CastBungeobbangSkill1());
    }

    public override void Skill2()
    {
        if (usingSkill2) return;
        usingSkill2 = true;

        StartCoroutine(CastBungeobbangSkill2());
    }

    private IEnumerator CastBungeobbangSkill1()
    {
        Vector3 dir = Target != null ? (Target.position - transform.position).normalized : Vector3.right;
        Vector3 skillPos = transform.position + dir * 0.3f;

        GameObject prefabToUse = custardMode && bungeobbangWUpgradedPrefab != null
            ? bungeobbangWUpgradedPrefab
            : bungeobbangWPrefab;

        if (prefabToUse == null)
        {
            usingSkill1 = false;
            yield break;
        }

        if (custardMode)
        {
            for (int i = 0; i < 8; i++)
            {
                float angle = (360f / 8) * i;
                Vector3 shotDir = Quaternion.Euler(0f, 0f, angle) * dir;
                StartCoroutine(BungeobbangProjectile(prefabToUse, transform.position, shotDir, bungeobbangWSpeed, bungeobbangWReach, bungeobbangWRadius));
            }
        }
        else
        {
            StartCoroutine(BungeobbangProjectile(prefabToUse, skillPos, Quaternion.Euler(0, 0, -45f) * dir, bungeobbangWSpeed, bungeobbangWReach, bungeobbangWRadius));
            StartCoroutine(BungeobbangProjectile(prefabToUse, skillPos, dir, bungeobbangWSpeed, bungeobbangWReach, bungeobbangWRadius));
            StartCoroutine(BungeobbangProjectile(prefabToUse, skillPos, Quaternion.Euler(0, 0, 45f) * dir, bungeobbangWSpeed, bungeobbangWReach, bungeobbangWRadius));
        }

        yield return new WaitForSeconds(0.1f);
        usingSkill1 = false;
    }

    private IEnumerator CastBungeobbangSkill2()
    {
        custardMode = !custardMode;

        if (bungeobbangEPrefab != null)
        {
            GameObject toggleFx = Instantiate(bungeobbangEPrefab, transform.position, Quaternion.identity);
            // 필요하다면 여기도 EnemySkillHitbox가 있을 수 있음
            var hitbox = toggleFx.GetComponent<EnemySkillHitbox>();
            if (hitbox != null)
                hitbox.Init(this);

            yield return new WaitForSeconds(0.5f);
            Destroy(toggleFx);
        }

        usingSkill2 = false;
    }

    private IEnumerator BungeobbangProjectile(GameObject prefab, Vector3 startPos, Vector3 dir, float speed, float reach, float radius)
    {
        Vector3 dirNorm = dir.normalized;
        float angle = Mathf.Atan2(dirNorm.y, dirNorm.x) * Mathf.Rad2Deg;

        GameObject projectile = Instantiate(prefab, startPos, Quaternion.Euler(0, 0, angle));
        projectile.transform.localScale = new Vector3(radius, radius, 1f);

        // ✅ 투사체에 EnemySkillHitbox 연결
        InitHitboxesOn(projectile);

        float t = 0f;
        while (t < reach)
        {
            if (projectile == null) yield break;

            float prev = t;
            t = Mathf.MoveTowards(t, reach, speed * Time.deltaTime);
            projectile.transform.position += (t - prev) * dirNorm;
            yield return null;
        }

        if (projectile != null) Destroy(projectile);
    }
    
    private void InitHitboxesOn(GameObject obj)
    {
        if (obj == null) return;
        var hitboxes = obj.GetComponentsInChildren<EnemySkillHitbox>(true);
        foreach (var hb in hitboxes)
            hb.Init(this);
    }

}
