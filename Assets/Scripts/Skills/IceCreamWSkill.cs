using System.Collections;
using UnityEngine;

public class IceCreamWSkill : MonoBehaviour
{
    [Header("Ice Cream W")]
    [SerializeField] private float iceCreamWSpeed = 6;
    [SerializeField] private float iceCreamWReach = 10f;
    [SerializeField] private float iceCreamWRadius = 1f;

    public GameObject IceCreamWPrefab;
    private bool isIceCreamWActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isIceCreamWActive)
        {
            StartCoroutine(W());
        }
    }

    IEnumerator W()
    {
        isIceCreamWActive = true;
        var (dir, _, skillPos) = MouseDirection.Mouse(transform);
        GameObject wSkill = Instantiate(IceCreamWPrefab, skillPos, Quaternion.identity);
        wSkill.transform.localScale = new Vector3(iceCreamWRadius, iceCreamWRadius, 1f);

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
        while (t < iceCreamWReach)
        {
            if (isHit || wSkill == null) break;

            float tt = t;
            t = Mathf.MoveTowards(t, iceCreamWReach, iceCreamWSpeed * Time.deltaTime);
            float move = t - tt;

            wSkill.transform.position += move * dir;
            yield return null;
        }
        if (wSkill != null) Destroy(wSkill);
        isIceCreamWActive = false;
    }
}
