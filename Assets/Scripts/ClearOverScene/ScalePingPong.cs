using UnityEngine;

public class ScalePingPong : MonoBehaviour
{
    [Header("스케일 설정")]
    [SerializeField] private float minScale = 5f;   // 최소 스케일
    [SerializeField] private float maxScale = 10f;  // 최대 스케일

    [Header("애니메이션 속도")]
    [SerializeField] private float speed = 1f;      // 값이 클수록 빨리 커졌다 작아짐

    private void Update()
    {
        // 0 ~ 1 사이를 왔다갔다하는 값
        float t = Mathf.PingPong(Time.time * speed, 1f);

        // minScale ~ maxScale 사이를 보간
        float scale = Mathf.Lerp(minScale, maxScale, t);

        // x, y, z 모두 같은 비율로 스케일 적용
        transform.localScale = new Vector3(5, scale, scale);
    }
}