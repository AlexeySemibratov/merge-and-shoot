using System;
using System.Collections.Generic;
using System.Linq;

public struct EnemyWaves
{
    public class Builder
    {
        private List<EnemyWave> _waves;

        public Builder()
        {
            _waves = new List<EnemyWave>();
        }

        public Builder AddWave(
            Enemy enemyPrefab,
            int enemiesCount = 1,
            float waveStartDelay = 1.0f,
            float enemiesSpawnDelay = 1.0f)
        {
            if (enemiesCount < 1 || waveStartDelay < 0 || enemiesSpawnDelay < 0)
                throw new ArgumentException("One ore params was incorrect while create wave");

            var wave = new EnemyWave()
            {
                EnemyPrefab = enemyPrefab,
                EnemiesCount = enemiesCount,
                WaveStartDelay = waveStartDelay,
                EnemiesSpawnDelay= enemiesSpawnDelay
            };
            _waves.Add(wave);

            return this;
        }

        public EnemyWaves Build()
        {
            if (_waves.Count == 0)
                throw new ArgumentException("You must add at least one wave.");

            return new EnemyWaves(_waves);
        }
    }

    public List<EnemyWave> Waves;

    public EnemyWaves(List<EnemyWave> waveList)
    {
        Waves = waveList;
    }

    public int GetTotatlEnemiesCount()
    {
        return Waves.Aggregate(0, (acc, wave) => acc + wave.EnemiesCount);
    }
}
