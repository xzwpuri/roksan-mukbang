using System.Collections;
using UnityEngine;

public class ColaWSkill : MonoBehaviour
{
    [Header("Cola W")]
    [SerializeField] private float colaWDuration = 3f;

    private bool isColaWActive = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !isColaWActive)
        {
            StartCoroutine(W());
        }
    }

    IEnumerator W()
    {
        isColaWActive = true;

        // 스피드 증가
        Debug.Log("스피드 증가");
        yield return new WaitForSeconds(colaWDuration);
        // 스피드 원상복귀
        Debug.Log("스피드 원상복귀");
        isColaWActive = false;
    }
}
