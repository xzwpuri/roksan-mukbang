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
        Init(48f, 2.6f, 2, 2);
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

        GameObject buffFx = null;
        if (colaWPrefab != null)
            buffFx = Instantiate(colaWPrefab, transform.position, Quaternion.identity, transform);

        yield return new WaitForSeconds(colaWDuration);

        MoveSpeed = originalSpeed;
        if (buffFx != null) Destroy(buffFx);

        usingSkill1 = false;
    }

    private IEnumerator CastColaSkill2()
    {
        if (colaEPrefab != null)
        {
            GameObject eSkill = Instantiate(colaEPrefab, transform.position, Quaternion.identity);
            eSkill.transform.localScale = new Vector3(colaEStartScale, colaEStartScale, 1f);
            yield return WaterESequence(eSkill, colaEStartScale, colaEEndScale, colaESpeed);
            Destroy(eSkill);
        }

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
}
