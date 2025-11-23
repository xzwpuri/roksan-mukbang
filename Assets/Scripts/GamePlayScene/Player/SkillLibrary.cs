using System.Collections;
using UnityEngine;

public static class SkillLibrary
{
    // Q/R: 고정 스킬 (예시)
    public static void Q_Fixed(Player c)
    {
        Debug.Log("[Q] 고정 스킬 발동");
    }

    public static void R_Fixed(Player c)
    {
        Debug.Log("[R] 고정 궁극기 발동");
    }

    // ===========================
    //  공통: 마우스 방향 계산
    // ===========================
    private static (Vector3 dir, float angleToMouse, Vector3 skillPos) MouseDirection(Player c)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 dir = (mousePos - c.transform.position).normalized;
        float angleToMouse = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Vector3 skillPos = c.transform.position + dir * 0.3f;

        return (dir, angleToMouse, skillPos);
    }

    // ===========================
    // Default W/E 구현
    // ===========================
    public static void W_Default(Player c)
    {
        // 찌르기 (Jab) – 중복 시도 방지
        if (c.isJabbing) return;
        c.StartCoroutine(JabCoroutine(c));
    }

    public static void E_Default(Player c)
    {
        // 휘두르기 (Swing) – 중복 시도 방지
        if (c.isSwinging) return;
        c.StartCoroutine(SwingCoroutine(c));
    }

    private static IEnumerator JabCoroutine(Player c)
    {
        c.isJabbing = true;

        var (dir, angleToMouse, skillPos) = MouseDirection(c);

        if (c.jabPivotPrefab == null)
        {
            Debug.LogWarning("[SkillLibrary] jabPivotPrefab 이 설정되어 있지 않습니다.");
            c.isJabbing = false;
            yield break;
        }

        GameObject jab = Object.Instantiate(
            c.jabPivotPrefab,
            skillPos,
            Quaternion.Euler(0, 0, angleToMouse)
        );
        jab.transform.SetParent(c.transform);

        float t = 0f;

        while (t < 1f)
        {
            float tt = t < 0.5f ? t * 2f : (t - 0.5f) * 2f;
            float scale;

            if (t < 0.5f)
                scale = Mathf.Pow(tt, 5);
            else
                scale = 1f - Mathf.Pow(tt, 5);

            jab.transform.localScale = new Vector3(scale * c.jabReach, c.jabWidth, 1f);

            t += Time.deltaTime * c.jabSpeed;
            yield return null;
        }

        Object.Destroy(jab);
        yield return new WaitForSeconds(c.jabCooldown);
        c.isJabbing = false;
    }

    private static IEnumerator SwingCoroutine(Player c)
    {
        c.isSwinging = true;

        var (dir, angleToMouse, skillPos) = MouseDirection(c);

        if (c.swingPivotPrefab == null)
        {
            Debug.LogWarning("[SkillLibrary] swingPivotPrefab 이 설정되어 있지 않습니다.");
            c.isSwinging = false;
            yield break;
        }

        GameObject swing = Object.Instantiate(
            c.swingPivotPrefab,
            skillPos,
            Quaternion.Euler(0, 0, angleToMouse + c.swingAngle1)
        );
        swing.transform.SetParent(c.transform);
        swing.transform.localScale = new Vector3(c.swingReach, c.swingWidth, 1f);

        float currentAngle = c.swingAngle1;

        while (currentAngle > c.swingAngle2)
        {
            currentAngle -= c.swingSpeed * Time.deltaTime;
            swing.transform.rotation = Quaternion.Euler(0, 0, angleToMouse + currentAngle);
            yield return null;
        }

        Object.Destroy(swing);
        yield return new WaitForSeconds(c.swingCooldown);
        c.isSwinging = false;
    }

    // ===========================
    // Fire / Water / Grass 그대로
    // ===========================
    public static void W_Fire(Player c)
    {
        Debug.Log("[W] Fire");
    }
    public static void E_Fire(Player c)
    {
        Debug.Log("[E] Fire");
    }

    public static void W_Water(Player c)
    {
        Debug.Log("[W] Water");
    }
    public static void E_Water(Player c)
    {
        Debug.Log("[E] Water");
    }

    public static void W_Grass(Player c)
    {
        Debug.Log("[W] Grass");
    }
    public static void E_Grass(Player c)
    {
        Debug.Log("[E] Grass");
    }
}
