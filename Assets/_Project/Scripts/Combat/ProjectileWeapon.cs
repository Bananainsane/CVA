using UnityEngine;
using CVA.Data;
using CVA.Enemies;
using CVA.Player;

namespace CVA.Combat
{
    /// <summary>
    /// Basic projectile weapon that shoots bullets at nearest enemy.
    /// Auto-aims and fires automatically.
    /// </summary>
    public class ProjectileWeapon : MonoBehaviour
    {
        [Header("Weapon Data")]
        [SerializeField] private WeaponData _weaponData;

        [Header("Projectile")]
        [SerializeField] private GameObject _projectilePrefab;

        [Header("Auto-Aim")]
        [SerializeField] private float _aimRange = 15f;

        // Runtime
        private Transform _playerTransform;
        private PowerupManager _powerupManager;
        private float _fireTimer = 0f;
        private float _baseDamage;
        private float _baseFireRate;
        private float _baseProjectileSpeed;

        // Weapon Upgrades
        private int _pierceCount = 0; // How many enemies projectile can pierce
        private int _bounceCount = 0; // How many times projectile can bounce
        private int _chainCount = 0; // How many enemies to chain to

        // Public properties for UI (with multipliers applied)
        public float CurrentDamage => _baseDamage * (_powerupManager != null ? _powerupManager.DamageMultiplier : 1f);
        public float CurrentFireRate => _baseFireRate * (_powerupManager != null ? _powerupManager.FireRateMultiplier : 1f);
        public float ProjectileSpeed => _baseProjectileSpeed * (_powerupManager != null ? _powerupManager.ProjectileSpeedMultiplier : 1f);
        public int AdditionalProjectiles => _powerupManager != null ? _powerupManager.AdditionalProjectiles : 0;
        public int PierceCount => _pierceCount;
        public int BounceCount => _bounceCount;
        public int ChainCount => _chainCount;

        #region Unity Lifecycle

        private void Start()
        {
            FindPlayer();
            InitializeWeapon();
        }

        private void Update()
        {
            if (_playerTransform == null)
                return;

            _fireTimer += Time.deltaTime;

            // Use CurrentFireRate property (includes multipliers)
            if (_fireTimer >= 1f / CurrentFireRate)
            {
                TryFire();
                _fireTimer = 0f;
            }
        }

        #endregion

        #region Initialization

        private void FindPlayer()
        {
            // This weapon is attached to the player, so use transform directly
            _playerTransform = transform;

            // Find PowerupManager on player
            _powerupManager = GetComponentInParent<PowerupManager>();
        }

        private void InitializeWeapon()
        {
            if (_weaponData != null)
            {
                _baseDamage = _weaponData.damage;
                _baseFireRate = _weaponData.fireRate;
                _baseProjectileSpeed = _weaponData.projectileSpeed;
            }
            else
            {
                // Default values
                _baseDamage = 10f;
                _baseFireRate = 2f;
                _baseProjectileSpeed = 15f;
            }
        }

        #endregion

        #region Firing

        private void TryFire()
        {
            if (_projectilePrefab == null)
            {
                return;
            }

            if (_playerTransform == null)
            {
                return;
            }

            // Find nearest enemy
            Enemy nearestEnemy = FindNearestEnemy();
            if (nearestEnemy == null)
            {
                return;
            }

            // Calculate direction
            Vector2 direction = (nearestEnemy.transform.position - _playerTransform.position).normalized;

            // Spawn projectiles (base + additional from global powerup)
            int totalProjectiles = 1 + AdditionalProjectiles;

            if (totalProjectiles == 1)
            {
                // Single projectile - fire straight
                SpawnProjectile(direction);
            }
            else
            {
                // Multiple projectiles - spread in a fan pattern
                float spreadAngle = 15f; // Degrees between projectiles
                float startAngle = -(totalProjectiles - 1) * spreadAngle / 2f;

                for (int i = 0; i < totalProjectiles; i++)
                {
                    float currentAngle = startAngle + (i * spreadAngle);
                    Vector2 rotatedDirection = RotateVector(direction, currentAngle);
                    SpawnProjectile(rotatedDirection);
                }
            }
        }

        private Enemy FindNearestEnemy()
        {
            Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            Enemy nearest = null;
            float minDistance = _aimRange;

            foreach (Enemy enemy in enemies)
            {
                float distance = Vector2.Distance(_playerTransform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        private void SpawnProjectile(Vector2 direction)
        {
            GameObject projectileGO = Instantiate(_projectilePrefab, _playerTransform.position, Quaternion.identity);

            // Rotate projectile to face direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectileGO.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

            // Initialize projectile (use properties to include multipliers)
            Projectile projectile = projectileGO.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(direction, CurrentDamage, ProjectileSpeed, _pierceCount, _bounceCount);
            }
        }

        private Vector2 RotateVector(Vector2 vector, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            return new Vector2(
                vector.x * cos - vector.y * sin,
                vector.x * sin + vector.y * cos
            );
        }

        #endregion

        #region Upgrade System

        public void IncreaseDamage(float amount)
        {
            _baseDamage += amount;
        }

        public void IncreaseFireRate(float amount)
        {
            _baseFireRate += amount;
        }

        public void IncreaseProjectileSpeed(float amount)
        {
            _baseProjectileSpeed += amount;
        }

        public void AddPierce(int count = 1)
        {
            _pierceCount += count;
        }

        public void AddBounce(int count = 1)
        {
            _bounceCount += count;
        }

        public void AddChain(int count = 1)
        {
            _chainCount += count;
        }

        #endregion
    }
}
