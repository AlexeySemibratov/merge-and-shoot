using UnityEngine;

public class FlashParticlesFactory : IFactory<ParticleSystem>
{
    private ParticleSystem _particlesPrefab;
    private Transform _gunMuzzleTransform;

    public FlashParticlesFactory(
        ParticleSystem particlesPrefab,
        Transform gunMuzzleTransform)
    {
        _particlesPrefab = particlesPrefab;
        _gunMuzzleTransform = gunMuzzleTransform;
    }

    public ParticleSystem Create()
    {
        return Object.Instantiate(_particlesPrefab, _gunMuzzleTransform);
    }
}
