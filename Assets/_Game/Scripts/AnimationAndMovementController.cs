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
   
   private Vector2 _currentMovementInput;
   private Vector3 _currentMovement;
   private Vector3 _currentRunMovement;
   private bool _isMovementPressed, _isRunPressed;
   private float _walkSpeed = 2;
   private float _rotationSpeed = 8;
   private float _runSpeed = 5;
    private void Awake()
    {
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();

        _isWalkingHash = Animator.StringToHash("IsWalking");
        _isRunningHash = Animator.StringToHash("IsRunning");
        
        _playerInput.CharacterControls.Move.started += OnMovementInput;
        _playerInput.CharacterControls.Move.canceled += OnMovementInput;
        _playerInput.CharacterControls.Move.performed += OnMovementInput;
        _playerInput.CharacterControls.Run.started += OnRunInput;
        _playerInput.CharacterControls.Run.canceled += OnRunInput;
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
    }

    private void OnRunInput(InputAction.CallbackContext context) => _isRunPressed = context.ReadValueAsButton();
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

    private void HandleGravity()
    {
        if (_characterController.isGrounded)
        {
            float groundedGravity = -0.05f;
            _currentMovement.y = groundedGravity;
            _currentRunMovement.y = groundedGravity;
        }
        else
        {
            float gravity = -9.8f;
            _currentMovement.y += gravity;
            _currentRunMovement.y += gravity;
        }
    }
    private void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _currentMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = _currentMovement.z;

        Quaternion currentRotation = transform.rotation;
        
        if (_isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);

            transform.rotation = Quaternion.Slerp(currentRotation,targetRotation,_rotationSpeed * Time.deltaTime);
        }
        
    }
    private void HandleAnimation()
    {
        bool isWalking = _animator.GetBool(_isWalkingHash);
        bool isRunning =_animator.GetBool(_isRunningHash);
        
        if(_isMovementPressed && !isWalking)
            _animator.SetBool(_isWalkingHash, true);
       else if(!_isMovementPressed && isWalking)
            _animator.SetBool(_isWalkingHash, false);
     
       if(_isMovementPressed && _isRunPressed && !isRunning) 
            _animator.SetBool(_isRunningHash, true);
       else if(_isMovementPressed && !_isRunPressed && isRunning)
           _animator.SetBool(_isRunningHash, false);
       else if(!_isMovementPressed && !_isRunPressed && isRunning)
            _animator.SetBool(_isRunningHash, false);
    }
    private void Update()
    {
        HandleGravity();
        HandleRotation();
        HandleAnimation();
        
        if(_isRunPressed) 
            _characterController.Move(_currentRunMovement * Time.deltaTime);
        else 
            _characterController.Move(_currentMovement * Time.deltaTime);
        
        
    }
}
