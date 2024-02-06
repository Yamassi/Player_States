using UniRx;
using UnityEngine;

public class MeleeNPCTakeHitState : NPCBaseState
{
    private int _takeHitHash;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private NPCSensor _sensor;

    public MeleeNPCTakeHitState(IStateSwitcher stateSwitcher,
        CharacterController characterController, NPCSensor sensor, Animator animator, Transform transform)
        : base(stateSwitcher, characterController, animator, transform)
    {
        _sensor = sensor;
        _takeHitHash = Animator.StringToHash("TakeHit");
    }

    public override void Enter()
    {
        animator.StopPlayback();
        animator.CrossFadeInFixedTime(_takeHitHash, 0.1f);
        TakeHit();
    }

    public override void Exit()
    {
        animator.StopPlayback();
        _disposable.Clear();
    }

    private void TakeHit()
    {
        Observable.EveryUpdate().Subscribe(_ =>
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                _sensor.IsHitTaking = false;
                stateSwitcher.SwitchState<MeleeNPCIdleState>();
            }
        }).AddTo(_disposable);
    }
}