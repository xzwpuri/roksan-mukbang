using System.Collections;
using UnityEngine;

public class BungUhBbangWSkill : MonoBehaviour
{
    [Header("BungUhBbang W")]
    [SerializeField] private float bungUhBbangWSpeed = 10f;
    [SerializeField] private float bungUhBbangWReach = 20f;
    [SerializeField] private float bungUhBbangWRadius = 0.7f;

    public GameObject BungUhBbangWPrefab;

    private bool isBungUhBbangWActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isBungUhBbangWActive)
        {
            StartCoroutine(W());
        }
    }

    IEnumerator W()
    {
        isBungUhBbangWActive = true;

        var (dir, _, skillPos) = MouseDirection.Mouse(transform);

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
        isBungUhBbangWActive = false;
        yield return null;
    }

    IEnumerator Projectile(Vector3 skillPos, Vector3 dir)
    {
        GameObject wSkill = Instantiate(BungUhBbangWPrefab, skillPos, Quaternion.identity);
        wSkill.transform.localScale = new Vector3(bungUhBbangWRadius, bungUhBbangWRadius, 1f);

        Collide p = wSkill.GetComponentInChildren<Collide>();

        bool isHit = false;
        p.onHit = (any) =>
        {
            if (isHit) return;
            isHit = true;
            Destroy(wSkill);
        };

        float t = 0f;
        while (t < bungUhBbangWReach)
        {
            if (isHit || wSkill == null) break;

            float tt = t;
            t = Mathf.MoveTowards(t, bungUhBbangWReach, bungUhBbangWSpeed * Time.deltaTime);
            float move = t - tt;

            wSkill.transform.position += move * dir;
            yield return null;
        }
        if (wSkill != null) Destroy(wSkill);
    }
}
