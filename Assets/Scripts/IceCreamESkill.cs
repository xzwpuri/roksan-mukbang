using System.Collections;
using UnityEngine;

public class IceCreamESkill : MonoBehaviour
{
    [Header("Ice Cream E")]
    [SerializeField] private float duration = 3f;
    [SerializeField] private float cooldown = 5f;

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
            t = Mathf.MoveTowards(t, duration, Time.deltaTime);
            //천천히 속도회복
            yield return null;
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isEActive = false;
    }
}
