using UniRx;
using UnityEngine;

public class Idle : WeaponStateBase
{
    private ITargetKeeper _targetKeeper;

    public Idle(
        IWeaponStateSwitcher stateSwitcher, 
        IProvider<WeaponStats> statsProvider,
        ITargetKeeper targetKeeper) : base(stateSwitcher, statsProvider)
    {
        _targetKeeper = targetKeeper;
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        Observable.EveryUpdate()
            .Subscribe(_ => CheckTarget())
            .AddTo(Disposables);
    }

    private void CheckTarget()
    {
        if (_targetKeeper.GetTarget() != null)
            SwitchStateTo<Shoot>();
    }
}
