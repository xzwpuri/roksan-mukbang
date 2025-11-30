using UnityEngine;

public class EnemyAttackState : State<EnemyBase>
{
    private float skill1Timer;
    private float skill2Timer;

    public override void Enter(EnemyBase owner)
    {
        skill1Timer = 0f;
        skill2Timer = 0f;

        if (owner.Animator != null)
            owner.Animator.SetTrigger("Idle");
    }

    public override void Update(EnemyBase owner)
    {
        if (owner.Target == null)
        {
            owner.StateMachine.ChangeState(owner.States[StateType.Idle]);
            return;
        }

        float dist = Vector2.Distance(owner.transform.position, owner.Target.position);

        // ✅ 공격 거리(AttackRange)를 벗어나면 다시 Move로
        if (dist > owner.AttackRange)
        {
            owner.StateMachine.ChangeState(owner.States[StateType.Move]);
            return;
        }

        // 쿨타임 타이머 감소
        skill1Timer -= Time.deltaTime;
        skill2Timer -= Time.deltaTime;

        bool skill1Ready = skill1Timer <= 0f;
        bool skill2Ready = skill2Timer <= 0f;

        // 아무 스킬도 준비 안 됐으면 대기
        if (!skill1Ready && !skill2Ready)
            return;

        // ✅ 스킬 선택 로직 (여기 패턴은 너 마음대로 바꿔도 됨)

        // 둘 다 준비됨 → 랜덤 선택 (예시)
        if (skill1Ready && skill2Ready)
        {
            if (Random.value < 0.5f)
            {
                owner.Skill1();
                skill1Timer = owner.Skill1Cooldown;
            }
            else
            {
                owner.Skill2();
                skill2Timer = owner.Skill2Cooldown;
            }
        }
        else if (skill1Ready) // 스킬1만 준비됨
        {
            owner.Skill1();
            skill1Timer = owner.Skill1Cooldown;
        }
        else if (skill2Ready) // 스킬2만 준비됨
        {
            owner.Skill2();
            skill2Timer = owner.Skill2Cooldown;
        }

        // 공격 중에도 플레이어가 거리를 벌리면 Move로 나가고 싶다면
        // 위에서 이미 dist > AttackRange 체크 했으니까 여기선 굳이 안 해도 됨
    }

    public override void Exit(EnemyBase owner)
    {
        if (owner.Animator != null)
            owner.Animator.ResetTrigger("Idle");
    }
}
