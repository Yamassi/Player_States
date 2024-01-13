using UniRx;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    private int _idleHash;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private float _idleTime;

    public PlayerIdleState(IStateSwitcher stateSwitcher, PlayerInput playerInput, ControlInput controlInput,
        CharacterController characterController, Animator animator, Transform transform)
        : base(stateSwitcher, playerInput, controlInput, characterController, animator, transform)
    {
        _idleHash = Animator.StringToHash("Idle");
    }

    public override void Enter()
    {
        Debug.Log("Enter Idle State");
        animator.StopPlayback();
        animator.CrossFadeInFixedTime(_idleHash, 0.1f);
        _idleTime = 0;
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
            _idleTime += Time.deltaTime;

            if (characterController.isGrounded && controlInput.IsJumpPressed && !Player.requireNewJumpPress)
                stateSwitcher.SwitchState<PlayerJumpState>();
            else if (controlInput.CurrentMovementInput.x != 0
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