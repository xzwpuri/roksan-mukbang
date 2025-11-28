using System.Collections;
using UnityEngine;

public class ColaESkill : MonoBehaviour
{
    [Header("Cola E")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float cooldown = 6;
    [SerializeField] private float startScale = 0f;
    [SerializeField] private float endScale = 8f; 
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
