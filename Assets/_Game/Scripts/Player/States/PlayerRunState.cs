using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunState : PlayerBaseState
{
    private int _runHash;

    private CompositeDisposable _disposable = new CompositeDisposable();

    public PlayerRunState(IStateSwitcher stateSwitcher, ControlInput controlInput,
        CharacterController characterController, Animator animator, Transform transform)
        : base(stateSwitcher, controlInput, characterController, animator, transform)
    {
        _runHash = Animator.StringToHash("Run");
    }

    public override void Enter()
    {
        Debug.Log($"Enter Run State");

        _disposable = new CompositeDisposable();

        animator.StopPlayback();
        animator.CrossFadeInFixedTime(_runHash, 0.1f);
        Move();
    }

    public override void Exit()
    {
        Debug.Log("Exit Run State");

        animator.StopPlayback();
        _disposable.Clear();
    }

    private void Move()
    {
        Observable.EveryUpdate().Subscribe(_ =>
        {
            HandleRotation();

            HandleInput(_runSpeed);


            if (characterController.isGrounded && controlInput.IsJumpPressed && !Player.isRequireNewJumpPress)
                stateSwitcher.SwitchState<PlayerJumpState>();
            else if (controlInput.IsAttackPressed && !Player.isRequiredNewAttackPress)
                stateSwitcher.SwitchState<PlayerAttackState>();
            else if (!controlInput.IsRunPressed)
                stateSwitcher.SwitchState<PlayerWalkState>();
            else if (controlInput.CurrentMovementInput is { x: 0, y: 0 })
                stateSwitcher.SwitchState<PlayerIdleState>();

            characterController.Move(_appliedMovement * Time.deltaTime);

            HandleGravity();
        }).AddTo(_disposable);
    }
}