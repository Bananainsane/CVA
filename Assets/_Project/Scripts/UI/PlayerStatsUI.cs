using UnityEngine;
using TMPro;
using CVA.Player;
using CVA.Combat;
using CVA.Core;

namespace CVA.UI
{
    /// <summary>
    /// Displays player stats in bottom-left corner for debugging/monitoring powerup effects.
    /// </summary>
    public class PlayerStatsUI : MonoBehaviour
    {
        [Header("UI Reference")]
        [SerializeField] private TextMeshProUGUI _statsText;

        [Header("Update Rate")]
        [SerializeField] private float _updateInterval = 0.2f;

        // Component references
        private PlayerController _playerController;
        private PowerupManager _powerupManager;
        private ProjectileWeapon _weapon;
        private PlayerHealth _playerHealth;
        private ExperienceManager _experienceManager;
        private GameStatsManager _statsManager;

        private float _updateTimer;

        #region Unity Lifecycle

        private void Start()
        {
            FindPlayerComponents();
        }

        private void Update()
        {
            _updateTimer += Time.deltaTime;

            if (_updateTimer >= _updateInterval)
            {
                _updateTimer = 0f;
                UpdateStatsDisplay();
            }
        }

        #endregion

        #region Initialization

        private void FindPlayerComponents()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerController = player.GetComponent<PlayerController>();
                _powerupManager = player.GetComponent<PowerupManager>();
                _weapon = player.GetComponentInChildren<ProjectileWeapon>();
                _playerHealth = player.GetComponent<PlayerHealth>();
            }

            _experienceManager = FindFirstObjectByType<ExperienceManager>();
            _statsManager = GameStatsManager.Instance;
        }

        #endregion

        #region Stats Display

        private void UpdateStatsDisplay()
        {
            if (_statsText == null)
                return;

            string stats = "<b><color=#FFD700>PLAYER STATS</color></b>\n";
            stats += "─────────────────\n";

            // Game Stats (Kills & Time)
            if (_statsManager != null)
            {
                stats += $"<color=#FF6666>Kills:</color> {_statsManager.EnemiesKilled}\n";
                stats += $"<color=#66FFFF>Time:</color> {_statsManager.GetFormattedSurvivalTime()}\n";
                stats += $"<color=#FFAA00>Wave:</color> {_statsManager.HighestWaveReached} (Loop {_statsManager.HighestLoopReached})\n";
            }

            stats += "\n";

            // Level & XP
            if (_experienceManager != null)
            {
                stats += $"<color=#FFFF00>Level:</color> {_experienceManager.CurrentLevel}\n";
                stats += $"<color=#00FFFF>XP:</color> {_experienceManager.CurrentXP}/{_experienceManager.XPRequired}\n";
            }

            stats += "\n";

            // Health
            if (_playerHealth != null)
            {
                stats += $"<color=#FF4444>Health:</color> {_playerHealth.CurrentHealth}/{_playerHealth.MaxHealth}\n";
            }

            stats += "\n";

            // Movement
            if (_playerController != null)
            {
                stats += $"<color=#44FF44>Move Speed:</color> {_playerController.GetMoveSpeed():F1}\n";
            }

            stats += "\n";

            // Weapon Stats
            if (_weapon != null)
            {
                stats += $"<color=#FF8844>Damage:</color> {_weapon.CurrentDamage:F1}\n";
                stats += $"<color=#FF8844>Fire Rate:</color> {_weapon.CurrentFireRate:F2}\n";
                stats += $"<color=#FF8844>Projectile Speed:</color> {_weapon.ProjectileSpeed:F1}\n";

                // Weapon Upgrades
                stats += "\n<b><color=#FFA500>WEAPON UPGRADES</color></b>\n";
                stats += $"<color=#FFA500>Projectiles:</color> {1 + _weapon.AdditionalProjectiles}\n";
                stats += $"<color=#FFA500>Pierce:</color> {_weapon.PierceCount}\n";
                stats += $"<color=#FFA500>Bounce:</color> {_weapon.BounceCount}\n";
                stats += $"<color=#FFA500>Chain:</color> {_weapon.ChainCount}\n";
            }

            stats += "\n";

            // Powerup Stats
            if (_powerupManager != null)
            {
                stats += $"<color=#FF44FF>Magnet Range:</color> {_powerupManager.MagnetRange:F1}\n";
                stats += $"<color=#44FFFF>Active Powerups:</color> {_powerupManager.ActivePowerupCount}\n";
            }

            _statsText.text = stats;
        }

        #endregion
    }
}
