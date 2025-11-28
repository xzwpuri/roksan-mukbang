using System.Collections;
using UnityEngine;

public class FriesWSkill : MonoBehaviour
{
    [Header("Fries W")]
    [SerializeField] private float speed = 15f;
    [SerializeField] private float cooldown = 3;
    [SerializeField] private float reach = 20f;
    [SerializeField] private float width = 1.5f;
    [SerializeField] private float height = 0.4f;

    public GameObject FriesWPrefab;

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

        var (dir, angleToMouse, skillPos) = MouseDirection.Mouse(transform);
        GameObject wSkill = Instantiate(FriesWPrefab, skillPos, Quaternion.Euler(0, 0, angleToMouse));
        wSkill.transform.localScale = FriesESkill.isUpgraded ? new Vector3(width * 2f, height * 2f, 1f) : new Vector3(width, height, 1f);
        StartCoroutine(Cooldown());
        FriesESkill.isUpgraded = false;

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
