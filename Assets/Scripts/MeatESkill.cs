using System.Collections;
using UnityEngine;

public class MeatESkill : MonoBehaviour
{
    [Header("Meat E")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float cooldown = 3f;
    [SerializeField] private float distance = 3f;

    private bool isEActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isEActive)
        {
            StartCoroutine(E());
        }
    }

    IEnumerator E()
    {
        isEActive = true;

        StartCoroutine(Cooldown());

        var (dir, _, _) = MouseDirection.Mouse(transform);
        float t = 0f;
        Vector3 startPos = transform.position;
        while (t < distance)
        {
            t = Mathf.MoveTowards(t, distance, speed * Time.deltaTime);

            float normalized = t / distance;
            float easing = 1f - Mathf.Pow(1f - normalized, 5f);
            transform.position = startPos + dir * (distance * easing);
            yield return null;
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isEActive = false;
    }
}
