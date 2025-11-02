using UnityEngine;

public class PlayerMoveState : State<Player>
{

    private const float arriveDistance = 0.05f;
    public override void Enter(Player owner)
    {
        //owner.Animator.SetTrigger("Move");
    }

    public override void Update(Player owner)
    {
        Vector2 currentPos = owner.transform.position;
        Vector2 targetPos = owner.MoveTarget;

        float dist = Vector2.Distance(currentPos, targetPos);
        if (dist < arriveDistance)
        {
            owner.Rigidbody2D.linearVelocityX = 0f;
            owner.Rigidbody2D.linearVelocityY = 0f;
            owner.StateMachine.ChangeState(owner.States[StateType.Idle]);
            return;
        }
        Vector2 dir = (targetPos - currentPos).normalized;
        owner.Rigidbody2D.linearVelocityX = dir.x * owner.mspeed;
    }

    public override void Exit(Player owner)
    {
        //owner.Animator.ResetTrigger("Move");
        owner.Rigidbody2D.linearVelocityX = 0f;
        owner.Rigidbody2D.linearVelocityY = 0f;
    }
}
