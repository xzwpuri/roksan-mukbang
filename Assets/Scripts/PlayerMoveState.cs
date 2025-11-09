using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : State<Player>
{
    private const float arriveDistance = 0.05f;
    private System.Action<InputAction.CallbackContext> onClick;

    public override void Enter(Player owner)
    {
        //owner.Animator.SetTrigger("Move");
        // 이동 중 클릭하면 목표 지점 갱신
        onClick = (ctx) =>
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            owner.MoveTarget = new Vector2(mouseWorld.x, mouseWorld.y);
        };
        PlayerInputManager.instance.RMC.performed += onClick;
    }

    public override void Update(Player owner)
    {
        Vector2 currentPos = owner.transform.position;
        Vector2 targetPos = owner.MoveTarget;

        if ((targetPos - currentPos).sqrMagnitude <= arriveDistance * arriveDistance)
        {
            owner.Rigidbody2D.position = targetPos; // 스냅(잔떨림 방지)
            owner.Rigidbody2D.linearVelocityX = 0f;
            owner.Rigidbody2D.linearVelocityY = 0f;
            owner.StateMachine.ChangeState(owner.States[StateType.Idle]);
            return;
        }

        Vector2 dir = (targetPos - currentPos).normalized;
        float speed = owner.MoveSpeed; // mspeed 대신 프로퍼티 사용
        owner.Rigidbody2D.linearVelocityX = dir.x * speed;
        owner.Rigidbody2D.linearVelocityY = dir.y * speed;
    }

    public override void Exit(Player owner)
    {

        if (onClick != null)
            PlayerInputManager.instance.RMC.performed -= onClick;
        //owner.Animator.ResetTrigger("Move");
        owner.Rigidbody2D.linearVelocityX = 0f;
        owner.Rigidbody2D.linearVelocityY = 0f;
    }
}
