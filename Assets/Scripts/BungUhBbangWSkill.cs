using System.Collections;
using UnityEngine;

public class BungUhBbangWSkill : MonoBehaviour
{
    [Header("BungUhBbang W")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float cooldown = 5;
    [SerializeField] private float reach = 20f;
    [SerializeField] private float radius = 0.7f;

    public GameObject BungUhBbangWPrefab;

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

        StartCoroutine(Cooldown());

        if (BungUhBbangESkill.isCustardCream)
        {
            for (int i = 0; i < 8; i++)
            {
                float angle = (360f / 8) * i;
                StartCoroutine(Projectile(transform.position, Quaternion.Euler(0, 0, angle) * dir));
            }
        }
        else
        {
            StartCoroutine(Projectile(skillPos, Quaternion.Euler(0, 0, -45f) * dir));
            StartCoroutine(Projectile(skillPos, dir));
            StartCoroutine(Projectile(skillPos, Quaternion.Euler(0, 0, 45f) * dir));
        }
        yield return null;
    }

    IEnumerator Projectile(Vector3 skillPos, Vector3 dir)
    {
        GameObject wSkill = Instantiate(BungUhBbangWPrefab, skillPos, Quaternion.identity);
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

            float tt = t;
            t = Mathf.MoveTowards(t, reach, speed * Time.deltaTime);
            float move = t - tt;

            wSkill.transform.position += move * dir;
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
