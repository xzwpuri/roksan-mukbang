using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoksanUruru : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float distance = 600f; // 오른쪽으로 얼마나 갈지
    [SerializeField] private float speed = 260f;    // 초당 이동 속도 (px/sec 느낌)

    private bool isEntered = false;
    private Vector3 basePos;        // 원래 위치
    private Coroutine moveRoutine;  // 현재 돌아가는 코루틴

    private void Awake()
    {
        basePos = transform.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isEntered) return;
        isEntered = true;

        // 오른쪽으로 distance만큼 이동한 위치로
        StartMove(basePos + Vector3.right * distance);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isEntered) return;
        isEntered = false;

        // 원래 자리로 복귀
        StartMove(basePos);
    }

    private void StartMove(Vector3 targetPos)
    {
        // 이전에 돌던 코루틴이 있으면 정지
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveTo(targetPos));
    }

    private IEnumerator MoveTo(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        float totalDist = Vector3.Distance(startPos, targetPos);

        if (totalDist <= 0.0001f)
        {
            moveRoutine = null;
            yield break;
        }

        float t = 0f;

        while (t < 1f)
        {
            // speed(거리/초)를 0~1 구간으로 환산
            t += (speed * Time.deltaTime) / totalDist;

            float eased = 1f - Mathf.Pow(1f - t, 5f); // ease-out
            transform.position = Vector3.Lerp(startPos, targetPos, eased);

            yield return null;
        }

        transform.position = targetPos;
        moveRoutine = null;
    }
}