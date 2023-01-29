public interface IWeaponStateSwitcher
{
    void SwitchStateTo<T>() where T : IWeaponState;
}
