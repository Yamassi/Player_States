public class PlayerStateMachine
{
    public PlayerBaseState CurrentPlayerBaseState { get; set; }

    public void Init(PlayerBaseState startPlayerBaseState)
    {
        CurrentPlayerBaseState = startPlayerBaseState;
        CurrentPlayerBaseState.Enter();
    }

    public void ChangeState(PlayerBaseState newPlayerBaseState)
    {
        CurrentPlayerBaseState.Exit();
        CurrentPlayerBaseState = newPlayerBaseState;
        CurrentPlayerBaseState.Enter();
    }
}