using System;

public interface IEnemyStateSwitcher
{
    void SwitchStateTo<T>() where T : IEnemyState;
}
