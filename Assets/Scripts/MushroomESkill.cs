using System.Collections;
using UnityEngine;

public class MushroomESkill : MonoBehaviour
{
    [Header("Mushroom E")]
    [SerializeField] private float duration = 5f;
    [SerializeField] private float cooldown = 7f;

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

        float t = 0f;
        while (t < duration)
        {
            t = Mathf.MoveTowards(t, duration, Time.deltaTime);
            //Èú ÃÊ¸¶´Ù
            yield return null;
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isEActive = false;
    }
}
