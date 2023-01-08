public class UpgradeItem
{
    public int Level { get; private set; }

    public int Cost { get; private set; }

    public UpgradeItem(int level, int cost)
    {
        Level = level;
        Cost = cost;    
    }

    public void Apply(IUpgradeable upgreadable)
    {
        upgreadable.ApplyUpgrade(this);
    }
}
