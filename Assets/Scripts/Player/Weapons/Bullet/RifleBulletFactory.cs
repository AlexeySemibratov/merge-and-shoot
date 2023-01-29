using UnityEngine;
public class RifleBulletFactory : IFactory<Bullet>
{
    private Bullet _bulletPrefab;
    private Transform _gunMuzzleTransform;
    private IProvider<WeaponStats> _weaponStatsProvider;
    private IDamageSkillRegistry _damageSkillRegistry;
    private IDamageTarget _rifleOwner;

    public RifleBulletFactory(
        Bullet bulletPrefab, 
        Transform gunMuzzleTransform, 
        IProvider<WeaponStats> weaponStatsProvider, 
        IDamageSkillRegistry damageSkillRegistry,
        IDamageTarget rifleOwner)
    {
        _bulletPrefab = bulletPrefab;
        _gunMuzzleTransform = gunMuzzleTransform;
        _weaponStatsProvider = weaponStatsProvider;
        _damageSkillRegistry = damageSkillRegistry;
        _rifleOwner = rifleOwner;
    }

    public Bullet Create()
    {
        Bullet bullet = Object.Instantiate(_bulletPrefab, _gunMuzzleTransform.position, _gunMuzzleTransform.rotation);

        var hitboxData = new HitboxData
        {
            Owner = _rifleOwner,
            DamageData = CalculateDamageData(),
        };

        bullet.SetHitboxData(hitboxData);

        return bullet;
    }

    private DamageData CalculateDamageData()
    {
        var basicDamage = new DamageData
        {
            BaseAmount = _weaponStatsProvider.Get().DamagePerBullet,
            CriticalMultiplyer = 1.0f,
            IsCritical = false,
            AdditionalAmount = 0,
            DamageType = DamageType.Physical
        };

        foreach (IDamageSkill skill in _damageSkillRegistry.GetDamageSkills())
        {
            basicDamage = skill.ApplyForDamage(basicDamage);
        }

        return basicDamage;
    }
}
