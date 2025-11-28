using System.Collections;
using UnityEngine;

public class WaterESkill : MonoBehaviour
{
    [Header("Water E")]
    [SerializeField] private float speed = 1f;
    [SerializeField] private float cooldown = 4;
    [SerializeField] private float startScale = 0f;
    [SerializeField] private float endScale = 5f;
    public GameObject WaterEPrefab;

    private bool isEActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isEActive)
        {
            StartCoroutine(E());
        }
    }

    IEnumerator E()
    {
        isEActive = true;

        GameObject eSkill = Instantiate(WaterEPrefab, transform.position, Quaternion.identity);
        eSkill.transform.localScale = new Vector3(startScale, startScale, 1f);
        StartCoroutine(Cooldown());

        float t = 0f;
        while (t < 1f)
        {
            t = Mathf.MoveTowards(t, 1f, Time.deltaTime * speed);

            float easing = Mathf.Sqrt(1 - Mathf.Pow(t - 1f, 2));
            float scale = Mathf.Lerp(startScale, endScale, easing);

            eSkill.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
        Destroy(eSkill);
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isEActive = false;
    }
}
