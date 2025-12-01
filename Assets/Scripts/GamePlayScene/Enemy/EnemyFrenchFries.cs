using System.Collections;
using UnityEngine;

public class EnemyFrenchFries : EnemyBase
{
    [Header("Fries Skill Prefabs")]
    [SerializeField] private GameObject friesWPrefab;
    [SerializeField] private GameObject friesWUpgradedPrefab;
    [SerializeField] private GameObject friesEPrefab;

    [Header("Fries Skill Parameters")]
    [SerializeField] private float friesWSpeed = 15f;
    [SerializeField] private float friesWReach = 20f;

    private bool friesUpgraded;
    private bool usingSkill1;
    private bool usingSkill2;

    protected override void Start()
    {
        base.Start();
        Init(60f, 2.5f, 2, 3);
    }

    public override void Skill1()
    {
        if (usingSkill1) return;
        usingSkill1 = true;
        StartCoroutine(CastFriesSkill1());
    }

    public override void Skill2()
    {
        if (usingSkill2) return;
        usingSkill2 = true;
        StartCoroutine(CastFriesSkill2());
    }

    private IEnumerator CastFriesSkill1()
    {
        Vector3 dir = Target != null ? (Target.position - transform.position).normalized : Vector3.right;
        Vector3 skillPos = transform.position + dir * 0.3f;

        GameObject prefabToUse = friesUpgraded && friesWUpgradedPrefab != null
            ? friesWUpgradedPrefab
            : friesWPrefab;

        friesUpgraded = false;

        if (prefabToUse != null)
            yield return LaunchFriesProjectile(prefabToUse, skillPos, dir, friesWSpeed, friesWReach);

        usingSkill1 = false;
    }

    private IEnumerator CastFriesSkill2()
    {
        friesUpgraded = true;

        if (friesEPrefab != null)
        {
            GameObject upgradeFx = Instantiate(friesEPrefab, transform.position, Quaternion.identity, transform);

            var hitbox = upgradeFx.GetComponent<EnemySkillHitbox>();
            if (hitbox != null)
                hitbox.Init(this);

            yield return new WaitForSeconds(0.5f);
            Destroy(upgradeFx);
        }

        usingSkill2 = false;
    }

    private IEnumerator LaunchFriesProjectile(GameObject prefab, Vector3 startPos, Vector3 dir, float speed, float reach)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        GameObject projectile = Instantiate(prefab, startPos, Quaternion.Euler(0, 0, angle));

        // ✅ 감자튀김 투사체 히트박스 연결
        InitHitboxesOn(projectile);

        float t = 0f;
        while (t < reach)
        {
            if (projectile == null) yield break;

            float prev = t;
            t = Mathf.MoveTowards(t, reach, speed * Time.deltaTime);
            projectile.transform.position += (t - prev) * dir;
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
