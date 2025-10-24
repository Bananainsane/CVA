using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CVA.Player;

namespace CVA.UI
{
    /// <summary>
    /// Displays player health with a fill bar.
    /// </summary>
    public class HealthBar : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _healthText;

        [Header("Player Reference")]
        [SerializeField] private PlayerHealth _playerHealth;
        [SerializeField] private bool _autoFindPlayer = true;

        #region Unity Lifecycle

        private void Start()
        {
            if (_autoFindPlayer && _playerHealth == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    _playerHealth = player.GetComponent<PlayerHealth>();
                }
            }

            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged.AddListener(UpdateHealthDisplay);
                UpdateHealthDisplay(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
            }
        }

        private void OnDestroy()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged.RemoveListener(UpdateHealthDisplay);
            }
        }

        #endregion

        #region UI Update

        private void UpdateHealthDisplay(float currentHealth, float maxHealth)
        {
            // Update fill bar
            if (_fillImage != null)
            {
                _fillImage.fillAmount = currentHealth / maxHealth;
            }

            // Update text
            if (_healthText != null)
            {
                _healthText.text = $"{currentHealth:F0}/{maxHealth:F0}";
            }
        }

        #endregion
    }
}
