using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnArea : MonoBehaviour
{
    public IObservable<Unit> AreaCleared => _areaClearedSubject;

    private const float SpawnYAngle = 180f;

    private const int StartWaveIndex = -1;

    [SerializeField]
    private float _spawnAreaWidth = 1.0f;

    private IntReactiveProperty CurrentWave = new IntReactiveProperty(StartWaveIndex);

    private IntReactiveProperty _totalEnemiesLeft = new IntReactiveProperty(-1);

    private Subject<Unit> _areaClearedSubject = new Subject<Unit>();

    private CompositeDisposable _wavesSpawnDisposables = new();

    private void Awake()
    {
        Setup();
    }

    public void SpawnWaves(EnemyWaves enemyWaves)
    {
        if (enemyWaves.Waves.Count <= 0)
        {
            throw new ArgumentException("Wave must be non empty!");
        }

        _wavesSpawnDisposables.Clear();
        _totalEnemiesLeft.Value = enemyWaves.GetTotatlEnemiesCount();

        SetupWaveSpawner(enemyWaves);
        ScheduleFirstWave(enemyWaves);
    }

    private void Setup()
    {
        _totalEnemiesLeft
            .Where(count => count == 0)
            .Subscribe(_ => _areaClearedSubject.OnNext(Unit.Default))
            .AddTo(this);
    }

    private void SetupWaveSpawner(EnemyWaves enemyWaves)
    {
        CurrentWave
            .Where(waveIndex => waveIndex > StartWaveIndex)
            .Take(enemyWaves.Waves.Count)
            .Subscribe(waveIndex => SetupWave(enemyWaves, waveIndex))
            .AddTo(_wavesSpawnDisposables);
    }

    private void SetupWave(EnemyWaves enemyWaves, int currentWaveIndex)
    {
        SpawnWave(enemyWaves.Waves[currentWaveIndex]);
        StartCoroutine(ScheduleNextWave(enemyWaves, currentWaveIndex));
    }

    private void ScheduleFirstWave(EnemyWaves enemyWaves)
    {
        StartCoroutine(ScheduleNextWave(enemyWaves, StartWaveIndex));
    }

    private IEnumerator ScheduleNextWave(EnemyWaves enemyWaves, int currentWave)
    {
        int nextWave = currentWave + 1;

        if (nextWave < enemyWaves.Waves.Count)
        {
            float delaySeconds = enemyWaves.Waves[nextWave].WaveStartDelay;
            yield return new WaitForSeconds(delaySeconds);
            TriggerNextWaveIfExist(enemyWaves, currentWave);
        }
    }

    private void TriggerNextWaveIfExist(EnemyWaves enemyWaves, int currentWave)
    {
        if (currentWave < enemyWaves.Waves.Count - 1)
        {
            CurrentWave.Value = currentWave + 1;
        } 
    }

    private void SpawnWave(EnemyWave wave)
    {
        Observable.Interval(TimeSpan.FromSeconds(wave.EnemiesSpawnDelay))
            .Take(wave.EnemiesCount)
            .Subscribe(_ => SpawnSingleEnemy(wave.EnemyPrefab))
            .AddTo(_wavesSpawnDisposables);
    }

    private void SpawnSingleEnemy(Enemy enemyPrefab)
    {
        float _randomX = Random.Range(-_spawnAreaWidth, _spawnAreaWidth);
        Vector3 _randomPosition = transform.position.WithX(_randomX);
        Enemy enemy = Instantiate(enemyPrefab, _randomPosition, Quaternion.Euler(0, SpawnYAngle, 0));

        ObserveEnemyEvents(enemy);
    }

    private void ObserveEnemyEvents(Enemy enemy)
    {
        enemy.DamageTarget
            .TargetDead
            .Subscribe(_ => ReduceEnemiesCount())
            .AddTo(_wavesSpawnDisposables);
    }

    private void ReduceEnemiesCount()
    {
        _totalEnemiesLeft.Value = _totalEnemiesLeft.Value - 1;
    }
}
