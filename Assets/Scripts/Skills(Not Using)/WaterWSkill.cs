using System.Collections;
using UnityEngine;

public class WaterWSkill : MonoBehaviour
{
    [Header("Water W")]
    [SerializeField] private float waterWSpeed = 10f;
    [SerializeField] private float waterWReach = 20f;
    [SerializeField] private float waterWRadius = 0.5f;
    [SerializeField] private float waterWGanGyeock = 0.1f;
    public GameObject WaterWPrefab;

    private bool isWaterWActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isWaterWActive)
        {
            StartCoroutine(W());
        }
    }

    IEnumerator W()
    {
        isWaterWActive = true;
        var (dir, _, skillPos) = MouseDirection.Mouse(transform);

        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(Water(skillPos, dir));
            yield return new WaitForSeconds(waterWGanGyeock);
        }
        isWaterWActive = false;
    }

    IEnumerator Water(Vector3 skillPos, Vector3 dir)
    {
        GameObject wSkill = Instantiate(WaterWPrefab, skillPos, Quaternion.identity);
        wSkill.transform.localScale = new Vector3(waterWRadius, waterWRadius, 1f);

        Collide p = wSkill.GetComponentInChildren<Collide>();

        bool isHit = false;
        p.onHit = (any) =>
        {
            if (isHit) return;
            isHit = true;
            Destroy(wSkill);
        };

        float t = 0f;
        while (t < waterWReach)
        {
            if (isHit || wSkill == null) break;

            float tt = t;
            t = Mathf.MoveTowards(t, waterWReach, waterWSpeed * Time.deltaTime);
            float move = t - tt;

            wSkill.transform.position += move * dir;
            yield return null;
        }
        if (wSkill != null) Destroy(wSkill);
    }
}