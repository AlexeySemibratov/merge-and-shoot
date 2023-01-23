using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public DamageTarget Owner => _owner;

    [SerializeField]
    private DamageTarget _owner;

    public virtual void OnReceiveDamage(DamageData damageData)
    {
    }
}
