using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public HitboxData HitboxData { get; private set; }

    [SerializeField]
    private bool _destroyGameObjectWhenHit = false;

    public void SetHitboxData(HitboxData data)
    {
        HitboxData = data;  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Hurtbox hurtbox))
        {
            OnHitTarget(hurtbox.Owner);
        }
    }

    private void OnHitTarget(IDamageTarget target)
    {
        HitboxData.Owner.DealDamage(target, HitboxData.DamageAmount);

        if (_destroyGameObjectWhenHit)
        {
            Destroy(gameObject);
        }
    }
}
