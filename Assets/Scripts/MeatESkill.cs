using System.Collections;
using UnityEngine;

public class MeatESkill : MonoBehaviour
{
    [Header("Meat E")]
    public float speed = 10f;
    public float cooldown = 3f;
    public float distance = 3f;

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
            float normalized = t / distance;
            float move = Mathf.Min(speed * Time.deltaTime, distance - t);
            float easing = 1f - Mathf.Pow(1f - normalized, 5f);
            transform.position = startPos + dir * (distance * easing);
            t += move;
            yield return null;
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isEActive = false;
    }
}
