// SkillLibrary.cs (신규)
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

    //일단은 속성으로 스킬 바뀌게 해뒀는데 추후에 몬스터 어떤 거 먹었는지로 바꿔야 함.

    // W/E: element == 0 
    public static void W_Default(Player c)
    {
        Debug.Log("[W] Default");
    }
    public static void E_Default(Player c)
    {
        Debug.Log("[E] Default");
    }
    // W/E: element == 1 (예: Fire)
    public static void W_Fire(Player c)
    {
        Debug.Log("[W] Fire");
    }
    public static void E_Fire(Player c)
    {
        Debug.Log("[E] Fire");
    }
    // W/E: element == 2 (예: Water)
    public static void W_Water(Player c)
    {
        Debug.Log("[W] Water");
    }
    public static void E_Water(Player c)
    {
        Debug.Log("[E] Water");
    }

    // W/E: element == 3 (예: Grass)
    public static void W_Grass(Player c)
    {
        Debug.Log("[W] Grass");
    }
    public static void E_Grass(Player c)
    {
        Debug.Log("[E] Grass");
    }
    // 필요 시 계속 추가…
}
