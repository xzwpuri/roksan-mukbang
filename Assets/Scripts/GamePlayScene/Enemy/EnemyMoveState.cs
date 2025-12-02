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

    float maxSkillRange = Mathf.Max(owner.Skill1Range, owner.Skill2Range);

    //Debug.Log($"[MoveState] dist={dist:F2}, runDist={owner.RunningDistance}, chaseRange={owner.ChaseRange}");

    // 1) 너무 멀어지면 Idle
    if (dist > (owner.ChaseRange + 1000000) * 1.5f)
    {
        Debug.Log("[MoveState] → Idle (dist > ChaseRange * 1.5)");
        if (owner.Rigidbody2D != null)
            owner.Rigidbody2D.linearVelocity = Vector2.zero;

        owner.StateMachine.ChangeState(owner.States[StateType.Idle]);
        return;
    }

    // 2) 스킬 닿으면 Attack
    if (dist <= maxSkillRange)
    {
        //Debug.Log("[MoveState] → Attack (dist <= maxSkillRange)");
        if (owner.Rigidbody2D != null)
            owner.Rigidbody2D.linearVelocity = Vector2.zero;

        owner.StateMachine.ChangeState(owner.States[StateType.Attack]);
        return;
    }

    // 3) MOVE + 속도 조정
    Vector2 dir = (targetPos - pos).normalized;
    float moveSpeed = owner.MoveSpeed;

    if (dist > owner.RunningDistance)
    {
        moveSpeed *= 5f;
        //Debug.Log($"[MoveState] RUN! speed x3, finalSpeed={moveSpeed}");
    }
    else
    {
        //Debug.Log($"[MoveState] WALK speed={moveSpeed}");
    }

    if (owner.Rigidbody2D != null)
        owner.Rigidbody2D.linearVelocity = dir * moveSpeed;
}


    public override void Exit(EnemyBase owner)
    {
        if (owner.Animator != null)
            owner.Animator.ResetTrigger("Idle");
    }
}