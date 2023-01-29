using System;
using UniRx;

public class Reloading : WeaponStateBase
{
    private const float ReloadingTimeInterval = 0.1f;

    public Reloading(
        IWeaponStateSwitcher stateSwitcher,
        IProvider<WeaponStats> statsProvider) : base(stateSwitcher, statsProvider)
    {

    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        Observable.Interval(TimeSpan.FromSeconds(ReloadingTimeInterval))
            .Subscribe(times => HandleReloadingTime(times))
            .AddTo(Disposables);
    }

    private void HandleReloadingTime(long times)
    {
        float totalTime = times * ReloadingTimeInterval;

        if (totalTime >= StatsProvider.Get().ReloadingTime)
            SwitchStateTo<Idle>();
    }
}
