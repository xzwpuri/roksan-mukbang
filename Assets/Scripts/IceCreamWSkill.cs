using System.Collections;
using UnityEngine;

public class IceCreamWSkill : MonoBehaviour
{
    [Header("Ice Cream W")]
    public float speed = 6;
    public float cooldown = 3;
    public float reach = 10f;
    public float radius = 1f;

    public GameObject IceCreamWPrefab;
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
        GameObject wSkill = Instantiate(IceCreamWPrefab, skillPos, Quaternion.identity);
        wSkill.transform.localScale = new Vector3(radius, radius, 1f);
        StartCoroutine(Cooldown());

        Collide p = wSkill.GetComponent<Collide>();

        bool isHit = false;
        p.onHit = (enemy) => // enemy 충돌 시로 바꿔야함
        {
            if (isHit) return;
            isHit = true;
            // 속박
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

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isWActive = false;
    }
}
