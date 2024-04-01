using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class MeleeNPCTakeHitState : NPCBaseState
{
    private int _takeHitHash;
    private CompositeDisposable _disposable = new CompositeDisposable();
    private NPCSensor _sensor;
    private CancellationTokenSource _cts;

    public MeleeNPCTakeHitState(IStateSwitcher stateSwitcher,
        CharacterController characterController, NPCSensor sensor, Animator animator, Transform transform)
        : base(stateSwitcher, characterController, animator,sensor, transform)
    {
        _sensor = sensor;
        _takeHitHash = Animator.StringToHash("TakeHit");
    }

    public override void Enter()
    {
        _cts = new();
        animator.StopPlayback();
        animator.CrossFadeInFixedTime(_takeHitHash, 0.1f);

        TakeHit();
    }

    public override void Exit()
    {
        animator.StopPlayback();
        _disposable.Clear();
        _cts.Cancel();
        _cts.Dispose();
    }

    private async void TakeHit()
    {
        await UniTask.Delay(100, cancellationToken: _cts.Token);

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