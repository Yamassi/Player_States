using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IStateSwitcher
{
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private Animator _animator;
    private Weapon _weapon;

    private StateMachine _stateMachine;
    private List<PlayerBaseState> _allStates;
    private ControlInput _controlInput;

    public static float groundedGravity = -0.05f;
    public static float gravity = -9.8f;
    public static float maxJumpHeight = 2;
    public static float maxJumpTime = 0.75f;
    public static bool isRequireNewJumpPress = false;
    public static bool isRequiredNewAttackPress = false;
    public static bool isUnarmed = true;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _weapon = GetComponent<Weapon>();

        _playerInput = new PlayerInput();
        _controlInput = new();

        _stateMachine = new();

        _allStates = new()
        {
            new PlayerIdleState(this, _controlInput,
                _characterController, _animator, transform),

            new PlayerWalkState(this, _controlInput,
                _characterController, _animator, transform),

            new PlayerRunState(this, _controlInput,
                _characterController, _animator, transform),

            new PlayerJumpState(this, _controlInput,
                _characterController, _animator, transform),

            new PlayerAttackState(this, _controlInput,
                _characterController, _weapon, _animator, transform),
        };

        _stateMachine.Init(_allStates[0]);

        _playerInput.CharacterControls.Move.started += OnMove;
        _playerInput.CharacterControls.Move.performed += OnMove;
        _playerInput.CharacterControls.Move.canceled += OnMove;
        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;
        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;
        _playerInput.CharacterControls.Attack.started += OnAttack;
        _playerInput.CharacterControls.Attack.canceled += OnAttack;
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        _controlInput.IsAttackPressed = context.ReadValueAsButton();
        isRequiredNewAttackPress = false;
    }


    private void OnJump(InputAction.CallbackContext context)
    {
        _controlInput.IsJumpPressed = context.ReadValueAsButton();
        isRequireNewJumpPress = false;
    }

    private void OnRun(InputAction.CallbackContext context)
        => _controlInput.IsRunPressed = context.ReadValueAsButton();

    private void OnMove(InputAction.CallbackContext context)
        => _controlInput.CurrentMovementInput = context.ReadValue<Vector2>();

    public void SwitchState<T>() where T : BaseState
    {
        var state = _allStates.FirstOrDefault(s => s is T);
        _stateMachine.ChangeState(state);
    }

    private void OnEnable()
    {
        _playerInput.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Disable();
    }

    private void OnDestroy()
    {
        _playerInput.CharacterControls.Move.started -= OnMove;
        _playerInput.CharacterControls.Move.performed -= OnMove;
        _playerInput.CharacterControls.Move.canceled -= OnMove;
        _playerInput.CharacterControls.Run.started -= OnRun;
        _playerInput.CharacterControls.Run.canceled -= OnRun;
        _playerInput.CharacterControls.Jump.started -= OnJump;
        _playerInput.CharacterControls.Jump.canceled -= OnJump;
    }
}

public class ControlInput
{
    public Vector2 CurrentMovementInput;
    public bool IsJumpPressed;
    public bool IsRunPressed;
    public bool IsAttackPressed;
}