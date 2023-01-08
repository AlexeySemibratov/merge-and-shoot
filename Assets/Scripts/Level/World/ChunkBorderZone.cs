using System;
using UniRx;
using UnityEngine;

public class ChunkBorderZone : MonoBehaviour
{
    public IObservable<Unit> BodyEnterZone => _bodyAnterZoneSubject;

    private Subject<Unit> _bodyAnterZoneSubject = new Subject<Unit>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            _bodyAnterZoneSubject.OnNext(Unit.Default);
        }
    }
}
