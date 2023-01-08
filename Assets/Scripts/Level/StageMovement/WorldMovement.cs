
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class WorldMovement
{
    private float _movementSpeed;
    private Transform _transform;
    private List<IWorldMovementHandler> _movementHandlers;

    private CompositeDisposable _movementDisposable = new CompositeDisposable();

    public WorldMovement(Transform transform, float movementSpeed, List<IWorldMovementHandler> handlers)
    {
        _movementSpeed = movementSpeed;
        _transform = transform;

        if (handlers == null)
        {
            _movementHandlers = new List<IWorldMovementHandler>();
        }
        else
        {
            _movementHandlers = handlers;
        }
    }

    public void MoveBackFor(float distance)
    {
        OnStartMovement();

        float currentDistance = 0;
        Observable.EveryUpdate()
            .TakeWhile(_ => currentDistance < distance)

            .Subscribe(
            onNext: _ => {
                float step = Time.deltaTime * _movementSpeed;
                currentDistance += step;
                MoveForwardForStep(step);
            },
            onCompleted: () => StopMovement())
            .AddTo(_movementDisposable);
    }

    private void MoveForwardForStep(float step)
    {
        Vector3 currentPosition = _transform.position;
        currentPosition.z = currentPosition.z - step;
        _transform.position = currentPosition;
    }

    private void StopMovement()
    {
        OnEndMovement();
        _movementDisposable.Clear();
    }

    private void OnStartMovement()
    {
        foreach (IWorldMovementHandler handler in _movementHandlers)
        {
            handler.OnWorldStartMovement();
        }
    }

    private void OnEndMovement()
    {
        foreach (IWorldMovementHandler handler in _movementHandlers)
        {
            handler.OnWorldEndMovement();
        }
    }
}
