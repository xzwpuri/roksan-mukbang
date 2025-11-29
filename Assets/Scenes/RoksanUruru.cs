using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoksanUruru : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private float distance = 800f;
    [SerializeField] private float speed = 350f;

    private bool isEntered = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isEntered)
        {
            isEntered = true;
            StartCoroutine(Ururu());
        }
    }

    IEnumerator Ururu()
    {
        Vector3 startPos = transform.position;
        float t = 0f;
        while (t < distance)
        {
            t = Mathf.MoveTowards(t, distance, speed * Time.deltaTime);

            float normalized = t / distance;
            float easing = 1f - Mathf.Pow(1f - normalized, 5f);
            transform.position = startPos + Vector3.right * (distance * easing);
            yield return null;
        }
    }
}
