using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private float _radius;

    public DamageTarget FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius, _layerMask);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Hurtbox target))
            {
                return target.Owner;
            }
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        // Gizmos.DrawSphere(transform.position, _radius);
    }
}
