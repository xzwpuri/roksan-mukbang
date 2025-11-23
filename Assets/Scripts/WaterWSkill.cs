using System.Collections;
using UnityEngine;

public class WaterWSkill : MonoBehaviour
{
    [Header("Water W")]
    public float speed = 10f;
    public float cooldown = 3;
    public float reach = 20f;
    public float radius = 0.5f;
    public float ganGyeock = 0.1f;
    public GameObject WaterWPrefab;

    private bool isWActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isWActive)
        {
            StartCoroutine(W());
        }
    }

    IEnumerator W()
    {
        isWActive = true;
        var (dir, _, skillPos) = MouseDirection.Mouse(transform);
        for (int i = 0; i < 3; i++)
        {
            StartCoroutine(Water(skillPos, dir));
            yield return new WaitForSeconds(ganGyeock);
        }
        yield return new WaitForSeconds(cooldown);
        isWActive = false;
    }

    IEnumerator Water(Vector3 skillPos, Vector3 dir)
    {
        GameObject wSkill = Instantiate(WaterWPrefab, skillPos, Quaternion.identity);
        wSkill.transform.localScale = new Vector3(radius, radius, 1f);

        Collide p = wSkill.GetComponentInChildren<Collide>();

        bool isHit = false;
        p.onHit = (any) =>
        {
            if (isHit) return;
            isHit = true;
            Destroy(wSkill);
        };

        float t = 0f;
        while (t < reach)
        {
            if (isHit || wSkill == null) break;
            float move = Mathf.Min(speed * Time.deltaTime, reach - t);
            wSkill.transform.position += move * dir;
            t += move;
            yield return null;
        }
        if (wSkill != null) Destroy(wSkill);
    }
}
