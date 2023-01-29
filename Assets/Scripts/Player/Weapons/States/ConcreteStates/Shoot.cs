using UnityEngine;

public class Shoot : WeaponStateBase
{
    private IFactory<Bullet> _bulletFactory;
    private IFactory<ParticleSystem> _flashParticlesFactory;
    private PlayerAnimator _animator;

    public Shoot(
        IWeaponStateSwitcher stateSwitcher,
        IProvider<WeaponStats> statsProvider,
        IFactory<Bullet> bulletFactory,
        IFactory<ParticleSystem> flashParticlesFactory,
        PlayerAnimator animator) : base(stateSwitcher, statsProvider)
    {
        _bulletFactory = bulletFactory;
        _flashParticlesFactory = flashParticlesFactory;
        _animator = animator;
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        _bulletFactory.Create();
        SpawnFlashParticles();
        _animator.PlayShoot();

        SwitchStateTo<Reloading>();
    }

    private void SpawnFlashParticles()
    {
        var particles = _flashParticlesFactory.Create();
        Object.Destroy(particles.gameObject, particles.main.duration);
    }
}
