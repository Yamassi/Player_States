using UnityEngine;

public abstract class PlayerBaseState
{
    protected readonly IStateSwitcher stateSwitcher;
    protected readonly PlayerInput playerInput;
    protected readonly CharacterController characterController;
    protected readonly Animator animator;
    protected readonly Transform transform;
    protected readonly ControlInput controlInput;

    protected const float _walkSpeed = 2;
    protected const float _rotationSpeed = 8;
    protected const float _runSpeed = 5;

    protected Vector3 _currentMovement;
    protected Vector3 _appliedMovement;

    public PlayerBaseState(IStateSwitcher stateSwitcher, PlayerInput playerInput, ControlInput controlInput,
        CharacterController characterController, Animator animator, Transform transform)
    {
        this.stateSwitcher = stateSwitcher;
        this.playerInput = playerInput;
        this.characterController = characterController;
        this.animator = animator;
        this.transform = transform;
        this.controlInput = controlInput;
    }

    public abstract void Enter();

    public abstract void Exit();

    protected virtual void HandleGravity()
    {
        bool isFalling = _currentMovement.y <= 0f && !controlInput.IsJumpPressed;
        float fallMultiplier = 2.0f;

        if (characterController.isGrounded)
        {
            _currentMovement.y = Player.groundedGravity;
            _appliedMovement.y = Player.groundedGravity;
        }


        else if (isFalling)
        {
            float previousYVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + (Player.gravity * fallMultiplier * Time.deltaTime);
            _appliedMovement.y = Mathf.Max((previousYVelocity + _currentMovement.y) * 0.5f, -20);
        }
        else
        {
            float previousYVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + Player.gravity * Time.deltaTime;
            _appliedMovement.y = (previousYVelocity + _currentMovement.y) * 0.5f;
        }
    }

    protected virtual void HandleInput(float speed)
    {
        _currentMovement.x = controlInput.CurrentMovementInput.x * speed;
        _currentMovement.z = controlInput.CurrentMovementInput.y * speed;
        _currentMovement = _currentMovement.ToIso();
        _appliedMovement.x = _currentMovement.x;
        _appliedMovement.z = _currentMovement.z;
    }

    protected virtual void HandleRotation()
    {
        Vector3 positionToLookAt;

        if (_currentMovement.x == 0 && _currentMovement.z == 0)
        {
            _currentMovement.x = transform.forward.x;
            _currentMovement.z = transform.forward.z;
        }

        positionToLookAt.x = _currentMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = _currentMovement.z;

        var currentRotation = transform.rotation;

        var targetRotation = Quaternion.LookRotation(positionToLookAt);

        transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationSpeed * Time.deltaTime);
    }
}