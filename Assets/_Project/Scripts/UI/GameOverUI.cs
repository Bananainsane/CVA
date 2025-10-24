using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using CVA.Core;

namespace CVA.UI
{
    /// <summary>
    /// Game Over screen that displays stats and provides restart/quit options.
    /// Updated: 2025-10-24 - Added highscore system integration
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _gameOverPanel;
        [SerializeField] private TextMeshProUGUI _survivalTimeText;
        [SerializeField] private TextMeshProUGUI _enemiesKilledText;
        [SerializeField] private TextMeshProUGUI _waveReachedText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _quitButton;

        [Header("Highscore Display")]
        [SerializeField] private GameObject _newHighscoreIndicator;
        [SerializeField] private TextMeshProUGUI _highscoreTimeText;
        [SerializeField] private TextMeshProUGUI _highscoreKillsText;
        [SerializeField] private TextMeshProUGUI _highscoreWaveText;

        [Header("Optional")]
        [SerializeField] private string _mainMenuSceneName = "MainMenu";

        #region Unity Lifecycle

        private bool _hasInitialized = false;

        private void Awake()
        {
            // Setup button listeners ONCE
            if (_restartButton != null)
            {
                _restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (_quitButton != null)
            {
                _quitButton.onClick.AddListener(OnQuitClicked);
            }

            _hasInitialized = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Show the game over screen with current stats.
        /// </summary>
        public void ShowGameOver()
        {
            if (_gameOverPanel == null)
            {
                Time.timeScale = 0f;
                return;
            }

            // Get stats from GameStatsManager
            GameStatsManager stats = GameStatsManager.Instance;

            // Check and save highscore
            if (HighscoreManager.Instance != null)
            {
                HighscoreManager.Instance.CheckAndSaveHighscore();
            }

            // Update current run stats display
            if (_survivalTimeText != null)
            {
                string timeStr = stats != null ? stats.GetFormattedSurvivalTime() : "00:00";
                _survivalTimeText.text = $"Survival Time: {timeStr}";
            }

            if (_enemiesKilledText != null)
            {
                int kills = stats != null ? stats.EnemiesKilled : 0;
                _enemiesKilledText.text = $"Enemies Killed: {kills}";
            }

            if (_waveReachedText != null)
            {
                if (stats != null)
                {
                    int wave = stats.HighestWaveReached;
                    int loop = stats.HighestLoopReached;
                    _waveReachedText.text = $"Wave Reached: {wave} (Loop {loop})";
                }
                else
                {
                    _waveReachedText.text = "Wave Reached: 1 (Loop 1)";
                }
            }

            // Display highscore information
            DisplayHighscores();

            // Show panel
            _gameOverPanel.SetActive(true);

            // Pause game
            Time.timeScale = 0f;
        }

        /// <summary>
        /// Displays highscore information and checks if player achieved a new high score.
        /// </summary>
        private void DisplayHighscores()
        {
            if (HighscoreManager.Instance == null)
                return;

            // Show/hide "NEW HIGH SCORE!" indicator
            bool isNewHighscore = HighscoreManager.Instance.IsNewHighscore;
            if (_newHighscoreIndicator != null)
            {
                _newHighscoreIndicator.SetActive(isNewHighscore);
            }

            // Display high score survival time
            if (_highscoreTimeText != null)
            {
                string highscoreTime = HighscoreManager.Instance.GetFormattedHighscoreTime();
                _highscoreTimeText.text = $"Best Time: {highscoreTime}";
            }

            // Display high score kills
            if (_highscoreKillsText != null)
            {
                int highscoreKills = HighscoreManager.Instance.GetHighscoreKills();
                _highscoreKillsText.text = $"Most Kills: {highscoreKills}";
            }

            // Display high score wave and loop
            if (_highscoreWaveText != null)
            {
                int highscoreWave = HighscoreManager.Instance.GetHighscoreWave();
                int highscoreLoop = HighscoreManager.Instance.GetHighscoreLoop();
                _highscoreWaveText.text = $"Highest: Wave {highscoreWave} (Loop {highscoreLoop})";
            }
        }

        #endregion

        #region Button Handlers

        private void OnRestartClicked()
        {
            // Resume time
            Time.timeScale = 1f;

            // Reset stats
            if (GameStatsManager.Instance != null)
            {
                GameStatsManager.Instance.ResetStats();
            }

            // Reload current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnQuitClicked()
        {
            // Resume time
            Time.timeScale = 1f;

            // Try to load main menu, otherwise quit application
            if (!string.IsNullOrEmpty(_mainMenuSceneName))
            {
                // Check if scene exists in build settings
                if (Application.CanStreamedLevelBeLoaded(_mainMenuSceneName))
                {
                    SceneManager.LoadScene(_mainMenuSceneName);
                    return;
                }
            }

            // If no main menu, just quit the application
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion
    }
}
