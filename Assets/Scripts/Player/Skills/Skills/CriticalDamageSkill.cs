using Random = System.Random;

public class CriticalDamageSkill : IDamageSkill, ISkill
{
    public string Description => string.Format(DescriptionFormat, _additionalCriticalRate, _criticalChance);

    private const string DescriptionFormat = "Deal additional {0} damage with {1} chance.";

    private double _additionalCriticalRate;
    private double _criticalChance;

    private Random _random = new Random();

    public CriticalDamageSkill(double additionalCriticalRate, double criticalChance)
    {
        _additionalCriticalRate = additionalCriticalRate;
        _criticalChance = criticalChance;
    }

    public DamageData ApplyForDamage(DamageData damage)
    {
        if (_random.NextDouble() <= _criticalChance)
        {
            damage.IsCritical = true;
            damage.CriticalMultiplyer = (1 + _additionalCriticalRate);
        }

        return damage;
    }
}
