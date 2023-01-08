using System;
using System.Collections;
using UniRx;
using UnityEngine;

public class LevelManager : MonoBehaviour, IWorldMovementHandler
{
    public ReactiveProperty<int> CurrentLevel => _currentLevelNumber;

    private const float LevelSwitchDelay = 1.5f;
    private const float LevelStartDelay = 1f;

    [SerializeField]
    private World _world;

    [SerializeField]
    private SpawnArea _spawnArea;

    [SerializeField]
    private EnemyWaves _waves;

    [SerializeField]
    private float _distanceBetweenStages;

    private IntReactiveProperty _currentLevelNumber = new IntReactiveProperty(1);

    private Player _player => Player.Instance;

    private void Awake()
    {
        Setup();
    }

    public void OnWorldStartMovement()
    {
    }

    public void OnWorldEndMovement()
    {
        StartCoroutine(IncrementLevelNumber());
    }

    private void Setup()
    {
        _spawnArea.AreaCleared
            .Delay(TimeSpan.FromSeconds(LevelSwitchDelay))
            .Subscribe(_ => GoToNextLevel())
            .AddTo(this);

        _currentLevelNumber
            .Subscribe(stage => SetupStage(stage))
            .AddTo(this);
    }

    private void GoToNextLevel()
    {
        _world.MoveBackFor(_distanceBetweenStages);
    }

    private void SetupStage(int stageNumber)
    {
        Debug.Log($"Spawn waves. stage {stageNumber}");
        _spawnArea.SpawnWaves(_waves);
    }

    private IEnumerator IncrementLevelNumber()
    {
        yield return new WaitForSeconds(LevelStartDelay);

        _currentLevelNumber.Value = _currentLevelNumber.Value + 1;
    }
}
