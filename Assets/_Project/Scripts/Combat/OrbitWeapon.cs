using UnityEngine;
using CVA.Data;
using CVA.Enemies;

namespace CVA.Combat
{
    /// <summary>
    /// Weapon that creates orbiting projectiles around the player (like Garlic in Vampire Survivors).
    /// Damages enemies on contact continuously.
    /// </summary>
    public class OrbitWeapon : MonoBehaviour
    {
        [Header("Weapon Data")]
        [SerializeField] private WeaponData _weaponData;

        [Header("Orbit Settings")]
        [SerializeField] private float _orbitRadius = 2f;
        [SerializeField] private float _rotationSpeed = 90f; // degrees per second
        [SerializeField] private int _projectileCount = 3;

        [Header("Visual")]
        [SerializeField] private Color _projectileColor = new Color(0.8f, 0.3f, 1f);
        [SerializeField] private float _projectileSize = 0.5f;

        // Runtime
        private Transform _playerTransform;
        private GameObject[] _orbitingObjects;
        private float _currentAngle = 0f;
        private float _currentDamage;
        private float _damageInterval = 0.5f; // Damage every 0.5 seconds

        #region Unity Lifecycle

        private void Start()
        {
            FindPlayer();
            InitializeWeapon();
            CreateOrbitingProjectiles();
        }

        private void Update()
        {
            if (_playerTransform == null)
                return;

            // Rotate orbiting objects
            _currentAngle += _rotationSpeed * Time.deltaTime;
            if (_currentAngle >= 360f)
                _currentAngle -= 360f;

            UpdateOrbitPositions();
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

        private void InitializeWeapon()
        {
            if (_weaponData != null)
            {
                _currentDamage = _weaponData.damage;
            }
        }

        private void CreateOrbitingProjectiles()
        {
            _orbitingObjects = new GameObject[_projectileCount];

            for (int i = 0; i < _projectileCount; i++)
            {
                GameObject orb = new GameObject($"OrbitProjectile_{i}");
                orb.transform.parent = transform;

                // Add visual
                SpriteRenderer sr = orb.AddComponent<SpriteRenderer>();
                sr.sprite = CreateSimpleSprite();
                sr.color = _projectileColor;
                orb.transform.localScale = Vector3.one * _projectileSize;

                // Add collider for damage
                CircleCollider2D collider = orb.AddComponent<CircleCollider2D>();
                collider.isTrigger = true;
                collider.radius = 0.5f;

                // Add damage component
                OrbitProjectile proj = orb.AddComponent<OrbitProjectile>();
                proj.Initialize(_currentDamage, _damageInterval);

                _orbitingObjects[i] = orb;
            }
        }

        #endregion

        #region Orbit Movement

        private void UpdateOrbitPositions()
        {
            if (_playerTransform == null || _orbitingObjects == null)
                return;

            float angleStep = 360f / _projectileCount;

            for (int i = 0; i < _orbitingObjects.Length; i++)
            {
                if (_orbitingObjects[i] == null)
                    continue;

                float angle = (_currentAngle + (angleStep * i)) * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * _orbitRadius,
                    Mathf.Sin(angle) * _orbitRadius,
                    0f
                );

                _orbitingObjects[i].transform.position = _playerTransform.position + offset;
            }
        }

        #endregion

        #region Upgrade System

        /// <summary>
        /// Increase the number of orbiting projectiles.
        /// </summary>
        public void IncreaseProjectileCount(int amount = 1)
        {
            _projectileCount = Mathf.Min(_projectileCount + amount, 12); // Max 12 projectiles

            // Recreate projectiles
            DestroyOrbitingProjectiles();
            CreateOrbitingProjectiles();
        }

        /// <summary>
        /// Increase orbit radius.
        /// </summary>
        public void IncreaseRadius(float amount)
        {
            _orbitRadius += amount;
        }

        /// <summary>
        /// Increase damage.
        /// </summary>
        public void IncreaseDamage(float amount)
        {
            _currentDamage += amount;

            // Update existing projectiles
            if (_orbitingObjects != null)
            {
                foreach (var obj in _orbitingObjects)
                {
                    if (obj != null)
                    {
                        OrbitProjectile proj = obj.GetComponent<OrbitProjectile>();
                        if (proj != null)
                            proj.SetDamage(_currentDamage);
                    }
                }
            }
        }

        private void DestroyOrbitingProjectiles()
        {
            if (_orbitingObjects != null)
            {
                foreach (var obj in _orbitingObjects)
                {
                    if (obj != null)
                        Destroy(obj);
                }
            }
        }

        private Sprite CreateSimpleSprite()
        {
            // Create a simple white circle sprite at runtime
            Texture2D texture = new Texture2D(64, 64);
            Color[] pixels = new Color[64 * 64];

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    float dx = x - 32f;
                    float dy = y - 32f;
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);

                    if (distance < 30f)
                        pixels[y * 64 + x] = Color.white;
                    else
                        pixels[y * 64 + x] = Color.clear;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        }

        #endregion
    }

    /// <summary>
    /// Individual orbiting projectile that damages enemies on contact.
    /// </summary>
    public class OrbitProjectile : MonoBehaviour
    {
        private float _damage;
        private float _damageInterval;
        private System.Collections.Generic.Dictionary<Enemy, float> _lastDamageTime = new System.Collections.Generic.Dictionary<Enemy, float>();

        public void Initialize(float damage, float damageInterval)
        {
            _damage = damage;
            _damageInterval = damageInterval;
        }

        public void SetDamage(float damage)
        {
            _damage = damage;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Check if we can damage this enemy again
                if (!_lastDamageTime.ContainsKey(enemy) || Time.time - _lastDamageTime[enemy] >= _damageInterval)
                {
                    enemy.TakeDamage(_damage);
                    _lastDamageTime[enemy] = Time.time;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null && _lastDamageTime.ContainsKey(enemy))
            {
                _lastDamageTime.Remove(enemy);
            }
        }
    }
}
