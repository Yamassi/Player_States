using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkState : PlayerBaseState
{
    private int _walkingHash;

    private CompositeDisposable _disposable = new CompositeDisposable();

    public PlayerWalkState(IStateSwitcher stateSwitcher, PlayerInput playerInput, ControlInput controlInput,
        CharacterController characterController, Animator animator, Transform transform)
        : base(stateSwitcher, playerInput, controlInput, characterController, animator, transform)
    {
        _walkingHash = Animator.StringToHash("Walk");
    }

    public override void Enter()
    {
        Debug.Log("Enter Walk State");

        animator.StopPlayback();
        animator.CrossFadeInFixedTime(_walkingHash, 0.1f);
        Move();
    }

    public override void Exit()
    {
        Debug.Log($"Exit Walk State");

        animator.StopPlayback();
        _disposable.Clear();
    }

    private void Move()
    {
        Observable.EveryUpdate().Subscribe(_ =>
        {
            HandleRotation();

            HandleInput(_walkSpeed);

            if (controlInput.CurrentMovementInput is { x: 0, y: 0 })
                stateSwitcher.SwitchState<PlayerIdleState>();
            if (controlInput.IsRunPressed)
                stateSwitcher.SwitchState<PlayerRunState>();
            if (characterController.isGrounded && controlInput.IsJumpPressed)
                stateSwitcher.SwitchState<PlayerJumpState>();

            characterController.Move(_appliedMovement * Time.deltaTime);

            HandleGravity();
        }).AddTo(_disposable);
    }
}