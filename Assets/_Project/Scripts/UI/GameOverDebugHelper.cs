using UnityEngine;
using CVA.Core;

namespace CVA.UI
{
    /// <summary>
    /// Debug helper to diagnose game over screen issues.
    /// Add this to any GameObject in your scene.
    /// Press 'K' to test game over, 'R' to show debug info.
    /// </summary>
    public class GameOverDebugHelper : MonoBehaviour
    {
        private void Update()
        {
            // Press K to manually trigger game over (for testing)
            if (Input.GetKeyDown(KeyCode.K))
            {
                TestGameOver();
            }

            // Press R to show debug info
            if (Input.GetKeyDown(KeyCode.R))
            {
                ShowDebugInfo();
            }
        }

        private void TestGameOver()
        {
            // Find GameOverUI
            GameOverUI gameOverUI = FindFirstObjectByType<GameOverUI>();

            if (gameOverUI == null)
            {
                return;
            }

            // Trigger game over
            gameOverUI.ShowGameOver();
        }

        private void ShowDebugInfo()
        {
            // Check GameOverUI
            GameOverUI gameOverUI = FindFirstObjectByType<GameOverUI>();

            // Check GameStatsManager
            GameStatsManager statsManager = GameStatsManager.Instance;

            // Check Player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var playerHealth = player.GetComponent<CVA.Player.PlayerHealth>();
            }

            // Check Canvas
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                var gameOverPanel = canvas.transform.Find("GameOverPanel");
            }
        }

        private void OnGUI()
        {
            // Show help text on screen
            GUI.Box(new Rect(10, Screen.height - 100, 300, 90), "");
            GUI.Label(new Rect(20, Screen.height - 90, 280, 30), "<b>GAME OVER DEBUG HELPER</b>");
            GUI.Label(new Rect(20, Screen.height - 70, 280, 20), "Press <b>K</b> to test Game Over");
            GUI.Label(new Rect(20, Screen.height - 50, 280, 20), "Press <b>R</b> for debug info");
            GUI.Label(new Rect(20, Screen.height - 30, 280, 20), "Check Console for results");
        }
    }
}
