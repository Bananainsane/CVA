using UnityEngine;
using UnityEngine.Events;
using CVA.Data;

namespace CVA.Core
{
    /// <summary>
    /// Manages wave progression in Vampire Survivors style.
    /// Tracks survival time, loops through waves, increases difficulty each loop.
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        [Header("Wave Configuration")]
        [SerializeField] private WaveData[] _waves;
        [SerializeField] private bool _loopWaves = true;

        [Header("Difficulty Scaling Per Loop")]
        [Tooltip("Multiply enemy health by this each loop (1.5 = +50% per loop)")]
        [SerializeField] private float _healthScalePerLoop = 1.5f;

        [Tooltip("Multiply enemy damage by this each loop")]
        [SerializeField] private float _damageScalePerLoop = 1.3f;

        [Tooltip("Multiply enemy speed by this each loop")]
        [SerializeField] private float _speedScalePerLoop = 1.2f;

        [Header("Events")]
        public UnityEvent<int, WaveData> OnWaveStart = new UnityEvent<int, WaveData>();
        public UnityEvent<int> OnWaveComplete = new UnityEvent<int>();
        public UnityEvent<int> OnLoopComplete = new UnityEvent<int>();

        // State
        private int _currentWaveIndex = 0;
        private int _loopIteration = 0;
        private float _currentWaveTimer = 0f;
        private float _totalSurvivalTime = 0f;
        private bool _waveActive = false;

        // Properties
        public int CurrentWaveIndex => _currentWaveIndex;
        public int CurrentLoop => _loopIteration;
        public float CurrentWaveTimer => _currentWaveTimer;
        public float TotalSurvivalTime => _totalSurvivalTime;
        public WaveData CurrentWave => _waves != null && _currentWaveIndex < _waves.Length
            ? _waves[_currentWaveIndex] : null;

        #region Unity Lifecycle

        private void Start()
        {
            if (_waves == null || _waves.Length == 0)
            {
                enabled = false;
                return;
            }

            StartCurrentWave();
        }

        private void Update()
        {
            if (!_waveActive)
                return;

            _currentWaveTimer += Time.deltaTime;
            _totalSurvivalTime += Time.deltaTime;

            // Check if wave is complete
            if (CurrentWave != null && _currentWaveTimer >= CurrentWave.WaveDuration)
            {
                CompleteCurrentWave();
            }
        }

        #endregion

        #region Wave Control

        private void StartCurrentWave()
        {
            if (_waves == null || _currentWaveIndex >= _waves.Length)
                return;

            WaveData wave = _waves[_currentWaveIndex];
            _currentWaveTimer = 0f;
            _waveActive = true;

            // Update stats tracker
            if (GameStatsManager.Instance != null)
            {
                GameStatsManager.Instance.SetWaveReached(_currentWaveIndex, _loopIteration);
            }

            OnWaveStart?.Invoke(_currentWaveIndex, wave);
        }

        public void CompleteCurrentWave()
        {
            _waveActive = false;

            OnWaveComplete?.Invoke(_currentWaveIndex);

            // Advance to next wave
            _currentWaveIndex++;

            // Check if we completed all waves (loop)
            if (_currentWaveIndex >= _waves.Length)
            {
                if (_loopWaves)
                {
                    _loopIteration++;
                    _currentWaveIndex = 0;
                    OnLoopComplete?.Invoke(_loopIteration);
                }
                else
                {
                    return;
                }
            }

            // Start next wave
            StartCurrentWave();
        }

        #endregion

        #region Difficulty Scaling

        /// <summary>
        /// Get the current health multiplier based on loop iteration.
        /// </summary>
        public float GetHealthMultiplier()
        {
            return Mathf.Pow(_healthScalePerLoop, _loopIteration);
        }

        /// <summary>
        /// Get the current damage multiplier based on loop iteration.
        /// </summary>
        public float GetDamageMultiplier()
        {
            return Mathf.Pow(_damageScalePerLoop, _loopIteration);
        }

        /// <summary>
        /// Get the current speed multiplier based on loop iteration.
        /// </summary>
        public float GetSpeedMultiplier()
        {
            return Mathf.Pow(_speedScalePerLoop, _loopIteration);
        }

        /// <summary>
        /// Get scaled enemy stats for current difficulty.
        /// </summary>
        public (float health, float damage, float speed) GetScaledEnemyStats(EnemyData baseData)
        {
            if (baseData == null)
                return (0, 0, 0);

            float health = baseData.MaxHealth * GetHealthMultiplier();
            float damage = baseData.ContactDamage * GetDamageMultiplier();
            float speed = baseData.MoveSpeed * GetSpeedMultiplier();

            return (health, damage, speed);
        }

        #endregion

        #region Debug

        private void OnGUI()
        {
            if (!Application.isPlaying)
                return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 150));
            GUILayout.Label($"<b>Wave:</b> {_currentWaveIndex + 1}/{_waves.Length}");
            GUILayout.Label($"<b>Loop:</b> {_loopIteration + 1}");
            GUILayout.Label($"<b>Time:</b> {_currentWaveTimer:F1}s / {(CurrentWave?.WaveDuration ?? 0):F0}s");
            GUILayout.Label($"<b>Total Survival:</b> {_totalSurvivalTime:F1}s");
            GUILayout.Label($"<b>Difficulty:</b> HP x{GetHealthMultiplier():F2}");
            GUILayout.EndArea();
        }

        #endregion
    }
}
