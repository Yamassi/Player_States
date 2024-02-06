using UnityEngine;

public abstract class NPCBaseState : BaseState
{
    protected readonly IStateSwitcher stateSwitcher;
    protected readonly CharacterController characterController;
    protected readonly Animator animator;
    protected readonly Transform transform;


    public NPCBaseState(IStateSwitcher stateSwitcher,
        CharacterController characterController, Animator animator, Transform transform)
    {
        this.stateSwitcher = stateSwitcher;
        this.characterController = characterController;
        this.animator = animator;
        this.transform = transform;
    }
}