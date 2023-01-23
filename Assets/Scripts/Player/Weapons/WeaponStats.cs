[System.Serializable]
public class WeaponStats
{
    private const int DamagePerLelevelMultiplier = 5;

    public float ReloadingTime = 0.5f;
    public int DamagePerBullet = 10;

    public void UpdateStats(int level)
    {
        DamagePerBullet += DamagePerLelevelMultiplier * level;
    }
}
