using UniRx;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    private int _isIdleHash;
    private CompositeDisposable _disposable = new CompositeDisposable();

    public PlayerIdleState(IStateSwitcher stateSwitcher, PlayerInput playerInput, ControlInput controlInput,
        CharacterController characterController, Animator animator, Transform transform)
        : base(stateSwitcher, playerInput, controlInput, characterController, animator, transform)
    {
        _isIdleHash = Animator.StringToHash("Idle");
    }

    public override void Enter()
    {
        Debug.Log("Enter Idle State");
        animator.StopPlayback();
        animator.CrossFade(_isIdleHash, 0.1f);
        Idle();
    }

    public override void Exit()
    {
        Debug.Log("Exit Idle State");
        animator.StopPlayback();
        _disposable.Clear();
    }

    private void Idle()
    {
        Observable.EveryUpdate().Subscribe(_ =>
        {
            if (controlInput.CurrentMovementInput.x != 0
                || controlInput.CurrentMovementInput.y != 0
                && !controlInput.IsRunPressed)
                stateSwitcher.SwitchState<PlayerWalkState>();
            else if (controlInput.CurrentMovementInput.x != 0
                     || controlInput.CurrentMovementInput.y != 0
                     && controlInput.IsRunPressed)
                stateSwitcher.SwitchState<PlayerRunState>();
        }).AddTo(_disposable);
    }
}