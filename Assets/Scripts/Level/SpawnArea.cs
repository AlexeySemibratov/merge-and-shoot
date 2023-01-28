using System;
using System.Collections;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnArea : MonoBehaviour
{
    public IObservable<Unit> AreaCleared => _areaClearedSubject;
    private Subject<Unit> _areaClearedSubject = new Subject<Unit>();

    private const float SpawnYAngle = 180f;

    [SerializeField]
    private float _spawnAreaWidth = 1.0f;

    private RewardManager _rewardManager => GameContext.Instance.RewardManager;

    private IntReactiveProperty _totalEnemiesLeft = new IntReactiveProperty(-1);

    private CompositeDisposable _spawnedEnemiesEventsContainer = new();

    private void Awake()
    {
        Setup();
    }

    public void SpawnWaves(EnemyWaves enemyWaves)
    {
        _spawnedEnemiesEventsContainer.Clear();
        _totalEnemiesLeft.Value = enemyWaves.GetTotatlEnemiesCount();

        StartCoroutine(SpawnWavesInternal(enemyWaves));
    }

    private void Setup()
    {
        _totalEnemiesLeft
            .Where(count => count == 0)
            .Subscribe(_ => _areaClearedSubject.OnNext(Unit.Default))
            .AddTo(this);
    }

    private IEnumerator SpawnWavesInternal(EnemyWaves enemyWaves)
    {
        for (int i = 0; i < enemyWaves.Waves.Count; i++)
        {
            EnemyWave wave = enemyWaves.Waves[i];
            yield return new WaitForSeconds(wave.WaveStartDelay);

            StartCoroutine(SpawnWave(wave));
        }
    }

    private IEnumerator SpawnWave(EnemyWave wave)
    {
        for (int i = 0; i < wave.EnemiesCount; i++)
        {
            SpawnSingleEnemy(wave.EnemyPrefab);
            yield return new WaitForSeconds(wave.EnemiesSpawnDelay);
        }
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
            .Subscribe(_ => {
                AddRewardForEnemy(enemy);
                ReduceEnemiesCount();
                })
            .AddTo(_spawnedEnemiesEventsContainer);
    }

    private void AddRewardForEnemy(Enemy enemy)
    {
        _rewardManager.AddRewardForEnemy(enemy.MoneyDropAmount);
    }

    private void ReduceEnemiesCount()
    {
        _totalEnemiesLeft.Value = _totalEnemiesLeft.Value - 1;
    }
}
