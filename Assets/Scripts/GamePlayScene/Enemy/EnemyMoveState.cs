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

        Vector2 pos = owner.transform.position;
        Vector2 targetPos = owner.Target.position;
        float dist = Vector2.Distance(pos, targetPos);

        float maxSkillRange = Mathf.Max(owner.Skill1Range, owner.Skill2Range);

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
            if (owner.Rigidbody2D != null)
                owner.Rigidbody2D.linearVelocity = Vector2.zero;

            owner.StateMachine.ChangeState(owner.States[StateType.Attack]);
            return;
        }

        // 3) MOVE + 속도 조정
        Vector2 dir = (targetPos - pos).normalized;

        // 🔥 한 번이라도 RunningDistance 밖으로 나가면, 그 순간에만 5배로 올리고 고정
        if (!owner.HasRunBoost && dist > owner.RunningDistance)
        {
            owner.HasRunBoost = true;
            owner.MoveSpeed *= 5f;          // ← 여기서 실제 스탯을 올려버림
            //Debug.Log($"[MoveState] RunBoost ON, new MoveSpeed={owner.MoveSpeed}");
        }

        float moveSpeed = owner.MoveSpeed;   // 이후엔 그냥 현재 MoveSpeed 사용

        if (owner.Rigidbody2D != null)
            owner.Rigidbody2D.linearVelocity = dir * moveSpeed;
    }

    public override void Exit(EnemyBase owner)
    {
        if (owner.Animator != null)
            owner.Animator.ResetTrigger("Idle");
    }
}
