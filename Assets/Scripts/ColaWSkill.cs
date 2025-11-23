using System.Collections;
using UnityEngine;

public class ColaWSkill : MonoBehaviour
{
    [Header("Cola W")]
    public float increaseSpeed = 3f;
    public float cooldown = 5f;
    public float duration = 3f;

    private bool isWActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isWActive)
        {
            StartCoroutine(W());
        }
    }

    IEnumerator W()
    {
        isWActive = true;
        StartCoroutine(Cooldown());

        // 스피드 증가

        float t = 0f;
        while (t < duration)
        {
            float move = Mathf.Min(Time.deltaTime, duration - t);
            t += move;
            yield return null;
        }
        // 스피드 원상복귀
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isWActive = false;
    }
}
