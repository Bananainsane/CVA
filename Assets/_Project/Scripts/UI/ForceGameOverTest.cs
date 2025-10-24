using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace CVA.UI
{
    /// <summary>
    /// EMERGENCY FIX: Add this to Canvas to force game over to work.
    /// This bypasses the GameOverUI script entirely.
    /// </summary>
    public class ForceGameOverTest : MonoBehaviour
    {
        [Header("Direct References")]
        [SerializeField] private GameObject gameOverPanel;

        private void Update()
        {
            // Press O to test (New Input System)
            if (Keyboard.current != null && Keyboard.current.oKey.wasPressedThisFrame)
            {
                if (gameOverPanel != null)
                {
                    gameOverPanel.SetActive(true);
                    Time.timeScale = 0f;
                }
            }
        }

        /// <summary>
        /// Call this directly when player dies as emergency backup.
        /// </summary>
        public void ShowGameOverDirect()
        {
            if (gameOverPanel != null)
            {
                // Update stats if possible
                var statsManager = CVA.Core.GameStatsManager.Instance;
                if (statsManager != null)
                {
                    var survivalText = gameOverPanel.transform.Find("Container/SurvivalTimeText")?.GetComponent<TMPro.TextMeshProUGUI>();
                    var killsText = gameOverPanel.transform.Find("Container/EnemiesKilledText")?.GetComponent<TMPro.TextMeshProUGUI>();
                    var waveText = gameOverPanel.transform.Find("Container/WaveReachedText")?.GetComponent<TMPro.TextMeshProUGUI>();

                    if (survivalText != null) survivalText.text = $"Survival Time: {statsManager.GetFormattedSurvivalTime()}";
                    if (killsText != null) killsText.text = $"Enemies Killed: {statsManager.EnemiesKilled}";
                    if (waveText != null) waveText.text = $"Wave Reached: {statsManager.HighestWaveReached} (Loop {statsManager.HighestLoopReached})";
                }

                gameOverPanel.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }
}
