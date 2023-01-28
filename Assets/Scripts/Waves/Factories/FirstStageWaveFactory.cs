public class FirstStageWaveFactory : IEnemyWavesFactory
{
    private Enemy _enemyBugGreenPrefab;
    private Enemy _enemyBugRedPrefab;

    public FirstStageWaveFactory(Enemy enemyBugGreenPrefab, Enemy enemyBugRedPrefab)
    {
        _enemyBugGreenPrefab = enemyBugGreenPrefab;
        _enemyBugRedPrefab = enemyBugRedPrefab;
    }

    public EnemyWaves CreateWaves(int levelNumber)
    {
        var builder = new EnemyWaves.Builder();

        var waveStartDelay = 1.0f;
        builder.AddWave(_enemyBugGreenPrefab, 1 + levelNumber, waveStartDelay);

        if (levelNumber >= 2)
        {
            waveStartDelay += 4;
            builder.AddWave(_enemyBugRedPrefab, levelNumber - 1, waveStartDelay, 2.0f);
        }

        if (levelNumber >= 6)
        {
            waveStartDelay += 10;
            builder.AddWave(_enemyBugGreenPrefab, levelNumber / 2, waveStartDelay);
        }

        return builder.Build();
    }
}
