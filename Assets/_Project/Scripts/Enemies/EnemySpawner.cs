using UnityEngine;
using CVA.Data;
using CVA.Core;

namespace CVA.Enemies
{
    /// <summary>
    /// Spawns enemies around the player in waves.
    /// Integrates with WaveManager for progressive difficulty.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Enemy Configuration")]
        [SerializeField] private Enemy _enemyPrefab;
        [SerializeField] private EnemyData[] _enemyTypes;

        [Header("Spawn Settings")]
        [SerializeField] private float _spawnInterval = 2f;
        [SerializeField] private int _enemiesPerSpawn = 1;
        [SerializeField] private float _spawnDistance = 15f;
        [SerializeField] private int _maxEnemies = 100;

        [Header("Wave Integration")]
        [SerializeField] private WaveManager _waveManager;
        [SerializeField] private bool _autoFindWaveManager = true;

        // Runtime state
        private Transform _playerTransform;
        private float _spawnTimer = 0f;
        private WaveData _currentWaveData;

        #region Unity Lifecycle

        private void Start()
        {
            FindPlayer();
            FindWaveManager();
        }

        private void Update()
        {
            if (_playerTransform == null)
                return;

            _spawnTimer += Time.deltaTime;

            if (_spawnTimer >= _spawnInterval)
            {
                SpawnEnemies();
                _spawnTimer = 0f;
            }
        }

        #endregion

        #region Initialization

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
        }

        private void FindWaveManager()
        {
            if (_autoFindWaveManager && _waveManager == null)
            {
                _waveManager = FindFirstObjectByType<WaveManager>();
            }

            if (_waveManager != null)
            {
                _waveManager.OnWaveStart.AddListener(OnWaveStart);
            }
        }

        private void OnWaveStart(int waveIndex, WaveData waveData)
        {
            _currentWaveData = waveData;

            // Update spawn settings from wave data
            if (waveData != null)
            {
                _spawnInterval = waveData.SpawnInterval;
                _enemiesPerSpawn = waveData.EnemiesPerSpawn;
                _maxEnemies = waveData.MaxEnemies;
            }
        }

        #endregion

        #region Spawning

        private void SpawnEnemies()
        {
            // Check max enemy limit
            Enemy[] existingEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            if (existingEnemies.Length >= _maxEnemies)
                return;

            int enemiesToSpawn = Mathf.Min(_enemiesPerSpawn, _maxEnemies - existingEnemies.Length);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            if (_enemyPrefab == null || _playerTransform == null)
                return;

            Vector3 spawnPos = GetRandomSpawnPosition();
            Enemy enemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);

            // Get enemy type
            EnemyData enemyData = GetEnemyTypeForSpawn();
            if (enemyData != null && _waveManager != null)
            {
                var scaledStats = _waveManager.GetScaledEnemyStats(enemyData);
                enemy.Initialize(enemyData, scaledStats.health, scaledStats.speed, scaledStats.damage);
            }
        }

        private Vector3 GetRandomSpawnPosition()
        {
            // Spawn in a circle around the player, just outside camera view
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 spawnOffset = new Vector3(randomDirection.x, randomDirection.y, 0f) * _spawnDistance;

            return _playerTransform.position + spawnOffset;
        }

        private EnemyData GetEnemyTypeForSpawn()
        {
            // If wave data is available, use it to get random enemy type
            if (_currentWaveData != null)
            {
                return _currentWaveData.GetRandomEnemyType();
            }

            // Otherwise, use configured enemy types
            if (_enemyTypes != null && _enemyTypes.Length > 0)
            {
                return _enemyTypes[Random.Range(0, _enemyTypes.Length)];
            }

            return null;
        }

        #endregion
    }
}
