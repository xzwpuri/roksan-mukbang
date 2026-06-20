using System.Collections;
using UnityEngine;

public class EnemyCoke : EnemyBase
{
    [Header("Cola Skill Prefabs")]
    [SerializeField] private GameObject colaWPrefab;
    [SerializeField] private GameObject colaEPrefab;

    [Header("Cola Skill Parameters")]
    [SerializeField] private float colaWDuration = 3f;
    [SerializeField] private float colaWSpeedMultiplier = 1.5f;
    [SerializeField] private float colaEStartScale = 0f;
    [SerializeField] private float colaEEndScale = 8f;
    [SerializeField] private float colaESpeed = 3f;

    private bool usingSkill1;
    private bool usingSkill2;

    protected override void Start()
    {
        base.Start();
        Init(20f, 3.0f, 1, 2);
    }

    public override void Skill1()
    {
        if (usingSkill1) return;
        usingSkill1 = true;
        StartCoroutine(CastColaSkill1());
    }

    public override void Skill2()
    {
        if (usingSkill2) return;
        usingSkill2 = true;
        StartCoroutine(CastColaSkill2());
    }

    private IEnumerator CastColaSkill1()
    {
        float originalSpeed = MoveSpeed;
        MoveSpeed *= colaWSpeedMultiplier;

        SpawnBuffEffect(colaWPrefab);

    yield return new WaitForSeconds(colaWDuration);

    MoveSpeed = originalSpeed;

        usingSkill1 = false;
    }

    private IEnumerator CastColaSkill2()
    {
        if (colaEPrefab != null)
        {
            GameObject eSkill = Instantiate(colaEPrefab, transform.position, Quaternion.identity);
            eSkill.transform.localScale = new Vector3(colaEStartScale, colaEStartScale, 1f);

            // ✅ 바닥 범위 공격일 가능성 높음 → owner 연결
            InitHitboxesOn(eSkill);

            yield return WaterESequence(eSkill, colaEStartScale, colaEEndScale, colaESpeed);
            Destroy(eSkill);
        }
        GetDamage(Hp, 0);
        usingSkill2 = false;
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
    private void InitHitboxesOn(GameObject obj)
    {
        if (obj == null) return;
        var hitboxes = obj.GetComponentsInChildren<EnemySkillHitbox>(true);
        foreach (var hb in hitboxes)
            hb.Init(this);
    }
}
