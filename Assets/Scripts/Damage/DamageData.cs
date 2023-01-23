public struct DamageData
{
    public int BaseAmount;

    public bool IsCritical;
    public double CriticalMultiplyer;
    public int AdditionalAmount;

    public DamageType DamageType;

    public int GetTotalDamageAmount()
    {
        if (IsCritical)
        {
            return (int)((BaseAmount + AdditionalAmount) * CriticalMultiplyer);
        } 
        else
        {
            return (BaseAmount + AdditionalAmount);
        }
    }
}

public enum DamageType
{
    Physical,
    Fire
}
