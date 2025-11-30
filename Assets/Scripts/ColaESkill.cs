using System.Collections;
using UnityEngine;

public class ColaESkill : MonoBehaviour
{
    [Header("Cola E")]
    [SerializeField] private float colaESpeed = 3f;
    [SerializeField] private float colaEStartScale = 0f;
    [SerializeField] private float colaEEndScale = 8f; 
    public GameObject ColaEPrefab;

    private bool isColaEActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isColaEActive)
        {
            StartCoroutine(E());
        }
    }

    IEnumerator E()
    {
        isColaEActive = true;

        GameObject eSkill = Instantiate(ColaEPrefab, transform.position, Quaternion.identity);
        eSkill.transform.localScale = new Vector3(colaEStartScale, colaEStartScale, 1f);

        float t = 0f;
        while (t < 1f)
        {
            t = Mathf.MoveTowards(t, 1f, Time.deltaTime * colaESpeed);

            float easing = Mathf.Sqrt(1 - Mathf.Pow(t - 1f, 2));
            float scale = Mathf.Lerp(colaEStartScale, colaEEndScale, easing);
            eSkill.transform.localScale = new Vector3(scale, scale, 1f);

            yield return null;
        }
        Destroy(eSkill);
        isColaEActive = false;
    }
}
