using UniRx;

public abstract class WeaponStateBase : IWeaponState
{
    protected IWeaponStateSwitcher StateSwitcher;
    protected IProvider<WeaponStats> StatsProvider;

    protected CompositeDisposable Disposables;

    public WeaponStateBase(
        IWeaponStateSwitcher stateSwitcher,
        IProvider<WeaponStats> statsProvider)
    {
        StateSwitcher = stateSwitcher;
        StatsProvider = statsProvider;

        Disposables = new CompositeDisposable();
    }

    public virtual void OnStateEnter() { }

    public virtual void OnStateExit() 
    {
        Disposables.Clear();
    }

    protected void SwitchStateTo<T>() where T : IWeaponState
    {
        StateSwitcher.SwitchStateTo<T>();
    }
}
