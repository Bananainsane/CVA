using UnityEngine;
using CVA.Data;
using CVA.Player;
using CVA.Core;

namespace CVA.Core
{
    /// <summary>
    /// XP Orb that drops from enemies.
    /// Automatically moves toward player when in range (magnet effect).
    /// Grants XP on collection.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class XPOrb : MonoBehaviour, IPoolable
    {
        [Header("Orb Data")]
        [SerializeField] private XPOrbData _orbData;

        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 8f;
        [SerializeField] private float _accelerationCurve = 2f;
        [SerializeField] private float _autoCollectDelay = 5f;

        [Header("Visual")]
        [SerializeField] private float _bobSpeed = 2f;
        [SerializeField] private float _bobHeight = 0.1f;

        // Component references
        private SpriteRenderer _spriteRenderer;
        private Transform _playerTransform;
        private ExperienceManager _experienceManager;
        private PowerupManager _powerupManager;

        // State
        private bool _isMovingToPlayer;
        private float _spawnTime;
        private float _startY;

        #region Unity Lifecycle

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            FindPlayerAndManagers();
        }

        private void Update()
        {
            if (_playerTransform == null)
                return;

            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
            float magnetRange = _powerupManager != null ? _powerupManager.MagnetRange : 2f;

            // Auto-collect after delay
            if (Time.time - _spawnTime > _autoCollectDelay)
            {
                _isMovingToPlayer = true;
            }
            // Magnet pull
            else if (distanceToPlayer < magnetRange)
            {
                _isMovingToPlayer = true;
            }

            if (_isMovingToPlayer)
            {
                MoveTowardPlayer();
            }
            else
            {
                // Idle bobbing animation
                float newY = _startY + Mathf.Sin(Time.time * _bobSpeed) * _bobHeight;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                CollectOrb();
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize orb with data and position.
        /// </summary>
        public void Initialize(XPOrbData orbData, Vector3 position)
        {
            _orbData = orbData;
            transform.position = position;
            _spawnTime = Time.time;
            _startY = position.y;
            _isMovingToPlayer = false;

            ApplyVisuals();
            FindPlayerAndManagers();
        }

        private void ApplyVisuals()
        {
            if (_orbData != null && _spriteRenderer != null)
            {
                _spriteRenderer.color = _orbData.OrbColor;
                transform.localScale = Vector3.one * _orbData.OrbSize;
            }
        }

        private void FindPlayerAndManagers()
        {
            if (_playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    _playerTransform = player.transform;
                    _powerupManager = player.GetComponent<PowerupManager>();
                }
            }

            if (_experienceManager == null)
            {
                _experienceManager = FindFirstObjectByType<ExperienceManager>();
            }
        }

        #endregion

        #region Movement

        private void MoveTowardPlayer()
        {
            if (_playerTransform == null)
                return;

            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            float speedMultiplier = Mathf.Pow(1f / Mathf.Max(distance, 0.1f), _accelerationCurve);

            Vector3 direction = (_playerTransform.position - transform.position).normalized;
            transform.position += direction * _moveSpeed * speedMultiplier * Time.deltaTime;
        }

        #endregion

        #region Collection

        private void CollectOrb()
        {
            if (_orbData == null)
            {
                Destroy(gameObject);
                return;
            }

            if (_experienceManager == null)
            {
                Destroy(gameObject);
                return;
            }

            _experienceManager.AddXP(_orbData.XPValue);

            // Return to pool or destroy
            Destroy(gameObject);
        }

        #endregion

        #region IPoolable Implementation

        public void OnSpawnedFromPool()
        {
            _isMovingToPlayer = false;
            _spawnTime = Time.time;
            transform.rotation = Quaternion.identity;
        }

        public void OnReturnedToPool()
        {
            _isMovingToPlayer = false;
        }

        #endregion
    }
}
