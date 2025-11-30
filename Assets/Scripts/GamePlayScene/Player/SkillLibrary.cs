using System.Collections;
using UnityEngine;

public static class SkillLibrary
{
    // Q/R: 고정 스킬
    public static void Q_Fixed(Player c)
    {
        // Debug.Log("[Q] Q_Fixed 호출됨");

        if (c == null)
        {
            // Debug.LogError("[Q] Player c 가 null 입니다.");
            return;
        }

        c.StartCoroutine(SwallowNearestDeadCoroutine(c));
    }

    public static void R_Fixed(Player c)
    {
        // Debug.Log("[R] 고정 궁극기 발동");
    }

    // ===========================
    //  W/E 기본 스킬
    // ===========================

    public static void W_Default(Player c)
    {
        // 찌르기 (Jab) – 중복 시도 방지
        if (c.isW) return;
        c.StartCoroutine(JabCoroutine(c));
    }

    public static void E_Default(Player c)
    {
        // 휘두르기 (Swing) – 중복 시도 방지
        if (c.isE) return;
        c.StartCoroutine(SwingCoroutine(c));
    }

    // ===========================
    //  Q 스킬: 시체 삼키기
    // ===========================

    private static IEnumerator SwallowNearestDeadCoroutine(Player self)
    {
        float searchRadius = 5f;
        float swallowTime = 0.2f;

        if (self == null) yield break;

        // Debug.Log("[Q] 시체 탐색 시작");

        Collider2D[] hits = Physics2D.OverlapCircleAll(self.transform.position, searchRadius);
        // Debug.Log($"[Q] OverlapCircleAll 결과 개수: {hits.Length}");

        Transform closestTarget = null;
        IUnit targetUnit = null;
        float closestDistSq = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            // Debug.Log($"[Q] hit: {hit.name}, tag={hit.tag}");

            if (!hit.TryGetComponent<IUnit>(out var unit))
            {
                // Debug.Log($"[Q] {hit.name} 에 IUnit 없음, 스킵");
                continue;
            }

            // 자기 자신이면 스킵
            if (hit.transform == self.transform)
            {
                // Debug.Log("[Q] 자기 자신이라 스킵");
                continue;
            }

            // Dead 태그 아니면 스킵
            if (!hit.CompareTag("Dead"))
            {
                // Debug.Log($"[Q] {hit.name} 은 Dead 태그 아님 (현재 tag={hit.tag})");
                continue;
            }

            float distSq = (hit.transform.position - self.transform.position).sqrMagnitude;
            // Debug.Log($"[Q] 후보: {hit.name}, distSq={distSq}, stomach={unit.Stomach}");

            if (distSq < closestDistSq)
            {
                closestDistSq = distSq;
                closestTarget = hit.transform;
                targetUnit = unit;
            }
        }

        if (closestTarget == null || targetUnit == null)
        {
            // Debug.Log("[Q] 주변에 Dead 태그 + IUnit 있는 대상이 없음");
            yield break;
        }

        // Debug.Log($"[Q] 최종 선택: {closestTarget.name}, stomach={targetUnit.Stomach}, element={targetUnit.Element}");

        // 2. 빨려오기 연출
        Vector3 startPos = closestTarget.position;
        Vector3 endPos = self.transform.position;
        float elapsed = 0f;

        while (elapsed < swallowTime)
        {
            if (closestTarget == null)
            {
                // Debug.LogWarning("[Q] 이동 도중 대상이 파괴됨");
                yield break;
            }

            float t = elapsed / swallowTime;
            closestTarget.position = Vector3.Lerp(startPos, endPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (closestTarget != null)
            closestTarget.position = endPos;

        // 3. Stomach + Element 흡수
        int gainedStomach = targetUnit.Stomach;
        int gainedElement = targetUnit.Element;

        // Stomach는 더하기 (누적)
        self.Setstomach(self.Stomach + gainedStomach);

        // Element는 먹은 대상의 속성으로 변경(원하면 조합 로직으로 바꿔도 됨)
        self.Element = gainedElement;

        // Debug.Log($"[Q] 흡수 후 stomach={self.Stomach}, element={self.Element}");

        // 4. 삭제
        if (closestTarget != null)
        {
            // Debug.Log($"[Q] {closestTarget.name} 삭제");
            Object.Destroy(closestTarget.gameObject);
        }
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
    //  Default W/E 구현
    // ===========================

    private static IEnumerator JabCoroutine(Player c)
    {
        c.isW = true;

        var (dir, angleToMouse, skillPos) = MouseDirection(c);

        if (c.jabPivotPrefab == null)
        {
            // Debug.LogWarning("[SkillLibrary] jabPivotPrefab 이 설정되어 있지 않습니다.");
            c.isW = false;
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

        // 쿨타임 제거: 바로 사용 가능 상태로
        c.isW = false;
    }


    private static IEnumerator SwingCoroutine(Player c)
    {
        c.isE = true;

        var (dir, angleToMouse, skillPos) = MouseDirection(c);

        if (c.swingPivotPrefab == null)
        {
            // Debug.LogWarning("[SkillLibrary] swingPivotPrefab 이 설정되어 있지 않습니다.");
            c.isE = false;
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

        // 쿨타임 제거: 바로 사용 가능 상태로
        c.isE = false;
    }


}
