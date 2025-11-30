using System.Collections;
using UnityEngine;

public static class SkillLibrary
{
    // Q/R: 고정 스킬
    public static void Q_Fixed(Player c)
    {
        if (c == null) return;
        c.StartCoroutine(SwallowNearestDeadCoroutine(c));
    }

    public static void R_Fixed(Player c)
    {
        Debug.Log("[R] 고정 궁극기 발동");
    }

    // ===========================
    //  W/E 기본 스킬 (Stomach 0)
    // ===========================

    public static void W_Default(Player c)
    {
        if (c.isW) return;
        c.StartCoroutine(JabCoroutine(c));
    }

    public static void E_Default(Player c)
    {
        if (c.isE) return;
        c.StartCoroutine(SwingCoroutine(c));
    }

    // ===========================
    //  W/E 붕어빵 스킬 (Stomach 1)
    // ===========================

    public static void W_Bungeobbang(Player c)
    {
        if (c.isW) return;
        c.StartCoroutine(BungeobbangWCoroutine(c));
    }

    public static void E_Bungeobbang(Player c)
    {
        if (c.isE) return;
        c.StartCoroutine(BungeobbangECoroutine(c));
    }

    // ===========================
    //  W/E 콜라 스킬 (Stomach 2)
    // ===========================

    public static void W_Cola(Player c)
    {
        if (c.isW) return;
        c.StartCoroutine(ColaWCoroutine(c));
    }

    public static void E_Cola(Player c)
    {
        if (c.isE) return;
        c.StartCoroutine(ColaECoroutine(c));
    }

    // ===========================
    //  W/E 감자튀김 스킬 (Stomach 3)
    // ===========================

    public static void W_Fries(Player c)
    {
        if (c.isW) return;
        c.StartCoroutine(FriesWCoroutine(c));
    }

    public static void E_Fries(Player c)
    {
        if (c.isE) return;
        c.StartCoroutine(FriesECoroutine(c));
    }

    // ===========================
    //  W/E 아이스크림 스킬 (Stomach 4)
    // ===========================

    public static void W_IceCream(Player c)
    {
        if (c.isW) return;
        c.StartCoroutine(IceCreamWCoroutine(c));
    }

    public static void E_IceCream(Player c)
    {
        if (c.isE) return;
        c.StartCoroutine(IceCreamECoroutine(c));
    }

    // ===========================
    //  W/E 고기 스킬 (Stomach 5)
    // ===========================

    public static void W_Meat(Player c)
    {
        if (c.isW) return;
        c.StartCoroutine(MeatWCoroutine(c));
    }

    public static void E_Meat(Player c)
    {
        if (c.isE) return;
        c.StartCoroutine(MeatECoroutine(c));
    }

    // ===========================
    //  W/E 버섯 스킬 (Stomach 6)
    // ===========================

    public static void W_Mushroom(Player c)
    {
        if (c.isW) return;
        c.StartCoroutine(MushroomWCoroutine(c));
    }

    public static void E_Mushroom(Player c)
    {
        if (c.isE) return;
        c.StartCoroutine(MushroomECoroutine(c));
    }

    // ===========================
    //  W/E 물 스킬 (Stomach 7)
    // ===========================

    public static void W_Water(Player c)
    {
        if (c.isW) return;
        c.StartCoroutine(WaterWCoroutine(c));
    }

    public static void E_Water(Player c)
    {
        if (c.isE) return;
        c.StartCoroutine(WaterECoroutine(c));
    }

    // ===========================
    //  Q 스킬: 시체 삼키기
    // ===========================

    private static IEnumerator SwallowNearestDeadCoroutine(Player self)
    {
        float searchRadius = 5f;
        float swallowTime = 0.2f;

        if (self == null) yield break;

        Collider2D[] hits = Physics2D.OverlapCircleAll(self.transform.position, searchRadius);

        Transform closestTarget = null;
        IUnit targetUnit = null;
        float closestDistSq = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit == null) continue;

            if (!hit.TryGetComponent<IUnit>(out var unit))
                continue;

            if (hit.transform == self.transform)
                continue;

            if (!hit.CompareTag("Dead"))
                continue;

            float distSq = (hit.transform.position - self.transform.position).sqrMagnitude;

            if (distSq < closestDistSq)
            {
                closestDistSq = distSq;
                closestTarget = hit.transform;
                targetUnit = unit;
            }
        }

        if (closestTarget == null || targetUnit == null)
            yield break;

        // 빨려오기 연출
        Vector3 startPos = closestTarget.position;
        Vector3 endPos = self.transform.position;
        float elapsed = 0f;

        while (elapsed < swallowTime)
        {
            if (closestTarget == null)
                yield break;

            float t = elapsed / swallowTime;
            closestTarget.position = Vector3.Lerp(startPos, endPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (closestTarget != null)
            closestTarget.position = endPos;

        // Stomach + Element 흡수
        int gainedStomach = targetUnit.Stomach;
        int gainedElement = targetUnit.Element;

        self.Setstomach(self.Stomach + gainedStomach);
        self.Element = gainedElement;

        // 삭제
        if (closestTarget != null)
            Object.Destroy(closestTarget.gameObject);
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
        c.isW = false;
    }

    private static IEnumerator SwingCoroutine(Player c)
    {
        c.isE = true;

        var (dir, angleToMouse, skillPos) = MouseDirection(c);

        if (c.swingPivotPrefab == null)
        {
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
        c.isE = false;
    }

    // ===========================
    //  붕어빵 스킬 구현
    // ===========================

    private static IEnumerator BungeobbangWCoroutine(Player c)
    {
        c.isW = true;

        // 붕어빵 W 파라미터
        float speed = 10f;
        float reach = 20f;
        float radius = 0.7f;

        var (dir, _, skillPos) = MouseDirection(c);

        if (c.isCustardCream)
        {
            // 8방향 발사
            for (int i = 0; i < 8; i++)
            {
                float angle = (360f / 8) * i;
                c.StartCoroutine(BungeobbangProjectile(c, c.transform.position, Quaternion.Euler(0, 0, angle) * dir, speed, reach, radius));
            }
        }
        else
        {
            // 3방향 발사
            c.StartCoroutine(BungeobbangProjectile(c, skillPos, Quaternion.Euler(0, 0, -45f) * dir, speed, reach, radius));
            c.StartCoroutine(BungeobbangProjectile(c, skillPos, dir, speed, reach, radius));
            c.StartCoroutine(BungeobbangProjectile(c, skillPos, Quaternion.Euler(0, 0, 45f) * dir, speed, reach, radius));
        }

        c.isW = false;
        yield return null;
    }

    private static IEnumerator BungeobbangProjectile(Player c, Vector3 startPos, Vector3 dir, float speed, float reach, float radius)
    {
        // BungeobbangWPrefab 필요 (Player에 추가 필요)
        if (c.bungeobbangWPrefab == null) yield break;

        GameObject projectile = Object.Instantiate(c.bungeobbangWPrefab, startPos, Quaternion.identity);
        projectile.transform.localScale = new Vector3(radius, radius, 1f);

        // PlayerSkillHitbox가 알아서 데미지 처리
        float t = 0f;
        while (t < reach)
        {
            if (projectile == null) break;

            float tt = t;
            t = Mathf.MoveTowards(t, reach, speed * Time.deltaTime);
            float move = t - tt;

            projectile.transform.position += move * dir;
            yield return null;
        }

        if (projectile != null) Object.Destroy(projectile);
    }

    private static IEnumerator BungeobbangECoroutine(Player c)
    {
        c.isE = true;
        c.isCustardCream = true;
        Debug.Log("[Bungeobbang E] 커스터드 크림 활성화!");
        c.isE = false;
        yield return null;
    }

    // ===========================
    //  콜라 스킬 구현
    // ===========================

    private static IEnumerator ColaWCoroutine(Player c)
    {
        c.isW = true;

        float duration = 3f;          // 지속 시간
        float speedMultiplier = 1.5f; // 이동속도 1.5배

        c.StartColaSpeedBuff(ColaSpeedBuff(c, duration, speedMultiplier));

        c.isW = false;
        yield return null;
    }

    private static IEnumerator ColaSpeedBuff(Player c, float duration, float multiplier)
    {
        float originalSpeed = c.MoveSpeed;
        c.MoveSpeed *= multiplier;

        Debug.Log($"[Cola W] 이동속도 증가! {originalSpeed} -> {c.MoveSpeed}");

        yield return new WaitForSeconds(duration);

        c.MoveSpeed = originalSpeed;
        Debug.Log($"[Cola W] 이동속도 원래대로: {c.MoveSpeed}");
    }

    private static IEnumerator ColaECoroutine(Player c)
    {
        c.isE = true;

        float colaESpeed = 3f;
        float colaEStartScale = 0f;
        float colaEEndScale = 8f;

        if (c.colaEPrefab == null)
        {
            c.isE = false;
            yield break;
        }

        GameObject eSkill = Object.Instantiate(c.colaEPrefab, c.transform.position, Quaternion.identity);
        eSkill.transform.localScale = new Vector3(colaEStartScale, colaEStartScale, 1f);

        float t = 0f;
        while (t < 1f)
        {
            t = Mathf.MoveTowards(t, 1f, Time.deltaTime * colaESpeed);

            float easing = Mathf.Sqrt(1 - Mathf.Pow(t - 1f, 2));
            float scale = Mathf.Lerp(colaEStartScale, colaEEndScale, easing);
            eSkill.transform.localScale = new Vector3(scale, scale, 1f);

            yield return null;
        }

        Object.Destroy(eSkill);
        c.isE = false;
    }

    // ===========================
    //  아이스크림 스킬 구현
    // ===========================

    private static IEnumerator IceCreamWCoroutine(Player c)
    {
        c.isW = true;

        float iceCreamWSpeed = 12f;
        float iceCreamWReach = 20f;
        float iceCreamWRadius = 0.6f;

        var (dir, _, skillPos) = MouseDirection(c);

        if (c.iceCreamWPrefab == null)
        {
            c.isW = false;
            yield break;
        }

        GameObject wSkill = Object.Instantiate(c.iceCreamWPrefab, skillPos, Quaternion.identity);
        wSkill.transform.localScale = new Vector3(iceCreamWRadius, iceCreamWRadius, 1f);

        // IceCreamRootHitbox가 알아서 속박 처리함
        float t = 0f;
        while (t < iceCreamWReach)
        {
            if (wSkill == null) break;

            float tt = t;
            t = Mathf.MoveTowards(t, iceCreamWReach, iceCreamWSpeed * Time.deltaTime);
            float move = t - tt;

            wSkill.transform.position += move * dir;
            yield return null;
        }

        if (wSkill != null) Object.Destroy(wSkill);
        c.isW = false;
    }

    private static IEnumerator IceCreamECoroutine(Player c)
    {
        c.isE = true;

        float slowDuration = 4f;        // 이동속도 감소 지속시간
        float slowMultiplier = 0.5f;    // 이동속도 50%로 감소
        float healAmount = 30f;         // 회복량
        float recoverySpeed = 2f;       // 속도 회복 속도 (초당)

        c.StartIceCreamSlow(IceCreamSlowAndHeal(c, slowDuration, slowMultiplier, healAmount, recoverySpeed));

        c.isE = false;
        yield return null;
    }

    private static IEnumerator IceCreamSlowAndHeal(Player c, float duration, float slowMultiplier, float healAmount, float recoverySpeed)
    {
        float originalSpeed = c.MoveSpeed;
        float targetSpeed = originalSpeed * slowMultiplier;
        c.MoveSpeed = targetSpeed;

        // 체력 회복
        c.Hp = Mathf.Min(c.Hp + healAmount, 100f); // 최대 체력 100 가정

        Debug.Log($"[IceCream E] 이동속도 감소 & 체력 회복 +{healAmount}");

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // 서서히 원래 속도로 회복
            c.MoveSpeed = Mathf.Lerp(targetSpeed, originalSpeed, progress);

            yield return null;
        }

        c.MoveSpeed = originalSpeed;
        Debug.Log($"[IceCream E] 이동속도 완전 회복: {c.MoveSpeed}");
    }

    // ===========================
    //  감자튀김 스킬 구현
    // ===========================

    private static IEnumerator FriesWCoroutine(Player c)
    {
        c.isW = true;

        float friesWSpeed = 15f;
        float friesWReach = 20f;
        float friesWWidth = 1.5f;
        float friesWHeight = 0.4f;

        var (dir, angleToMouse, skillPos) = MouseDirection(c);

        if (c.friesWPrefab == null)
        {
            c.isW = false;
            yield break;
        }

        GameObject wSkill = Object.Instantiate(c.friesWPrefab, skillPos, Quaternion.Euler(0, 0, angleToMouse));

        // Player의 isFriesUpgraded 사용
        wSkill.transform.localScale = c.isFriesUpgraded
            ? new Vector3(friesWWidth * 2f, friesWHeight * 2f, 1f)
            : new Vector3(friesWWidth, friesWHeight, 1f);

        // 사용 후 강화 상태 초기화
        c.isFriesUpgraded = false;

        // PlayerSkillHitbox가 알아서 데미지 처리
        float t = 0f;
        while (t < friesWReach)
        {
            if (wSkill == null) break;

            float tt = t;
            t = Mathf.MoveTowards(t, friesWReach, friesWSpeed * Time.deltaTime);
            float move = t - tt;

            wSkill.transform.position += move * dir;
            yield return null;
        }

        if (wSkill != null) Object.Destroy(wSkill);
        c.isW = false;
    }

    private static IEnumerator FriesECoroutine(Player c)
    {
        c.isE = true;
        c.isFriesUpgraded = true;
        Debug.Log("[Fries E] 감자튀김 강화!");
        c.isE = false;
        yield return null;
    }

    // ===========================
    //  고기 스킬 구현
    // ===========================

    private static IEnumerator MeatWCoroutine(Player c)
    {
        c.isW = true;

        float meatWSpeed = 720f;
        float meatWAngle1 = 70f;
        float meatWAngle2 = -70f;
        float meatWWidth = 2f;
        float meatWHeight = 0.5f;

        var (dir, angleToMouse, skillPos) = MouseDirection(c);

        if (c.meatWPrefab == null)
        {
            c.isW = false;
            yield break;
        }

        GameObject wSkill = Object.Instantiate(c.meatWPrefab, skillPos, Quaternion.Euler(0, 0, angleToMouse + meatWAngle1));
        wSkill.transform.SetParent(c.transform);
        wSkill.transform.localScale = new Vector3(meatWWidth, meatWHeight, 1f);

        float meatWCurrentAngle = meatWAngle1;

        while (meatWCurrentAngle > meatWAngle2)
        {
            meatWCurrentAngle = Mathf.MoveTowardsAngle(meatWCurrentAngle, meatWAngle2, meatWSpeed * Time.deltaTime);
            wSkill.transform.rotation = Quaternion.Euler(0, 0, angleToMouse + meatWCurrentAngle);
            yield return null;
        }

        Object.Destroy(wSkill);
        c.isW = false;
    }

    private static IEnumerator MeatECoroutine(Player c)
    {
        c.isE = true;

        float meatESpeed = 10f;
        float meatEDistance = 3f;

        var (dir, _, _) = MouseDirection(c);
        float t = 0f;
        Vector3 startPos = c.transform.position;

        while (t < meatEDistance)
        {
            t = Mathf.MoveTowards(t, meatEDistance, meatESpeed * Time.deltaTime);

            float normalized = t / meatEDistance;
            float easing = 1f - Mathf.Pow(1f - normalized, 5f);
            c.transform.position = startPos + dir * (meatEDistance * easing);
            yield return null;
        }

        c.isE = false;
    }

    // ===========================
    //  버섯 스킬 구현
    // ===========================

    private static IEnumerator MushroomWCoroutine(Player c)
    {
        c.isW = true;

        float mushroomWDuration = 6f;
        float mushroomWRadius = 4.5f;

        var (dir, _, _) = MouseDirection(c);

        if (c.mushroomWPrefab == null)
        {
            c.isW = false;
            yield break;
        }

        GameObject wSkill = Object.Instantiate(c.mushroomWPrefab, c.transform.position + dir * 3f, Quaternion.identity);
        wSkill.transform.localScale = new Vector3(mushroomWRadius, mushroomWRadius, 1f);

        yield return new WaitForSeconds(mushroomWDuration);

        Object.Destroy(wSkill);
        c.isW = false;
    }

    private static IEnumerator MushroomECoroutine(Player c)
    {
        c.isE = true;

        float healDuration = 6f;       // 도트힐 지속시간
        float healInterval = 0.5f;     // 회복 간격 (0.5초마다)
        float healPerTick = 3f;        // 틱당 회복량

        c.StartMushroomHeal(MushroomHealOverTime(c, healDuration, healInterval, healPerTick));

        c.isE = false;
        yield return null;
    }

    private static IEnumerator MushroomHealOverTime(Player c, float duration, float interval, float healPerTick)
    {
        Debug.Log($"[Mushroom E] 도트힐 시작! {duration}초 동안 {interval}초마다 {healPerTick}씩 회복");

        float elapsed = 0f;
        float nextHealTime = interval;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            if (elapsed >= nextHealTime)
            {
                c.Hp = Mathf.Min(c.Hp + healPerTick, 100f); // 최대 체력 100 가정
                Debug.Log($"[Mushroom E] 체력 회복 +{healPerTick}, 현재 HP: {c.Hp}");
                nextHealTime += interval;
            }

            yield return null;
        }

        Debug.Log("[Mushroom E] 도트힐 종료");
    }

    // ===========================
    //  물 스킬 구현
    // ===========================

    private static IEnumerator WaterWCoroutine(Player c)
    {
        c.isW = true;

        float waterWSpeed = 10f;
        float waterWReach = 20f;
        float waterWRadius = 0.5f;
        float waterWGanGyeock = 0.1f;

        var (dir, _, skillPos) = MouseDirection(c);

        for (int i = 0; i < 3; i++)
        {
            c.StartCoroutine(WaterProjectile(c, skillPos, dir, waterWSpeed, waterWReach, waterWRadius));
            yield return new WaitForSeconds(waterWGanGyeock);
        }

        c.isW = false;
    }

    private static IEnumerator WaterProjectile(Player c, Vector3 skillPos, Vector3 dir, float speed, float reach, float radius)
    {
        if (c.waterWPrefab == null) yield break;

        GameObject wSkill = Object.Instantiate(c.waterWPrefab, skillPos, Quaternion.identity);
        wSkill.transform.localScale = new Vector3(radius, radius, 1f);

        // PlayerSkillHitbox가 알아서 데미지 처리
        float t = 0f;
        while (t < reach)
        {
            if (wSkill == null) break;

            float tt = t;
            t = Mathf.MoveTowards(t, reach, speed * Time.deltaTime);
            float move = t - tt;

            wSkill.transform.position += move * dir;
            yield return null;
        }

        if (wSkill != null) Object.Destroy(wSkill);
    }

    private static IEnumerator WaterECoroutine(Player c)
    {
        c.isE = true;

        float waterESpeed = 1f;
        float waterEStartScale = 0f;
        float waterEEndScale = 5f;

        if (c.waterEPrefab == null)
        {
            c.isE = false;
            yield break;
        }

        GameObject eSkill = Object.Instantiate(c.waterEPrefab, c.transform.position, Quaternion.identity);
        eSkill.transform.localScale = new Vector3(waterEStartScale, waterEStartScale, 1f);

        float t = 0f;
        while (t < 1f)
        {
            t = Mathf.MoveTowards(t, 1f, Time.deltaTime * waterESpeed);

            float easing = Mathf.Sqrt(1 - Mathf.Pow(t - 1f, 2));
            float scale = Mathf.Lerp(waterEStartScale, waterEEndScale, easing);

            eSkill.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        Object.Destroy(eSkill);
        c.isE = false;
    }
}