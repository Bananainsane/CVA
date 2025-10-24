using UnityEngine;
using CVA.Enemies;
using System.Collections.Generic;

namespace CVA.Combat
{
    /// <summary>
    /// Projectile that moves forward and damages enemies.
    /// Supports pierce (goes through enemies) and bounce (bounces off screen edges).
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float _speed = 15f;
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _lifetime = 3f;

        private Vector2 _direction;
        private float _spawnTime;

        // Upgrades
        private int _pierceRemaining = 0; // How many enemies left to pierce
        private int _bounceRemaining = 0; // How many bounces left
        private HashSet<int> _hitEnemies = new HashSet<int>(); // Track hit enemy IDs (for pierce)

        #region Unity Lifecycle

        private void Start()
        {
            _spawnTime = Time.time;
        }

        private void Update()
        {
            // Move projectile
            transform.Translate(_direction * _speed * Time.deltaTime, Space.World);

            // Check screen bounds for bounce
            if (_bounceRemaining > 0)
            {
                CheckScreenBounce();
            }

            // Destroy after lifetime
            if (Time.time - _spawnTime > _lifetime)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Check if hit enemy
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                int enemyID = enemy.GetInstanceID();

                // Skip if we already hit this enemy (for pierce)
                if (_hitEnemies.Contains(enemyID))
                    return;

                // Deal damage
                enemy.TakeDamage(_damage);
                _hitEnemies.Add(enemyID);

                // Pierce logic
                if (_pierceRemaining > 0)
                {
                    _pierceRemaining--;
                    // Continue through enemy
                }
                else
                {
                    // No pierce remaining - destroy projectile
                    Destroy(gameObject);
                }
            }
        }

        private void CheckScreenBounce()
        {
            // Get camera bounds
            Camera cam = Camera.main;
            if (cam == null) return;

            Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
            bool bounced = false;

            // Check horizontal bounds
            if (viewportPos.x < 0f || viewportPos.x > 1f)
            {
                _direction.x *= -1f;
                bounced = true;
            }

            // Check vertical bounds
            if (viewportPos.y < 0f || viewportPos.y > 1f)
            {
                _direction.y *= -1f;
                bounced = true;
            }

            if (bounced)
            {
                _bounceRemaining--;
                // Update rotation to match new direction
                float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Initialize the projectile with direction, damage, speed, and upgrades.
        /// </summary>
        public void Initialize(Vector2 direction, float damage, float speed, int pierce = 0, int bounce = 0)
        {
            _direction = direction.normalized;
            _damage = damage;
            _speed = speed;
            _pierceRemaining = pierce;
            _bounceRemaining = bounce;
            _spawnTime = Time.time;
            _hitEnemies.Clear();
        }

        #endregion
    }
}
