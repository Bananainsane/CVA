using UnityEngine;
using UnityEngine.Events;
using CVA.UI;

namespace CVA.Player
{
    /// <summary>
    /// Manages player health and damage.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _currentHealth;

        [Header("Invincibility")]
        [SerializeField] private float _invincibilityDuration = 1f;
        private float _invincibilityTimer = 0f;

        [Header("Events")]
        public UnityEvent<float, float> OnHealthChanged = new UnityEvent<float, float>(); // current, max
        public UnityEvent OnDeath = new UnityEvent();

        // Properties
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        public bool IsAlive => _currentHealth > 0f;

        #region Unity Lifecycle

        private void Start()
        {
            _currentHealth = _maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        private void Update()
        {
            if (_invincibilityTimer > 0f)
            {
                _invincibilityTimer -= Time.deltaTime;
            }
        }

        #endregion

        #region Health Management

        public void TakeDamage(float damage)
        {
            if (_invincibilityTimer > 0f || !IsAlive)
                return;

            _currentHealth = Mathf.Max(0f, _currentHealth - damage);
            _invincibilityTimer = _invincibilityDuration;

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (!IsAlive)
                return;

            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void SetMaxHealth(float newMax)
        {
            float healthPercent = _currentHealth / _maxHealth;
            _maxHealth = newMax;
            _currentHealth = _maxHealth * healthPercent;

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void AddMaxHealth(float amount)
        {
            _maxHealth += amount;
            _currentHealth += amount; // Also heal by the amount added

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        private void Die()
        {
            OnDeath?.Invoke();

            // Show game over screen - search thoroughly including inactive objects
            GameOverUI gameOverUI = FindFirstObjectByType<GameOverUI>(FindObjectsInactive.Include);

            if (gameOverUI == null)
            {
                // Try searching in Canvas
                Canvas canvas = FindFirstObjectByType<Canvas>();
                if (canvas != null)
                {
                    gameOverUI = canvas.GetComponentInChildren<GameOverUI>(true); // true = include inactive
                }
            }

            if (gameOverUI != null)
            {
                gameOverUI.ShowGameOver();
            }
            else
            {
                // Backup: Try ForceGameOverTest on Canvas
                Canvas canvas = FindFirstObjectByType<Canvas>();
                if (canvas != null)
                {
                    ForceGameOverTest forceGO = canvas.GetComponent<ForceGameOverTest>();
                    if (forceGO != null)
                    {
                        forceGO.ShowGameOverDirect();
                        return;
                    }
                }

                Time.timeScale = 0f;
            }
        }

        #endregion
    }
}
