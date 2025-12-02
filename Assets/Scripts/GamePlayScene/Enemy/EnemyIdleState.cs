using UnityEngine;

public class EnemyIdleState : State<EnemyBase>
{
    public override void Enter(EnemyBase owner)
    {
        if (owner.Animator != null)
            owner.Animator.SetTrigger("Idle");

        if (owner.Rigidbody2D != null)
            owner.Rigidbody2D.linearVelocity = Vector2.zero;
    }

    public override void Update(EnemyBase owner)
    {
        if (owner.Target == null)
            return;

        float dist = Vector2.Distance(owner.transform.position, owner.Target.position);

        // 추격 범위 안에 들어오면 Move로 전환
        if (dist <= owner.ChaseRange)
        {
            owner.StateMachine.ChangeState(owner.States[StateType.Move]);
        }
    }

    public override void Exit(EnemyBase owner)
    {
        if (owner.Animator != null)
            owner.Animator.ResetTrigger("Idle");
    }
}
