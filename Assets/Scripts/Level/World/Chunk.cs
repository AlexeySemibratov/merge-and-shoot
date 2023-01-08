using System;
using UniRx;
using UnityEngine;
using Random = System.Random;

public class Chunk : MonoBehaviour
{
    public int id = new Random().Next(100);

    public IObservable<ChunkCollisionAction> BodyCollisionAction => _bodyCollisionAction;

    private Subject<ChunkCollisionAction> _bodyCollisionAction = new Subject<ChunkCollisionAction>();

    [SerializeField]
    private ChunkBorderZone _chunkEnterZone;

    [SerializeField]
    private ChunkBorderZone _chunkExitZone;

    private void Awake()
    {
        Observable.Merge(
            _chunkEnterZone.BodyEnterZone.Select(_ => ChunkCollisionAction.BodyEnter),
            _chunkExitZone.BodyEnterZone.Select(_ => ChunkCollisionAction.BodyExit))
            .Subscribe(action => _bodyCollisionAction.OnNext(action))
            .AddTo(this);   
    }

    public float GetDistanceFromStartToCenter()
    {
        return (transform.position - _chunkEnterZone.transform.position).magnitude;
    }

    public Vector3 GetEndBorderPosition()
    {
        return _chunkExitZone.transform.position;
    }

    public float GetLength()
    {
        return (_chunkExitZone.transform.position - _chunkEnterZone.transform.position).magnitude;
    }
}
