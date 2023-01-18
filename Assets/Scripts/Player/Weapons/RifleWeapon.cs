using System;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(DamageTarget))]
public class RifleWeapon : MonoBehaviour, IUpgradeable
{
    public IObservable<RifleWeaponEvent> SingleShootPerformed { get => _performSingleShootSubject; }

    public IReadOnlyReactiveProperty<int> Level { get => _currentLevel; }

    private const float ReloadingTimeInterval = 0.1f;

    private const int DamagePerLelevelMultiplier = 5;

    [SerializeField]
    private LayerMask _enemiesLayerMask;

    [SerializeField]
    private Bullet _bulletPrefab;

    [SerializeField]
    private ParticleSystem _flashParticlesPrefab;

    [SerializeField]
    private Transform _gunMuzzleTransform;

    [SerializeField]
    private float _reloadingTime = 0.5f;

    [SerializeField]
    private int _damagePerBullet = 10;

    private Subject<RifleWeaponEvent> _performSingleShootSubject = new Subject<RifleWeaponEvent>();

    private IDamageTarget _owner;

    private ReactiveProperty<State> _currentState = new ReactiveProperty<State>(State.ReadyToFire);

    private CompositeDisposable _reloadingDisposable = new CompositeDisposable();

    private ReactiveProperty<int> _currentLevel = new ReactiveProperty<int>(1);

    private void Awake()
    {
        _owner = GetComponent<IDamageTarget>();
    }

    public void ApplyUpgrade(UpgradeItem upgrade)
    {
        _currentLevel.Value = upgrade.Level;
        UpdateStats(upgrade.Level);
    }

    private void UpdateStats(int level)
    {
        _damagePerBullet += DamagePerLelevelMultiplier * level;
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
        if (totalTime >= _reloadingTime)
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
            DamageAmount = _damagePerBullet,
        };

        bullet.SetHitboxData(hitboxData);
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
