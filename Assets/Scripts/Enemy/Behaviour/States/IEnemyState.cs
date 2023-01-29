using UnityEngine;

public interface IEnemyState
{
    void OnStateEnter();

    void OnStateExit();

    void OnStateDestroyed();

    void OnTriggerEnter(Collider other);

    void OnTriggerExit(Collider other);
}
