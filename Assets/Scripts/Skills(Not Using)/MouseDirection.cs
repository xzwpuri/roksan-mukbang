using UnityEngine;

public class MouseDirection : MonoBehaviour
{
    public static (Vector3 dir, float angleToMouse, Vector3 skillPos) Mouse(Transform transform)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector3 dir = (mousePos - transform.position).normalized;
        float angleToMouse = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Vector3 skillPos = transform.position + dir * 0.3f;
        return (dir, angleToMouse, skillPos);
    }
}
