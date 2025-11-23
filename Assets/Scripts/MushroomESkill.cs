using System.Collections;
using UnityEngine;

public class MushroomESkill : MonoBehaviour
{
    [Header("Mushroom E")]
    public float heal = 30f;
    public float duration = 5f;
    public float cooldown = 7f;

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
            t += Time.deltaTime;
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
