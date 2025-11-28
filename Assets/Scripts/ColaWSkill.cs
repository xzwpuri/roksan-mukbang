using System.Collections;
using UnityEngine;

public class ColaWSkill : MonoBehaviour
{
    [Header("Cola W")]
    [SerializeField] private float cooldown = 5f;
    [SerializeField] private float duration = 3f;

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
        yield return new WaitForSeconds(duration);
        // 스피드 원상복귀
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isWActive = false;
    }
}
