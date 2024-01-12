using UnityEngine;

public abstract class PlayerBaseState
{
    protected readonly IStateSwitcher stateSwitcher;
    protected readonly PlayerInput playerInput;
    protected readonly CharacterController characterController;
    protected readonly Animator animator;
    protected readonly Transform transform;
    protected readonly ControlInput controlInput;

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
}