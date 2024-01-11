using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private Animator _animator;

    private int _isWalkingHash;
    private int _isRunningHash;
    private int _isJumpingHash;

    private Vector2 _currentMovementInput;
    private Vector3 _currentMovement;
    private Vector3 _currentRunMovement;
    private Vector3 _appliedMovement;

    private bool _isMovementPressed, _isRunPressed;
    private float _walkSpeed = 2;
    private float _rotationSpeed = 8;
    private float _runSpeed = 5;

    private float _groundedGravity = -0.05f;
    private float _gravity = -9.8f;

    private bool _isJumpPressed = false;
    private float _initialJumpVelocity;
    private float _maxJumpHeight = 2;
    private float _maxJumpTime = 0.75f;
    private bool _isJumping = false;
    private bool _isJumpAnimation = false;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        _isWalkingHash = Animator.StringToHash("IsWalking");
        _isRunningHash = Animator.StringToHash("IsRunning");
        _isJumpingHash = Animator.StringToHash("IsJumping");

        _playerInput.CharacterControls.Move.started += OnMovementInput;
        _playerInput.CharacterControls.Move.canceled += OnMovementInput;
        _playerInput.CharacterControls.Move.performed += OnMovementInput;
        _playerInput.CharacterControls.Run.started += OnRunInput;
        _playerInput.CharacterControls.Run.canceled += OnRunInput;
        _playerInput.CharacterControls.Jump.started += OnJumpInput;
        _playerInput.CharacterControls.Jump.canceled += OnJumpInput;

        SetupJumpVariables();
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
        _playerInput.CharacterControls.Move.started -= OnMovementInput;
        _playerInput.CharacterControls.Move.canceled -= OnMovementInput;
        _playerInput.CharacterControls.Move.performed -= OnMovementInput;
        _playerInput.CharacterControls.Run.started -= OnRunInput;
        _playerInput.CharacterControls.Run.canceled -= OnRunInput;
        _playerInput.CharacterControls.Jump.started -= OnJumpInput;
        _playerInput.CharacterControls.Jump.canceled -= OnJumpInput;
    }

    private void SetupJumpVariables()
    {
        var timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    }

    private void OnRunInput(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovement.x = _currentMovementInput.x * _walkSpeed;
        _currentMovement.z = _currentMovementInput.y * _walkSpeed;
        _currentMovement = _currentMovement.ToIso();

        _currentRunMovement.x = _currentMovementInput.x * _runSpeed;
        _currentRunMovement.z = _currentMovementInput.y * _runSpeed;
        _currentRunMovement = _currentRunMovement.ToIso();

        _isMovementPressed = _currentMovement.x != 0 || _currentMovement.z != 0;
    }

    private void HandleJump()
    {
        if (!_isJumping && _characterController.isGrounded && _isJumpPressed)
        {
            _animator.SetBool(_isJumpingHash, true);
            _isJumpAnimation = true;
            _isJumping = true;
            _currentMovement.y = _initialJumpVelocity;
            _appliedMovement.y = _initialJumpVelocity;
        }
        else if (!_isJumpPressed && _isJumping && _characterController.isGrounded)
        {
            _isJumping = false;
        }
    }

    private void HandleGravity()
    {
        bool isFalling = _currentMovement.y <= 0f || !_isJumpPressed;
        float fallMultiplier = 2.0f;

        if (_characterController.isGrounded)
        {
            if (_isJumpAnimation)
            {
                _animator.SetBool(_isJumpingHash, false);
                _isJumpAnimation = false;
            }

            _currentMovement.y = _groundedGravity;
            _currentRunMovement.y = _groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + (_gravity * fallMultiplier * Time.deltaTime);
            _appliedMovement.y = Mathf.Max((previousYVelocity + _currentMovement.y) * 0.5f, -20);
        }

        else
        {
            float previousYVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + _gravity * Time.deltaTime;
            _appliedMovement.y = (previousYVelocity + _currentMovement.y) * 0.5f;
        }
    }

    private void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _currentMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = _currentMovement.z;

        var currentRotation = transform.rotation;

        if (_isMovementPressed)
        {
            var targetRotation = Quaternion.LookRotation(positionToLookAt);

            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleAnimation()
    {
        var isWalking = _animator.GetBool(_isWalkingHash);
        var isRunning = _animator.GetBool(_isRunningHash);

        if (_isMovementPressed && !isWalking)
            _animator.SetBool(_isWalkingHash, true);
        else if (!_isMovementPressed && isWalking)
            _animator.SetBool(_isWalkingHash, false);

        if (_isMovementPressed && _isRunPressed && !isRunning)
            _animator.SetBool(_isRunningHash, true);
        else if (_isMovementPressed && !_isRunPressed && isRunning)
            _animator.SetBool(_isRunningHash, false);
        else if (!_isMovementPressed && !_isRunPressed && isRunning)
            _animator.SetBool(_isRunningHash, false);
    }

    private void Update()
    {
        HandleRotation();
        HandleAnimation();

        if (_isRunPressed)
        {
            _appliedMovement.x = _currentRunMovement.x;
            _appliedMovement.z = _currentRunMovement.z;
        }
        else
        {
            _appliedMovement.x = _currentMovement.x;
            _appliedMovement.z = _currentMovement.z;
        }

        _characterController.Move(_appliedMovement * Time.deltaTime);

        HandleGravity();
        HandleJump();
    }
}