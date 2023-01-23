using System;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(DamageTarget))]
public class RifleWeapon : MonoBehaviour, IUpgradeable
{
    public IObservable<RifleWeaponEvent> SingleShootPerformed => _performSingleShootSubject;
    private Subject<RifleWeaponEvent> _performSingleShootSubject = new Subject<RifleWeaponEvent>();

    public IReadOnlyReactiveProperty<int> Level => _currentLevel;
    private ReactiveProperty<int> _currentLevel = new ReactiveProperty<int>(1);

    private const float ReloadingTimeInterval = 0.1f;

    [SerializeField]
    private LayerMask _enemiesLayerMask;

    [SerializeField]
    private Bullet _bulletPrefab;

    [SerializeField]
    private ParticleSystem _flashParticlesPrefab;

    [SerializeField]
    private Transform _gunMuzzleTransform;

    [SerializeField]
    private WeaponStats _weaponStats;

    private IDamageSkillRegistry _damageSkills;

    private IDamageTarget _owner;

    private ReactiveProperty<State> _currentState = new ReactiveProperty<State>(State.ReadyToFire);

    private CompositeDisposable _reloadingDisposable = new CompositeDisposable();

    private void Awake()
    {
        _owner = GetComponent<IDamageTarget>();

        var skillContainer = new SkillContainer(); // TODO Temporal. Rifle class is not a skill container
        skillContainer.AddSkill(new CriticalDamageSkill(0.5f, 0.3f));

        _damageSkills = skillContainer;
    }

    public void ApplyUpgrade(UpgradeItem upgrade)
    {
        _currentLevel.Value = upgrade.Level;
        _weaponStats.UpdateStats(upgrade.Level);
    }

    public void ShootAt(DamageTarget target)
    {
        if (_currentState.Value != State.ReadyToFire)
            return;

        Vector3 direction = _gunMuzzleTransform.transform.position.DirectedTo(target.transform.position);

        if (Physics.Raycast(_gunMuzzleTransform.transform.position, direction, out RaycastHit hit, Mathf.Infinity, _enemiesLayerMask))
        {
            Shoot();
        }
    }

    private void ChangeStateTo(State newState)
    {
        _currentState.Value = newState;
    }

    private void Shoot()
    {
        _performSingleShootSubject.OnNext(RifleWeaponEvent.SingleShoot);
        StartReloading();
        SpawnFlashParticles();
        SpawnBullet();
    }

    private void StartReloading()
    {
        ChangeStateTo(State.Reloading);

        Observable.Interval(TimeSpan.FromSeconds(ReloadingTimeInterval))
            .Subscribe(time => HandleReloadingTime(time))
            .AddTo(_reloadingDisposable);
    }

    private void HandleReloadingTime(long time)
    { 
        float totalTime = time * ReloadingTimeInterval;
        if (totalTime >= _weaponStats.ReloadingTime)
        {
            _reloadingDisposable.Clear();
            _currentState.Value = State.ReadyToFire;
        }
    }

    private void SpawnBullet()
    {
        Bullet bullet = Bullet.Instantiate(_bulletPrefab, _gunMuzzleTransform.position, _gunMuzzleTransform.rotation);
        var hitboxData = new HitboxData
        {
            Owner = _owner,
            DamageData = CalculateDamageData(),
        };

        bullet.SetHitboxData(hitboxData);
    }

    private DamageData CalculateDamageData()
    {
        var basicDamage = new DamageData
        {
            BaseAmount = _weaponStats.DamagePerBullet,
            CriticalMultiplyer = 1.0f,
            IsCritical = false,
            AdditionalAmount = 0,
            DamageType = DamageType.Physical
        };

        foreach (IDamageSkill skill in _damageSkills.GetDamageSkills())
        {
            basicDamage = skill.ApplyForDamage(basicDamage);
        }

        return basicDamage;
    }

    private void SpawnFlashParticles()
    {
        var particles = Instantiate(_flashParticlesPrefab, _gunMuzzleTransform);
        Destroy(particles.gameObject, particles.main.duration);
    }

    private enum State
    {
        ReadyToFire,
        Reloading
    }
}
