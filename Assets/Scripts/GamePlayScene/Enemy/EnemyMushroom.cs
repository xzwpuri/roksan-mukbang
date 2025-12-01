using System.Collections;
using UnityEngine;

public class EnemyMushroom : EnemyBase
{
    [Header("Mushroom Skill Prefabs")]
    [SerializeField] private GameObject mushroomWPrefab;
    [SerializeField] private GameObject mushroomEPrefab;

    [Header("Mushroom Skill Parameters")]
    [SerializeField] private float mushroomWDuration = 6f;
    [SerializeField] private float mushroomWRadius = 4.5f;
    [SerializeField] private float mushroomEHealDuration = 6f;
    [SerializeField] private float mushroomEHealInterval = 0.5f;
    [SerializeField] private float mushroomEHealPerTick = 3f;

    private bool usingSkill1;
    private bool usingSkill2;

    protected override void Start()
    {
        base.Start();
        Init(55f, 2.1f, 2, 6);
    }

    public override void Skill1()
    {
        if (usingSkill1) return;
        usingSkill1 = true;
        StartCoroutine(CastMushroomSkill1());
    }

    public override void Skill2()
    {
        if (usingSkill2) return;
        usingSkill2 = true;
        StartCoroutine(CastMushroomSkill2());
    }

    private IEnumerator CastMushroomSkill1()
    {
        Vector3 dir = Target != null ? (Target.position - transform.position).normalized : Vector3.right;

        if (mushroomWPrefab != null)
        {
            GameObject wSkill = Instantiate(mushroomWPrefab, transform.position + dir * 3f, Quaternion.identity);
            wSkill.transform.localScale = new Vector3(mushroomWRadius, mushroomWRadius, 1f);

            // ✅ 버섯 독구름 장판 히트박스 연결
            InitHitboxesOn(wSkill);

            yield return new WaitForSeconds(mushroomWDuration);

            if (wSkill != null) Destroy(wSkill);
        }

        usingSkill1 = false;
    }

    private IEnumerator CastMushroomSkill2()
    {
        if (mushroomEPrefab != null)
        {
            GameObject eSkill = Instantiate(mushroomEPrefab, transform.position, Quaternion.identity, transform);

            var hitbox = eSkill.GetComponent<EnemySkillHitbox>();
            if (hitbox != null)
                hitbox.Init(this);

            StartCoroutine(DestroyAfter(eSkill, mushroomEHealDuration));
        }

        float elapsed = 0f;
        float nextHealTime = mushroomEHealInterval;

        while (elapsed < mushroomEHealDuration)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= nextHealTime)
            {
                Hp = Mathf.Min(Hp + mushroomEHealPerTick, 100f);
                nextHealTime += mushroomEHealInterval;
            }

            yield return null;
        }

        usingSkill2 = false;
    }

    private IEnumerator DestroyAfter(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null) Destroy(target);
    }
    private void InitHitboxesOn(GameObject obj)
    {
        if (obj == null) return;
        var hitboxes = obj.GetComponentsInChildren<EnemySkillHitbox>(true);
        foreach (var hb in hitboxes)
            hb.Init(this);
    }
}
