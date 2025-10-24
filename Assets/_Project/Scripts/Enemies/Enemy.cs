using UnityEngine;
using CVA.Data;
using CVA.Core;
using CVA.Player;
using CVA.UI;

namespace CVA.Enemies
{
    /// <summary>
    /// Basic enemy that chases the player and deals contact damage.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private EnemyData _enemyData;

        [Header("XP Orb Spawning")]
        [SerializeField] private GameObject _xpOrbPrefab;
        [SerializeField] private int _orbsToSpawn = 0; // 0 = auto-calculate
        [SerializeField] private float _orbSpawnRadius = 0.5f;

        [Header("References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        // Runtime state
        private float _currentHealth;
        private Transform _playerTransform;
        private Rigidbody2D _rb;
        private float _damageContactInterval = 1f;
        private float _lastDamageTime = 0f;

        // Stats (can be scaled by WaveManager)
        private float _maxHealth;
        private float _moveSpeed;
        private float _contactDamage;

        // Health bar
        private EnemyHealthBar _healthBar;

        // Public properties
        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;

        #region Unity Lifecycle

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();

            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            FindPlayer();
            InitializeFromData();
            CreateHealthBar();
        }

        private void CreateHealthBar()
        {
            _healthBar = gameObject.AddComponent<EnemyHealthBar>();
            if (_healthBar != null)
            {
                _healthBar.UpdateHealth(_currentHealth, _maxHealth);
            }
        }

        private void Update()
        {
            ChasePlayer();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                DamagePlayer(collision);
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

        private void InitializeFromData()
        {
            if (_enemyData == null)
            {
                return;
            }

            // Check if WaveManager exists for scaling
            WaveManager waveManager = FindFirstObjectByType<WaveManager>();
            if (waveManager != null)
            {
                var scaledStats = waveManager.GetScaledEnemyStats(_enemyData);
                _maxHealth = scaledStats.health;
                _moveSpeed = scaledStats.speed;
                _contactDamage = scaledStats.damage;
            }
            else
            {
                _maxHealth = _enemyData.MaxHealth;
                _moveSpeed = _enemyData.MoveSpeed;
                _contactDamage = _enemyData.ContactDamage;
            }

            _currentHealth = _maxHealth;

            // Apply visuals
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _enemyData.enemyColor;
            }

            transform.localScale = Vector3.one * _enemyData.sizeScale;
        }

        /// <summary>
        /// Initialize with custom stats (for wave scaling).
        /// </summary>
        public void Initialize(EnemyData data, float health, float speed, float damage)
        {
            _enemyData = data;
            _maxHealth = health;
            _moveSpeed = speed;
            _contactDamage = damage;
            _currentHealth = _maxHealth;

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = data.enemyColor;
            }

            transform.localScale = Vector3.one * data.sizeScale;
        }

        #endregion

        #region Movement

        private void ChasePlayer()
        {
            if (_playerTransform == null)
                return;

            Vector2 direction = (_playerTransform.position - transform.position).normalized;
            _rb.linearVelocity = direction * _moveSpeed;
        }

        #endregion

        #region Combat

        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;

            // Update health bar
            if (_healthBar != null)
            {
                _healthBar.UpdateHealth(_currentHealth, _maxHealth);
            }

            // Spawn damage number
            DamageNumber.Create(transform.position + Vector3.up * 0.3f, damage);

            // Hit flash
            if (_spriteRenderer != null)
            {
                StartCoroutine(HitFlash());
            }

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        private System.Collections.IEnumerator HitFlash()
        {
            Color original = _spriteRenderer.color;
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            _spriteRenderer.color = original;
        }

        private void DamagePlayer(Collider2D playerCollider)
        {
            if (Time.time - _lastDamageTime < _damageContactInterval)
                return;

            PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(_contactDamage);
                _lastDamageTime = Time.time;
            }
        }

        #endregion

        #region Death

        private void Die()
        {
            // Record kill stat
            if (GameStatsManager.Instance != null)
            {
                GameStatsManager.Instance.RecordKill();
            }

            // Award XP (spawn orbs)
            AwardXP();

            // VFX
            VFXManager.Death(transform.position, _enemyData.enemyColor);

            Destroy(gameObject);
        }

        private void AwardXP()
        {
            ExperienceManager expManager = FindFirstObjectByType<ExperienceManager>();
            if (expManager == null)
                return;

            // Spawn XP orbs if prefab is assigned
            if (_xpOrbPrefab != null)
            {
                int orbCount = _orbsToSpawn > 0 ? _orbsToSpawn : Mathf.Max(1, _enemyData.XPValue / 5);

                for (int i = 0; i < orbCount; i++)
                {
                    SpawnXPOrb();
                }
            }
            else
            {
                // Fallback: give instant XP
                expManager.AddXP(_enemyData.XPValue);
            }
        }

        private void SpawnXPOrb()
        {
            Vector2 randomOffset = Random.insideUnitCircle * _orbSpawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

            GameObject orbGO = Instantiate(_xpOrbPrefab, spawnPos, Quaternion.identity);

            // Initialize orb with data
            XPOrb orb = orbGO.GetComponent<XPOrb>();
            if (orb != null)
            {
                XPOrbData orbData = _enemyData.GetXPOrbData();
                if (orbData != null)
                {
                    orb.Initialize(orbData, spawnPos);
                }
                else
                {
                    // Fallback: give instant XP if no orb data
                    ExperienceManager expManager = FindFirstObjectByType<ExperienceManager>();
                    if (expManager != null)
                    {
                        expManager.AddXP(_enemyData.XPValue);
                    }
                    Destroy(orbGO);
                }
            }
        }

        #endregion
    }
}
