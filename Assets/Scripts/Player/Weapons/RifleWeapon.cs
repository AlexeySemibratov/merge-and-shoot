using System;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(DamageTarget))]
public class RifleWeapon : MonoBehaviour, ILevelAbility
{
    public IObservable<RifleWeaponEvent> SingleShootPerformed { get => _performSingleShootSubject; }

    public IReadOnlyReactiveProperty<int> Level { get => _currentLevel; }

    private const float ReloadingTimeInterval = 0.1f;

    [SerializeField]
    private LayerMask _enemiesLayerMask;

    [SerializeField]
    private Bullet _bulletPrefab;

    [SerializeField]
    private Transform _gunMuzzleTransform;

    [SerializeField]
    private float _reloadingTime = 0.5f;

    [SerializeField]
    private int _baseDamagePerBullet = 50;

    private int _damagePerBullet = 50;

    private Subject<RifleWeaponEvent> _performSingleShootSubject = new Subject<RifleWeaponEvent>();

    private IDamageTarget _owner;

    private ReactiveProperty<State> _currentState = new ReactiveProperty<State>(State.ReadyToFire);

    private CompositeDisposable _reloadingDisposable = new CompositeDisposable();

    private ReactiveProperty<int> _currentLevel = new ReactiveProperty<int>(1);

    private void Start()
    {
        UpdateStatsFromLevel();

        _owner = GetComponent<IDamageTarget>();
    }

    public void OnLevelUp(int level)
    {
        _currentLevel.Value = level;
    }

    private void UpdateStatsFromLevel()
    {
        _currentLevel.Subscribe(level => UpdateDamage(level));
    }

    private void UpdateDamage(int level)
    {
        _damagePerBullet = _baseDamagePerBullet * level;
    }

    public void ShootAt(DamageTarget target)
    {
        if (_currentState.Value != State.ReadyToFire)
            return;

        Vector3 direction = _gunMuzzleTransform.transform.position.DirectedTo(target.transform.position);

        if (Physics.Raycast(_gunMuzzleTransform.transform.position, direction, out RaycastHit hit, Mathf.Infinity, _enemiesLayerMask))
        {
            ShootAt(direction);
        }
    }

    private void ChangeStateTo(State newState)
    {
        _currentState.Value = newState;
    }

    private void ShootAt(Vector3 direction)
    {
        _performSingleShootSubject.OnNext(RifleWeaponEvent.SingleShoot);
        StartReloading();
        SpawnBullet(direction);
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

    private void SpawnBullet(Vector3 direction)
    {
        Bullet bullet = Bullet.Instantiate(_bulletPrefab, _gunMuzzleTransform.position, Quaternion.LookRotation(direction));
        var hitboxData = new HitboxData
        {
            Owner = _owner,
            DamageAmount = _damagePerBullet,
        };

        bullet.SetHitboxData(hitboxData);
    }

    private enum State
    {
        ReadyToFire,
        Reloading
    }
}
