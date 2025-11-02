using UnityEngine.XR;

public class StateMachine<T> where T : class
{
    //소유자
    private T owner;
    //현재 상태
    private State<T> currentState;
    //이전 상태
    private State<T> previousState;

    public void Setup(T _onwer, State<T> state)
    {
        this.owner = _onwer;
        currentState = null;
        previousState = null;

        ChangeState(state);
    }

    public void ChangeState(State<T> newState)
    {
        if (newState == null) return;
        if (currentState != null)
        {
            previousState = currentState;
            currentState.Exit(this.owner);
        }

        currentState = newState;
        currentState.Enter(this.owner);
    }

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update(this.owner);
        }
    }

    public State<T> GetCurrentState()
    {
        return currentState;
    }
}