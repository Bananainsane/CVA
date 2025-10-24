using UnityEngine;

namespace CVA.Core
{
    /// <summary>
    /// Manages high scores across game sessions using PlayerPrefs.
    /// Tracks best survival time, most kills, highest wave, and highest loop.
    /// Singleton pattern ensures persistence throughout game lifecycle.
    /// </summary>
    public class HighscoreManager : MonoBehaviour
    {
        public static HighscoreManager Instance { get; private set; }

        #region PlayerPrefs Keys

        private const string KEY_SURVIVAL_TIME = "Highscore_SurvivalTime";
        private const string KEY_KILLS = "Highscore_Kills";
        private const string KEY_WAVE = "Highscore_Wave";
        private const string KEY_LOOP = "Highscore_Loop";

        #endregion

        #region Private Fields

        private float _highscoreSurvivalTime = 0f;
        private int _highscoreKills = 0;
        private int _highscoreWave = 1;
        private int _highscoreLoop = 1;
        private bool _isNewHighscore = false;

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns true if the last CheckAndSaveHighscore() call resulted in a new high score.
        /// </summary>
        public bool IsNewHighscore => _isNewHighscore;

        #endregion

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
            DontDestroyOnLoad(gameObject);

            LoadHighscores();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Compares current run stats with saved high scores and updates if better.
        /// Call this when the game ends.
        /// </summary>
        public void CheckAndSaveHighscore()
        {
            _isNewHighscore = false;

            if (GameStatsManager.Instance == null)
                return;

            GameStatsManager stats = GameStatsManager.Instance;

            // Check if current survival time beats high score (primary metric)
            if (stats.SurvivalTime > _highscoreSurvivalTime)
            {
                _highscoreSurvivalTime = stats.SurvivalTime;
                PlayerPrefs.SetFloat(KEY_SURVIVAL_TIME, _highscoreSurvivalTime);
                _isNewHighscore = true;
            }

            // Update individual stat records
            if (stats.EnemiesKilled > _highscoreKills)
            {
                _highscoreKills = stats.EnemiesKilled;
                PlayerPrefs.SetInt(KEY_KILLS, _highscoreKills);
            }

            if (stats.HighestWaveReached > _highscoreWave)
            {
                _highscoreWave = stats.HighestWaveReached;
                PlayerPrefs.SetInt(KEY_WAVE, _highscoreWave);
            }

            if (stats.HighestLoopReached > _highscoreLoop)
            {
                _highscoreLoop = stats.HighestLoopReached;
                PlayerPrefs.SetInt(KEY_LOOP, _highscoreLoop);
            }

            // Save changes to disk
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Gets the best survival time ever achieved.
        /// </summary>
        public float GetHighscoreSurvivalTime()
        {
            return _highscoreSurvivalTime;
        }

        /// <summary>
        /// Gets the most kills achieved in a single run.
        /// </summary>
        public int GetHighscoreKills()
        {
            return _highscoreKills;
        }

        /// <summary>
        /// Gets the highest wave ever reached.
        /// </summary>
        public int GetHighscoreWave()
        {
            return _highscoreWave;
        }

        /// <summary>
        /// Gets the highest loop ever reached.
        /// </summary>
        public int GetHighscoreLoop()
        {
            return _highscoreLoop;
        }

        /// <summary>
        /// Get formatted high score survival time string (MM:SS).
        /// </summary>
        public string GetFormattedHighscoreTime()
        {
            int minutes = Mathf.FloorToInt(_highscoreSurvivalTime / 60f);
            int seconds = Mathf.FloorToInt(_highscoreSurvivalTime % 60f);
            return $"{minutes:00}:{seconds:00}";
        }

        /// <summary>
        /// Resets all high scores to default values.
        /// Use this for testing or when implementing a "Reset Progress" feature.
        /// </summary>
        [ContextMenu("Reset Highscores")]
        public void ResetHighscores()
        {
            PlayerPrefs.DeleteKey(KEY_SURVIVAL_TIME);
            PlayerPrefs.DeleteKey(KEY_KILLS);
            PlayerPrefs.DeleteKey(KEY_WAVE);
            PlayerPrefs.DeleteKey(KEY_LOOP);
            PlayerPrefs.Save();

            LoadHighscores();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads high scores from PlayerPrefs.
        /// </summary>
        private void LoadHighscores()
        {
            _highscoreSurvivalTime = PlayerPrefs.GetFloat(KEY_SURVIVAL_TIME, 0f);
            _highscoreKills = PlayerPrefs.GetInt(KEY_KILLS, 0);
            _highscoreWave = PlayerPrefs.GetInt(KEY_WAVE, 1);
            _highscoreLoop = PlayerPrefs.GetInt(KEY_LOOP, 1);
            _isNewHighscore = false;
        }

        #endregion
    }
}
