public abstract class State<T> where T : class
{
    //상태 진입
    public abstract void Enter(T owner);
    //상태 중
    public abstract void Update(T owner);
    //상태 종료
    public abstract void Exit(T owner);
}