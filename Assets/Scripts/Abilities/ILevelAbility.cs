using UniRx;

public interface ILevelAbility
{
    IReadOnlyReactiveProperty<int> Level { get; }

    void OnLevelUp(int level);
}
