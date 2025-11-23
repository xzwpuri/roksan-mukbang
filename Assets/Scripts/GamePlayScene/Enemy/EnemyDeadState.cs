using UnityEngine;

public class EnemyDeadState : State<EnemyBase>
{
    private float removeDelay = 1.0f;
    private float timer;

    public override void Enter(EnemyBase owner)
    {
        if (owner.Animator != null)
            owner.Animator.SetTrigger("Dead");

        owner.Rigidbody2D.linearVelocity = Vector2.zero;
        owner.Rigidbody2D.simulated = false;

        owner.gameObject.tag = "Dead";
        timer = removeDelay;
    }

    public override void Update(EnemyBase owner)
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            GameObject.Destroy(owner.gameObject);
        }
    }

    public override void Exit(EnemyBase owner)
    {
    }
}
