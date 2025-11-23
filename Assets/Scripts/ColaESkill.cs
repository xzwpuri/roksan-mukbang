using System.Collections;
using UnityEngine;

public class ColaESkill : MonoBehaviour
{
    [Header("Cola E")]
    public float speed = 3f;
    public float cooldown = 6;
    public float startScale = 0f;
    public float endScale = 8f; 
    public GameObject ColaEPrefab;

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

        GameObject eSkill = Instantiate(ColaEPrefab, transform.position, Quaternion.identity);
        eSkill.transform.localScale = new Vector3(startScale, startScale, 1f);
        StartCoroutine(Cooldown());

        float t = 0f;
        while (t < 1f)
        {
            float easing = Mathf.Sqrt(1 - Mathf.Pow(t - 1f, 2));
            easing = Mathf.Clamp01(easing);
            float scale = Mathf.Lerp(startScale, endScale, easing);
            eSkill.transform.localScale = new Vector3(scale, scale, 1f);
            t += Time.deltaTime * speed;
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
