using UnityEngine;

namespace CVA.Core
{
    /// <summary>
    /// Tracks game statistics like kills, survival time, etc.
    /// Singleton pattern for easy access.
    /// </summary>
    public class GameStatsManager : MonoBehaviour
    {
        public static GameStatsManager Instance { get; private set; }

        [Header("Stats")]
        private int _enemiesKilled = 0;
        private float _survivalTime = 0f;
        private int _highestWaveReached = 1;
        private int _highestLoopReached = 1;

        // Public properties
        public int EnemiesKilled => _enemiesKilled;
        public float SurvivalTime => _survivalTime;
        public int HighestWaveReached => _highestWaveReached;
        public int HighestLoopReached => _highestLoopReached;

        #region Unity Lifecycle

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            // Track survival time
            _survivalTime += Time.deltaTime;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Call when an enemy is killed.
        /// </summary>
        public void RecordKill()
        {
            _enemiesKilled++;
        }

        /// <summary>
        /// Update the highest wave reached.
        /// </summary>
        public void SetWaveReached(int waveIndex, int loopIteration)
        {
            _highestWaveReached = Mathf.Max(_highestWaveReached, waveIndex + 1);
            _highestLoopReached = Mathf.Max(_highestLoopReached, loopIteration + 1);
        }

        /// <summary>
        /// Reset all stats (for new game).
        /// </summary>
        public void ResetStats()
        {
            _enemiesKilled = 0;
            _survivalTime = 0f;
            _highestWaveReached = 1;
            _highestLoopReached = 1;
        }

        /// <summary>
        /// Get formatted survival time string (MM:SS).
        /// </summary>
        public string GetFormattedSurvivalTime()
        {
            int minutes = Mathf.FloorToInt(_survivalTime / 60f);
            int seconds = Mathf.FloorToInt(_survivalTime % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        #endregion
    }
}
