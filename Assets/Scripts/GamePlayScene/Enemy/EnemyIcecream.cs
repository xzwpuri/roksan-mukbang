using System.Collections;
using UnityEngine;

public class EnemyIcecream : EnemyBase
{
    [Header("IceCream Skill Prefabs")]
    [SerializeField] private GameObject iceCreamWPrefab;
    [SerializeField] private GameObject iceCreamEPrefab;

    [Header("IceCream Skill Parameters")]
    [SerializeField] private float iceCreamWSpeed = 12f;
    [SerializeField] private float iceCreamWReach = 20f;
    [SerializeField] private float iceCreamWRadius = 0.6f;
    [SerializeField] private float iceCreamESlowDuration = 4f;
    [SerializeField] private float iceCreamESlowMultiplier = 0.5f;
    [SerializeField] private float iceCreamEHealAmount = 20f;

    private bool usingSkill1;
    private bool usingSkill2;

    protected override void Start()
    {
        base.Start();
        Init(52f, 2.3f, 1, 4);
    }

    public override void Skill1()
    {
        if (usingSkill1) return;
        usingSkill1 = true;
        StartCoroutine(CastIceCreamSkill1());
    }

    public override void Skill2()
    {
        if (usingSkill2) return;
        usingSkill2 = true;
        StartCoroutine(CastIceCreamSkill2());
    }

    private IEnumerator CastIceCreamSkill1()
    {
        Vector3 dir = Target != null ? (Target.position - transform.position).normalized : Vector3.right;
        Vector3 skillPos = transform.position + dir * 0.3f;

        if (iceCreamWPrefab != null)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            GameObject wSkill = Instantiate(iceCreamWPrefab, skillPos, Quaternion.Euler(0, 0, angle));
            wSkill.transform.localScale = new Vector3(iceCreamWRadius, iceCreamWRadius, 1f);

            // ✅ 아이스크림 투사체도 히트박스 연결
            InitHitboxesOn(wSkill);

            float t = 0f;
            while (t < iceCreamWReach)
            {
                if (wSkill == null) yield break;

                float prev = t;
                t = Mathf.MoveTowards(t, iceCreamWReach, iceCreamWSpeed * Time.deltaTime);
                wSkill.transform.position += (t - prev) * dir;
                yield return null;
            }

            if (wSkill != null) Destroy(wSkill);
        }

        usingSkill1 = false;
    }

    private IEnumerator CastIceCreamSkill2()
    {
        float originalSpeed = MoveSpeed;
        MoveSpeed = originalSpeed * iceCreamESlowMultiplier;
        Hp = Mathf.Min(Hp + iceCreamEHealAmount, 100f);

        GameObject fx = null;
        if (iceCreamEPrefab != null)
        {
            fx = Instantiate(iceCreamEPrefab, transform.position, Quaternion.identity, transform);

            var hitbox = fx.GetComponent<EnemySkillHitbox>();
            if (hitbox != null)
                hitbox.Init(this);
        }

        float elapsed = 0f;
        while (elapsed < iceCreamESlowDuration)
        {
            elapsed += Time.deltaTime;
            MoveSpeed = Mathf.Lerp(originalSpeed * iceCreamESlowMultiplier, originalSpeed, elapsed / iceCreamESlowDuration);
            yield return null;
        }

        MoveSpeed = originalSpeed;
        if (fx != null) Destroy(fx);

        usingSkill2 = false;
    }
    private void InitHitboxesOn(GameObject obj)
    {
        if (obj == null) return;
        var hitboxes = obj.GetComponentsInChildren<EnemySkillHitbox>(true);
        foreach (var hb in hitboxes)
            hb.Init(this);
    }
}
