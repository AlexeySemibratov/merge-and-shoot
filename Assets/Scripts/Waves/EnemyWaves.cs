using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Waves", menuName = "ScriptableObjects/Waves")]
public class EnemyWaves : ScriptableObject
{
    public List<EnemyWave> Waves;

    public int GetTotatlEnemiesCount()
    {
        return Waves.Aggregate(0, (acc, wave) => acc + wave.EnemiesCount);
    }
}
