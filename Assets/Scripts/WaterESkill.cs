using System.Collections;
using UnityEngine;

public class WaterESkill : MonoBehaviour
{
    [Header("Water E")]
    [SerializeField] private float waterESpeed = 1f;
    [SerializeField] private float waterEStartScale = 0f;
    [SerializeField] private float waterEEndScale = 5f;
    public GameObject WaterEPrefab;

    private bool isWaterEActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isWaterEActive)
        {
            StartCoroutine(E());
        }
    }

    IEnumerator E()
    {
        isWaterEActive = true;

        GameObject eSkill = Instantiate(WaterEPrefab, transform.position, Quaternion.identity);
        eSkill.transform.localScale = new Vector3(waterEStartScale, waterEStartScale, 1f);

        float t = 0f;
        while (t < 1f)
        {
            t = Mathf.MoveTowards(t, 1f, Time.deltaTime * waterESpeed);

            float easing = Mathf.Sqrt(1 - Mathf.Pow(t - 1f, 2));
            float scale = Mathf.Lerp(waterEStartScale, waterEEndScale, easing);

            eSkill.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
        Destroy(eSkill);
        isWaterEActive = false;
    }
}
