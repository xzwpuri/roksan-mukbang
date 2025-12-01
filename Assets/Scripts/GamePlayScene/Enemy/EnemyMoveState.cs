using UnityEngine;

public class EnemyMoveState : State<EnemyBase>
{
    public override void Enter(EnemyBase owner)
    {
        if (owner.Animator != null)
            owner.Animator.SetTrigger("Idle");
    }

    public override void Update(EnemyBase owner)
    {
        if (owner.Target == null)
            return;

        Vector2 pos       = owner.transform.position;
        Vector2 targetPos = owner.Target.position;
        float dist        = Vector2.Distance(pos, targetPos);

        // 너무 멀어지면 아예 Idle로 (추격 포기)
        if (dist > owner.ChaseRange * 1.5f)
        {
            if (owner.Rigidbody2D != null)
                owner.Rigidbody2D.linearVelocity = Vector2.zero;

            owner.StateMachine.ChangeState(owner.States[StateType.Idle]);
            return;
        }

        float maxSkillRange = Mathf.Max(owner.Skill1Range, owner.Skill2Range);

        // 스킬 중 하나라도 닿는 거리면 공격 상태로 전환
        if (dist <= maxSkillRange)
        {
            if (owner.Rigidbody2D != null)
                owner.Rigidbody2D.linearVelocity = Vector2.zero;

            owner.StateMachine.ChangeState(owner.States[StateType.Attack]);
            return;
        }

        // 그 외: 스킬 안 닿을 정도로 멀다 → stopDistance까지 접근
        Vector2 dir = (targetPos - pos).normalized;

        if (owner.Rigidbody2D != null)
            owner.Rigidbody2D.linearVelocity = dir * owner.MoveSpeed;
    }

    public override void Exit(EnemyBase owner)
    {
        if (owner.Animator != null)
            owner.Animator.ResetTrigger("Idle");
    }
}