using UnityEngine;

public static class ElementCalculate
{
    /// <summary>
    /// 1 ← 2 ← 3 ← 1 상성 기준 데미지 배율 적용
    /// attackerElement == 0 이면 상성 무시(그냥 기본 데미지)
    /// defenderElement == 0 도 상성 없음으로 처리
    /// </summary>
    public static float ApplyElementModifier(
        float damage,
        int attackerElement,
        int defenderElement,
        float advantageMul = 1.5f,
        float disadvantageMul = 0.5f)
    {
        // 공격자 무속성(0) → 상성 없음
        if (attackerElement == 0)
            return damage;

        // 수비가 0이거나, 둘이 같은 속성이면 상성 없음
        if (defenderElement == 0 || attackerElement == defenderElement)
            return damage;

        // ✅ 상성 유리: attacker == (defender % 3) + 1
        if (attackerElement == (defenderElement % 3) + 1)
            return damage * advantageMul;

        // ✅ 상성 불리: defender == (attacker % 3) + 1
        if (defenderElement == (attackerElement % 3) + 1)
            return damage * disadvantageMul;

        // 혹시 이상한 값 들어왔으면 그냥 기본 데미지
        return damage;
    }
}
