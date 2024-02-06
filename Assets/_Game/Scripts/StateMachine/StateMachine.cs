public class StateMachine
{
    public BaseState CurrentBaseState { get; set; }

    public void Init(BaseState startBaseState)
    {
        CurrentBaseState = startBaseState;
        CurrentBaseState.Enter();
    }

    public void ChangeState(BaseState newBaseState)
    {
        CurrentBaseState.Exit();
        CurrentBaseState = newBaseState;
        CurrentBaseState.Enter();
    }
}