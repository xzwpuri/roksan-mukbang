using System.Collections;
using UnityEngine;

public class EnemyMeat : EnemyBase
{
    [Header("Meat Skill Prefabs")]
    [SerializeField] private GameObject meatWPrefab;
    [SerializeField] private GameObject meatEPrefab;

    [Header("Meat Skill Parameters")]
    [SerializeField] private float meatWSpeed = 720f;
    [SerializeField] private float meatWAngle1 = 70f;
    [SerializeField] private float meatWAngle2 = -70f;
    [SerializeField] private float meatWWidth = 2f;
    [SerializeField] private float meatWHeight = 0.5f;
    [SerializeField] private float meatESpeed = 10f;
    [SerializeField] private float meatEDistance = 3f;

    private bool usingSkill1;
    private bool usingSkill2;

    protected override void Start()
    {
        base.Start();
        Init(40f, 2f, 3, 5);
    }

    public override void Skill1()
    {
        if (usingSkill1) return;
        usingSkill1 = true;
        StartCoroutine(CastMeatSkill1());
    }

    public override void Skill2()
    {
        if (usingSkill2) return;
        usingSkill2 = true;
        StartCoroutine(CastMeatSkill2());
    }

    private IEnumerator CastMeatSkill1()
    {
        Vector3 dir = Target != null ? (Target.position - transform.position).normalized : Vector3.right;
        float angleToTarget = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 skillPos = transform.position + dir * 0.4f;

        if (meatWPrefab != null)
        {
            GameObject wSkill = Instantiate(meatWPrefab, skillPos, Quaternion.Euler(0, 0, angleToTarget + meatWAngle1));
            wSkill.transform.SetParent(transform);
            wSkill.transform.localScale = new Vector3(meatWWidth, meatWHeight, 1f);

            // ✅ 근접 휩쓸기 히트박스 연결
            InitHitboxesOn(wSkill);

            float currentAngle = meatWAngle1;
            while (currentAngle > meatWAngle2)
            {
                currentAngle = Mathf.MoveTowardsAngle(currentAngle, meatWAngle2, meatWSpeed * Time.deltaTime);
                wSkill.transform.rotation = Quaternion.Euler(0, 0, angleToTarget + currentAngle);
                yield return null;
            }

            Destroy(wSkill);
        }

        usingSkill1 = false;
    }

    private IEnumerator CastMeatSkill2()
    {
        Vector3 dir = Target != null ? (Target.position - transform.position).normalized : Vector3.right;

        SpawnBuffEffect(meatEPrefab);

        float t = 0f;
        Vector3 startPos = transform.position;

        while (t < meatEDistance)
        {
            t = Mathf.MoveTowards(t, meatEDistance, meatESpeed * Time.deltaTime);
            float normalized = t / meatEDistance;
            float easing = 1f - Mathf.Pow(1f - normalized, 5f);
            transform.position = startPos + dir * (meatEDistance * easing);
            yield return null;
        }

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
