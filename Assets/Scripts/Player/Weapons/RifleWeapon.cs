using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(DamageTarget))]
public class RifleWeapon : MonoBehaviour, IUpgradeable, IWeaponStateSwitcher, IProvider<WeaponStats>
{
    public IObservable<RifleWeaponEvent> SingleShootPerformed => _performSingleShootSubject;
    private Subject<RifleWeaponEvent> _performSingleShootSubject = new Subject<RifleWeaponEvent>();

    public IReadOnlyReactiveProperty<int> Level => _currentLevel;
    private ReactiveProperty<int> _currentLevel = new ReactiveProperty<int>(1);

    [SerializeField]
    private LayerMask _enemiesLayerMask;

    [SerializeField]
    private Bullet _bulletPrefab;

    [SerializeField]
    private ParticleSystem _flashParticlesPrefab;

    [SerializeField]
    private Transform _gunMuzzleTransform;

    [SerializeField]
    private PlayerAnimator _animator;

    [SerializeField]
    private WeaponStats _weaponStats;

    private IDamageSkillRegistry _damageSkills;
    private IFactory<Bullet> _bulletFactory;
    private IFactory<ParticleSystem> _flashParticlesFactory;

    private IDamageTarget _owner;
    private ITargetKeeper _targetKeeper;

    private List<IWeaponState> _states;

    private IWeaponState _currentState;

    private void Awake()
    {
        InitializeComponents();
        InitializeStates();
    }

    public void SwitchStateTo<T>() where T : IWeaponState
    {
        var type = typeof(T);

        var newState = _states.FirstOrDefault(e => e.GetType() == type);

        if (newState == null)
            throw new ArgumentException($"State {type} not found");

        if (_currentState != null)
            _currentState.OnStateExit();

        _currentState = newState;
        _currentState.OnStateEnter();
    }

    public WeaponStats Get()
    {
        return _weaponStats;
    }

    public void ApplyUpgrade(UpgradeItem upgrade)
    {
        _currentLevel.Value = upgrade.Level;
        _weaponStats.UpdateStats(upgrade.Level);
    }

    public void Disable()
    {
        _currentState = null;
    }

    private void InitializeComponents()
    {
        _owner = GetComponent<IDamageTarget>();
        _targetKeeper = GetComponent<ITargetKeeper>();

        var skillContainer = new SkillContainer(); // TODO Temporal. Rifle class is not a skill container
        skillContainer.AddSkill(new CriticalDamageSkill(0.5f, 0.3f));

        _damageSkills = skillContainer;

        _bulletFactory = new RifleBulletFactory(_bulletPrefab, _gunMuzzleTransform, this, _damageSkills, _owner);
        _flashParticlesFactory = new FlashParticlesFactory(_flashParticlesPrefab, _gunMuzzleTransform);
    }

    private void InitializeStates()
    {
        _states = new List<IWeaponState>() 
        { 
            new Shoot(this, this, _bulletFactory, _flashParticlesFactory, _animator),
            new Reloading(this, this),
            new Idle(this, this, _targetKeeper)
        };

        SwitchStateTo<Idle>();
    }
}
