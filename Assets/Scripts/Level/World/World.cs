using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class World : MonoBehaviour
{
    private const int MinActiveChunks = 3;
    private const int ActiveChunkIndex = 1;

    [SerializeField]
    private float _worldMovementSpeed = 2.5f;

    [SerializeField]
    [Tooltip("A list of chunks that will be cyclically repeated during the world movement")]
    private List<Chunk> _cycledChunks;

    [SerializeField]
    private LevelManager _levelManager;

    private Queue<Chunk> _chunksQueue;

    private WorldMovement _worldMovement;

    private CompositeDisposable _chunksDisposables = new();

    private void OnValidate()
    {
        if (_cycledChunks.Count == 0)
        {
            Debug.LogError("Chunks count must non zero!");
        }
    }

    private void Awake()
    {
        SetupWorldMovement();
        FillChunksQueue();
        ObserveChunkCollisionActions(GetActiveChunk());
    }

    public void MoveBackFor(float distance)
    {
        _worldMovement.MoveBackFor(distance);
    }

    private void SetupWorldMovement()
    {
        List<IWorldMovementHandler> worldMovementHandlers = new() 
        { 
            Player.Instance, _levelManager 
        };

        _worldMovement = new WorldMovement(transform, _worldMovementSpeed, worldMovementHandlers);
    }

    private Chunk GetActiveChunk()
    {
        return _chunksQueue.ElementAt(ActiveChunkIndex);
    }

    private void FillChunksQueue()
    {
        while (_cycledChunks.Count < MinActiveChunks)
        {
            _cycledChunks.Add(_cycledChunks[0]);
        }

        _chunksQueue = new Queue<Chunk>(_cycledChunks);
    }

    private void ObserveChunkCollisionActions(Chunk chunk)
    {
        chunk.BodyCollisionAction
            .Subscribe(action => HandlePlayerChunkCollisionAction(action))
            .AddTo(_chunksDisposables);
    }

    private void HandlePlayerChunkCollisionAction(ChunkCollisionAction action)
    {
        if (action == ChunkCollisionAction.BodyExit)
        {
            ShiftChunks();
        }
    }

    private void ShiftChunks()
    {
        _chunksDisposables.Clear();

        Chunk lastChunk = _chunksQueue.Last();
        Chunk firstChunk = _chunksQueue.Dequeue();

        _chunksQueue.Enqueue(firstChunk);

        SetupChunkPosition(firstChunk, lastChunk);

        ObserveChunkCollisionActions(GetActiveChunk());
    }

    private void SetupChunkPosition(Chunk chunkToSetup, Chunk lastChunk)
    {
        Vector3 chunkPosition = lastChunk.GetEndBorderPosition() + new Vector3(0, 0, chunkToSetup.GetDistanceFromStartToCenter());
        chunkToSetup.transform.position = chunkPosition;
    }
}
