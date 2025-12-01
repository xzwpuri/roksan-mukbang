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
    private GameObject activeWSkill;

    protected override void Start()
    {
        base.Start();
        Init(80f, 1.8f, 2, 6);
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

    private void OnDisable()
    {
        CleanupMushroomW();
    }

    private IEnumerator CastMushroomSkill1()
    {
        Vector3 dir = Target != null ? (Target.position - transform.position).normalized : Vector3.right;

        if (mushroomWPrefab != null)
        {
            GameObject wSkill = Instantiate(mushroomWPrefab, transform.position + dir * 3f, Quaternion.identity);
            activeWSkill = wSkill;
            wSkill.transform.localScale = new Vector3(mushroomWRadius, mushroomWRadius, 1f);

            // ✅ 버섯 독구름 장판 히트박스 연결
            InitHitboxesOn(wSkill);

            yield return new WaitForSeconds(mushroomWDuration);

            CleanupMushroomW();
        }

        usingSkill1 = false;
    }
    private IEnumerator CastMushroomSkill2()
    {
        SpawnBuffEffect(mushroomEPrefab);

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

    private void InitHitboxesOn(GameObject obj)
    {
        if (obj == null) return;
    var hitboxes = obj.GetComponentsInChildren<EnemySkillHitbox>(true);
    foreach (var hb in hitboxes)
        hb.Init(this);
    }

    private void CleanupMushroomW()
    {
        if (activeWSkill != null)
        {
            Destroy(activeWSkill);
            activeWSkill = null;
        }

        usingSkill1 = false;
    }
}