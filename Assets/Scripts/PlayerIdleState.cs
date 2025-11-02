using UnityEngine;

public class PlayerIdleState : State<Player>
{
    public override void Enter(Player owner)
    {
        //owner.Animator.SetTrigger("Idle");
    }

    public override void Update(Player owner)
    {
        if (PlayerInputManager.instance.RMC.IsPressed())
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            owner.MoveTarget = new Vector2(mouseWorld.x, mouseWorld.y);
            owner.StateMachine.ChangeState(owner.States[StateType.Move]);
        }
    }

    public override void Exit(Player owner)
    {
        //owner.Animator.ResetTrigger("Idle");
    }
}