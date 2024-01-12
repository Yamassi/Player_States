using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkState : PlayerBaseState
{
    private int _isWalkingHash;
    private const float _walkSpeed = 2;
    private const float _rotationSpeed = 8;

    protected Vector3 _currentMovement;
    protected Vector3 _appliedMovement;

    private CompositeDisposable _disposable = new CompositeDisposable();

    public PlayerWalkState(IStateSwitcher stateSwitcher, PlayerInput playerInput, ControlInput controlInput,
        CharacterController characterController, Animator animator, Transform transform)
        : base(stateSwitcher, playerInput, controlInput, characterController, animator, transform)
    {
        _isWalkingHash = Animator.StringToHash("Walk");
    }

    public override void Enter()
    {
        Debug.Log("Enter Walk State");

        animator.StopPlayback();
        animator.CrossFade(_isWalkingHash, 0.1f);
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

            HandleInput();

            if (controlInput.CurrentMovementInput is { x: 0, y: 0 })
                stateSwitcher.SwitchState<PlayerIdleState>();
            if (controlInput.IsRunPressed)
                stateSwitcher.SwitchState<PlayerRunState>();

            characterController.Move(_appliedMovement * Time.deltaTime);

            HandleGravity();
        }).AddTo(_disposable);
    }

    private void HandleInput()
    {
        _currentMovement.x = controlInput.CurrentMovementInput.x * _walkSpeed;
        _currentMovement.z = controlInput.CurrentMovementInput.y * _walkSpeed;
        _currentMovement = _currentMovement.ToIso();
        _appliedMovement.x = _currentMovement.x;
        _appliedMovement.z = _currentMovement.z;
    }

    private void HandleGravity()
    {
        bool isFalling = _currentMovement.y <= 0f;
        float fallMultiplier = 2.0f;

        if (characterController.isGrounded)
            _currentMovement.y = Player.groundedGravity;
        else if (isFalling)
        {
            float previousYVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + (Player.gravity * fallMultiplier * Time.deltaTime);
            _appliedMovement.y = Mathf.Max((previousYVelocity + _currentMovement.y) * 0.5f, -20);
        }
    }

    private void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _currentMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = _currentMovement.z;

        var currentRotation = transform.rotation;

        var targetRotation = Quaternion.LookRotation(positionToLookAt);

        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }
}