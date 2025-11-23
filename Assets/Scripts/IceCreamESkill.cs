using System.Collections;
using UnityEngine;

public class IceCreamESkill : MonoBehaviour
{
    [Header("Ice Cream E")]
    public float heal = 20f;
    public float speedDiscount = -2f;
    public float duration = 3f;
    public float cooldown = 5f;

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

        //힐
        //속도감소
        StartCoroutine(Cooldown());

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            //속도회복
            yield return null;
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isEActive = false;
    }
}
