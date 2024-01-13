using UniRx;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private int _jumpHash;
    private CompositeDisposable _disposable = new CompositeDisposable();

    private float _initialJumpVelocity;

    public PlayerJumpState(IStateSwitcher stateSwitcher, PlayerInput playerInput, ControlInput controlInput,
        CharacterController characterController, Animator animator, Transform transform)
        : base(stateSwitcher, playerInput, controlInput, characterController, animator, transform)
    {
        _jumpHash = Animator.StringToHash("Jump");
        SetupJumpVariables();
    }

    public override void Enter()
    {
        Debug.Log("Enter Jump State");
        animator.StopPlayback();
        animator.CrossFadeInFixedTime(_jumpHash, 0.1f);
        
        Jump();
    }

    public override void Exit()
    {
        animator.StopPlayback();
        _disposable.Clear();
    }

    private void Jump()
    {
        Observable.EveryUpdate().Subscribe(_ =>
        {
            HandleRotation();

            if (!controlInput.IsRunPressed)
                HandleInput(_walkSpeed);
            if (controlInput.IsRunPressed)
                HandleInput(_runSpeed);
            
            if (characterController.isGrounded && !controlInput.IsJumpPressed)
            {
                if (controlInput.CurrentMovementInput is { x: 0, y: 0 })
                    stateSwitcher.SwitchState<PlayerIdleState>();

                if (!controlInput.IsRunPressed && controlInput.CurrentMovementInput.x != 0
                    || controlInput.CurrentMovementInput.y != 0)
                    stateSwitcher.SwitchState<PlayerWalkState>();

                if (controlInput.IsRunPressed && controlInput.CurrentMovementInput.x != 0
                    || controlInput.CurrentMovementInput.y != 0)
                    stateSwitcher.SwitchState<PlayerRunState>();
            }

            characterController.Move(_appliedMovement * Time.deltaTime);

            HandleGravity();
            HandleJump();
        }).AddTo(_disposable);
    }

    private void HandleJump()
    {
        if (characterController.isGrounded && controlInput.IsJumpPressed)
        {
            _currentMovement.y = _initialJumpVelocity;
            _appliedMovement.y = _initialJumpVelocity;
        }
    }

    private void SetupJumpVariables()
    {
        var timeToApex = Player.maxJumpTime / 2;
        Player.gravity = (-2 * Player.maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * Player.maxJumpHeight) / timeToApex;
    }
}