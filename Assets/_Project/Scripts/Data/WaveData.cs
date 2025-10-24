using UnityEngine;
using System.Collections.Generic;

namespace CVA.Data
{
    /// <summary>
    /// Defines a wave configuration with enemy types, spawn rates, and duration.
    /// Used by WaveManager for progressive difficulty.
    /// </summary>
    [CreateAssetMenu(fileName = "Wave_New", menuName = "CVA/Data/Wave Data")]
    public class WaveData : ScriptableObject
    {
        [Header("Wave Info")]
        [SerializeField] private string _waveName = "Wave 1";
        [SerializeField, TextArea] private string _description = "Survive the first wave";

        [Header("Duration")]
        [Tooltip("How long this wave lasts in seconds")]
        [SerializeField] private float _waveDuration = 60f;

        [Header("Enemy Configuration")]
        [Tooltip("Which enemy types can spawn in this wave")]
        [SerializeField] private List<EnemySpawnEntry> _enemyTypes = new List<EnemySpawnEntry>();

        [Header("Spawn Settings")]
        [Tooltip("How many enemies spawn at once")]
        [SerializeField] private int _enemiesPerSpawn = 2;

        [Tooltip("Time between spawns")]
        [SerializeField] private float _spawnInterval = 2f;

        [Tooltip("Maximum enemies alive at once")]
        [SerializeField] private int _maxEnemies = 50;

        // Properties
        public string WaveName => _waveName;
        public string Description => _description;
        public float WaveDuration => _waveDuration;
        public int EnemiesPerSpawn => _enemiesPerSpawn;
        public float SpawnInterval => _spawnInterval;
        public int MaxEnemies => _maxEnemies;

        /// <summary>
        /// Get a random enemy type based on spawn weights.
        /// </summary>
        public EnemyData GetRandomEnemyType()
        {
            if (_enemyTypes == null || _enemyTypes.Count == 0)
                return null;

            // Calculate total weight
            int totalWeight = 0;
            foreach (var entry in _enemyTypes)
            {
                if (entry.EnemyData != null)
                    totalWeight += entry.SpawnWeight;
            }

            if (totalWeight == 0)
                return _enemyTypes[0].EnemyData;

            // Random weighted selection
            int randomValue = Random.Range(0, totalWeight);
            int currentWeight = 0;

            foreach (var entry in _enemyTypes)
            {
                if (entry.EnemyData != null)
                {
                    currentWeight += entry.SpawnWeight;
                    if (randomValue < currentWeight)
                        return entry.EnemyData;
                }
            }

            return _enemyTypes[0].EnemyData;
        }
    }

    /// <summary>
    /// Enemy type with spawn weight for weighted random selection.
    /// </summary>
    [System.Serializable]
    public class EnemySpawnEntry
    {
        public EnemyData EnemyData;
        [Tooltip("Higher = more common. Example: Common=70, Rare=10")]
        public int SpawnWeight = 100;
    }
}
