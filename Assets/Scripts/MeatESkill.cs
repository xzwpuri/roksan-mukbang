using System.Collections;
using UnityEngine;

public class MeatESkill : MonoBehaviour
{
    [Header("Meat E")]
    [SerializeField] private float meatESpeed = 10f;
    [SerializeField] private float meatEDistance = 3f;

    private bool isMeatEActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isMeatEActive)
        {
            StartCoroutine(E());
        }
    }

    IEnumerator E()
    {
        isMeatEActive = true;

        var (dir, _, _) = MouseDirection.Mouse(transform);
        float t = 0f;
        Vector3 startPos = transform.position;
        while (t < meatEDistance)
        {
            t = Mathf.MoveTowards(t, meatEDistance, meatESpeed * Time.deltaTime);

            float normalized = t / meatEDistance;
            float easing = 1f - Mathf.Pow(1f - normalized, 5f);
            transform.position = startPos + dir * (meatEDistance * easing);
            yield return null;
        }
        isMeatEActive = false;
    }
}
