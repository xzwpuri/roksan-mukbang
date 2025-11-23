using UnityEngine;

public class EnemyMoveState : State<EnemyBase>
{
    public override void Enter(EnemyBase owner)
    {
        if (owner.Animator != null)
            owner.Animator.SetTrigger("Move");
    }

    public override void Update(EnemyBase owner)
    {
        if (owner.Target == null)
            return;

        Vector2 pos = owner.transform.position;
        Vector2 targetPos = owner.Target.position;
        float dist = Vector2.Distance(pos, targetPos);

        // 일정 거리 이내로 들어오면 이동 멈추고 공격 or Idle
        if (dist <= owner.StopDistance)
        {
            if (owner.Rigidbody2D != null)
                owner.Rigidbody2D.linearVelocity = Vector2.zero;

            if (dist <= owner.AttackRange)
                owner.StateMachine.ChangeState(owner.States[StateType.Attack]);
            else
                owner.StateMachine.ChangeState(owner.States[StateType.Idle]);

            return;
        }

        // 플레이어 방향으로 이동
        Vector2 dir = (targetPos - pos).normalized;
        if (owner.Rigidbody2D != null)
            owner.Rigidbody2D.linearVelocity = dir * owner.MoveSpeed;

        // 너무 멀어지면 다시 Idle로 복귀
        if (dist > owner.ChaseRange * 1.5f)
        {
            owner.StateMachine.ChangeState(owner.States[StateType.Idle]);
        }
    }

    public override void Exit(EnemyBase owner)
    {
        if (owner.Animator != null)
            owner.Animator.ResetTrigger("Move");
    }
}
