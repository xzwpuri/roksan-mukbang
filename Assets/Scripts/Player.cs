using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    Idle,
    Move
}

public class Player : Unit
{
    public Dictionary<StateType, State<Player>> States;
    public StateMachine<Player> StateMachine;

    [HideInInspector]
    public Rigidbody2D Rigidbody2D;
    [HideInInspector]
    public Animator Animator;
    [HideInInspector]
    public SpriteRenderer SpriteRenderer;
    [HideInInspector]
    public Vector2 MoveTarget;
    [HideInInspector]
    public float mspeed;

    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
       //Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Init(100f, 5f);
        mspeed = this.movespeed;
        States = new Dictionary<StateType, State<Player>>();
        StateMachine = new StateMachine<Player>();

        States.Add(StateType.Idle, new PlayerIdleState());
        States.Add(StateType.Move, new PlayerMoveState());

        StateMachine.Setup(this, States[StateType.Idle]);
    }

    private void Update()
    {
        StateMachine.Update();
        CheckFlipX();
    }

    private void CheckFlipX()
    {
        if (Rigidbody2D.linearVelocityX > 0.01f) SpriteRenderer.flipX = false;
        else if (Rigidbody2D.linearVelocityX < -0.01f) SpriteRenderer.flipX = true;
    }
}