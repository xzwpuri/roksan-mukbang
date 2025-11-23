using UnityEngine;

public class EnemyWater : EnemyBase
{
    //[Header("Water 전용 공격 옵션")]

    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();             // FSM 셋업
        Init(50f, 2.5f, 2, 0);    // EnemyWater 기본값
    }
    public override void Skill1()
    {
        Debug.Log("Water Skill1 사용!");
    }

    public override void Skill2()
    {
        Debug.Log("Water Skill2 사용!");
    }
}
