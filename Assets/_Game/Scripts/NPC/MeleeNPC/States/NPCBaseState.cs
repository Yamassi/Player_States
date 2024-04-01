using UnityEngine;

public abstract class NPCBaseState : BaseState
{
    protected readonly IStateSwitcher stateSwitcher;
    protected readonly CharacterController characterController;
    protected readonly Animator animator;
    protected readonly NPCSensor sensor;
    protected readonly Transform transform;


    public NPCBaseState(IStateSwitcher stateSwitcher,
        CharacterController characterController, Animator animator, NPCSensor sensor, Transform transform)
    {
        this.stateSwitcher = stateSwitcher;
        this.characterController = characterController;
        this.animator = animator;
        this.sensor = sensor;
        this.transform = transform;
    }
}