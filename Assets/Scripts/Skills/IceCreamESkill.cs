using System.Collections;
using UnityEngine;

public class IceCreamESkill : MonoBehaviour
{
    [Header("Ice Cream E")]
    [SerializeField] private float iceCreamEDuration = 3f;

    private bool isIceCreamEActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isIceCreamEActive)
        {
            StartCoroutine(E());
        }
    }

    IEnumerator E()
    {
        isIceCreamEActive = true;

        //힐
        //속도감소
        Debug.Log("힐, 속도감소");

        float t = 0f;
        while (t < iceCreamEDuration)
        {
            t = Mathf.MoveTowards(t, iceCreamEDuration, Time.deltaTime);
            //천천히 속도회복
            Debug.Log("천천히 속도회복, 속도: " + t / iceCreamEDuration);
            yield return null;
        }
        isIceCreamEActive = false;
    }
}
