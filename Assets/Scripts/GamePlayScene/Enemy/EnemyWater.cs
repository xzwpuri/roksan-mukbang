using System.Collections;
using UnityEngine;

public class EnemyWater : EnemyBase
{
    [Header("Water Skill Prefabs")]
    [SerializeField] private GameObject waterWPrefab;
    [SerializeField] private GameObject waterEPrefab;

    [Header("Water Skill Parameters")]
    [SerializeField] private float waterWSpeed = 10f;
    [SerializeField] private float waterWReach = 20f;
    [SerializeField] private float waterWRadius = 0.5f;
    [SerializeField] private float waterWInterval = 0.1f;
    [SerializeField] private int waterWCount = 3;
    [SerializeField] private float waterESpeed = 1f;
    [SerializeField] private float waterEStartScale = 0f;
    [SerializeField] private float waterEEndScale = 5f;

    private bool usingSkill1;
    private bool usingSkill2;

    protected override void Start()
    {
        base.Start();             // FSM 셋업
        Init(50f, 2.5f, 1, 7);    // EnemyWater 기본값 (stomach 7)
    }

    public override void Skill1()
    {
        if (usingSkill1) return;
        usingSkill1 = true;

        StartCoroutine(CastWaterSkill1());
    }

    public override void Skill2()
    {
        if (usingSkill2) return;
        usingSkill2 = true;

        StartCoroutine(CastWaterSkill2());
    }

    private IEnumerator CastWaterSkill1()
    {
        Vector3 dir = Target != null ? (Target.position - transform.position).normalized : Vector3.right;
        Vector3 origin = transform.position;

        for (int i = 0; i < waterWCount; i++)
        {
            StartCoroutine(WaterProjectile(waterWPrefab, origin, dir, waterWSpeed, waterWReach, waterWRadius));
            yield return new WaitForSeconds(waterWInterval);
        }

        usingSkill1 = false;
    }

    private IEnumerator CastWaterSkill2()
    {
        GameObject eSkill = null;

        if (waterEPrefab != null)
        {
            eSkill = Instantiate(waterEPrefab, transform.position, Quaternion.identity);
            eSkill.transform.localScale = new Vector3(waterEStartScale, waterEStartScale, 1f);
        }

        yield return WaterESequence(eSkill, waterEStartScale, waterEEndScale, waterESpeed);

        if (eSkill != null)
        {
            Destroy(eSkill);
        }

        usingSkill2 = false;
    }

    private IEnumerator WaterProjectile(
        GameObject projectilePrefab,
        Vector3 skillPos,
        Vector3 dir,
        float speed,
        float reach,
        float radius
    )
    {
        if (projectilePrefab == null) yield break;

        Vector3 dirNorm = dir.normalized;
        float angle = Mathf.Atan2(dirNorm.y, dirNorm.x) * Mathf.Rad2Deg;

        GameObject wSkill = Instantiate(
            projectilePrefab,
            skillPos,
            Quaternion.Euler(0, 0, angle)
        );
        wSkill.transform.localScale = new Vector3(radius, radius, 1f);

        float t = 0f;
        while (t < reach)
        {
            if (wSkill == null) break;

            float tt = t;
            t = Mathf.MoveTowards(t, reach, speed * Time.deltaTime);
            float move = t - tt;

            wSkill.transform.position += move * dirNorm;
            yield return null;
        }

        if (wSkill != null) Destroy(wSkill);
    }

    private IEnumerator WaterESequence(GameObject effect, float startScale, float endScale, float speed)
    {
        if (effect == null) yield break;

        float t = 0f;
        while (t < 1f)
        {
            t = Mathf.MoveTowards(t, 1f, Time.deltaTime * speed);

            float easing = Mathf.Sqrt(1 - Mathf.Pow(t - 1f, 2));
            float scale = Mathf.Lerp(startScale, endScale, easing);

            effect.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
    }
}
