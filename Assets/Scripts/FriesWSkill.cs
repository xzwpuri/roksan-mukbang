using System.Collections;
using UnityEngine;

public class FriesWSkill : MonoBehaviour
{
    [Header("Fries W")]
    [SerializeField] private float friesWSpeed = 15f;
    [SerializeField] private float friesWReach = 20f;
    [SerializeField] private float friesWWidth = 1.5f;
    [SerializeField] private float friesWHeight = 0.4f;

    public GameObject FriesWPrefab;

    private bool isFriesWActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isFriesWActive)
        {
            StartCoroutine(W());
        }
    }

    IEnumerator W()
    {
        isFriesWActive = true;

        var (dir, angleToMouse, skillPos) = MouseDirection.Mouse(transform);
        GameObject wSkill = Instantiate(FriesWPrefab, skillPos, Quaternion.Euler(0, 0, angleToMouse));
        wSkill.transform.localScale = FriesESkill.isFriesUpgraded ? new Vector3(friesWWidth * 2f, friesWHeight * 2f, 1f) : new Vector3(friesWWidth, friesWHeight, 1f);
        FriesESkill.isFriesUpgraded = false;

        Collide p = wSkill.GetComponentInChildren<Collide>();

        bool isHit = false;
        p.onHit = (any) =>
        {
            if (isHit) return;
            isHit = true;
            Destroy(wSkill);
        };

        float t = 0f;
        while (t < friesWReach)
        {
            if (isHit || wSkill == null) break;

            float tt = t;
            t = Mathf.MoveTowards(t, friesWReach, friesWSpeed * Time.deltaTime);
            float move = t - tt;

            wSkill.transform.position += move * dir;
            yield return null;
        }
        if (wSkill != null) Destroy(wSkill);
        isFriesWActive = false;
    }
}
