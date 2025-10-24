using UnityEngine;
using System.Collections.Generic;
using CVA.Enemies;
using CVA.Player;

namespace CVA.Combat
{
    /// <summary>
    /// Blades that orbit around the player, damaging enemies on contact.
    /// </summary>
    public class OrbitingWeapon : MonoBehaviour
    {
        [Header("Orbit Settings")]
        [SerializeField] private int _bladeCount = 3;
        [SerializeField] private float _orbitRadius = 3f;
        [SerializeField] private float _rotationSpeed = 180f; // Degrees per second
        [SerializeField] private float _damagePerHit = 25f;
        [SerializeField] private float _bladeSize = 0.5f;

        [Header("Visual")]
        [SerializeField] private Color _bladeColor = new Color(0.8f, 0.8f, 0.2f); // Yellow

        private PowerupManager _powerupManager;
        private List<OrbitingBlade> _blades = new List<OrbitingBlade>();
        private float _currentAngle = 0f;

        private void Awake()
        {
            _powerupManager = GetComponent<PowerupManager>();
            CreateBlades();
        }

        private void CreateBlades()
        {
            for (int i = 0; i < _bladeCount; i++)
            {
                CreateBlade();
            }
            RecalculateBladeOffsets();
        }

        private void CreateBlade()
        {
            GameObject bladeObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(bladeObj.GetComponent<BoxCollider>()); // Remove 3D collider

            // Add 2D collider for damage
            BoxCollider2D collider = bladeObj.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = Vector2.one * _bladeSize;

            // Setup visual
            bladeObj.transform.SetParent(transform);
            bladeObj.transform.localScale = new Vector3(_bladeSize, _bladeSize, 0.1f);

            MeshRenderer mr = bladeObj.GetComponent<MeshRenderer>();
            mr.material = new Material(Shader.Find("Sprites/Default"));
            mr.material.color = _bladeColor;
            mr.sortingLayerName = "Default";

            // Add blade component
            Blade blade = bladeObj.AddComponent<Blade>();
            blade.damage = _damagePerHit;
            blade.weapon = this;

            OrbitingBlade orbitingBlade = new OrbitingBlade
            {
                gameObject = bladeObj,
                angleOffset = 0, // Will be recalculated
                blade = blade
            };

            _blades.Add(orbitingBlade);
        }

        private void RecalculateBladeOffsets()
        {
            float angleStep = 360f / _blades.Count;
            for (int i = 0; i < _blades.Count; i++)
            {
                _blades[i].angleOffset = i * angleStep;
            }
        }

        private void Update()
        {
            // Ensure we have the right number of blades based on global powerup
            int additionalBlades = _powerupManager != null ? _powerupManager.AdditionalProjectiles : 0;
            int requiredBlades = _bladeCount + additionalBlades;
            while (_blades.Count < requiredBlades)
            {
                CreateBlade();
                RecalculateBladeOffsets();
            }

            // Rotate blades (scale with fire rate for faster hits)
            float fireRateMultiplier = _powerupManager != null ? _powerupManager.FireRateMultiplier : 1f;
            _currentAngle += _rotationSpeed * Time.deltaTime * fireRateMultiplier;
            if (_currentAngle >= 360f) _currentAngle -= 360f;

            // Update blade positions
            foreach (var blade in _blades)
            {
                float angle = (_currentAngle + blade.angleOffset) * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * _orbitRadius;
                blade.gameObject.transform.position = transform.position + offset;

                // Rotate blade itself for visual effect (also scaled by fire rate)
                blade.gameObject.transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime * 2f * fireRateMultiplier);
            }
        }

        public float GetDamage()
        {
            float damage = _damagePerHit;
            if (_powerupManager != null)
            {
                damage *= _powerupManager.DamageMultiplier;
            }
            return damage;
        }

        private class OrbitingBlade
        {
            public GameObject gameObject;
            public float angleOffset;
            public Blade blade;
        }

        private class Blade : MonoBehaviour
        {
            public float damage;
            public OrbitingWeapon weapon;
            private HashSet<int> _hitThisFrame = new HashSet<int>();

            private void LateUpdate()
            {
                _hitThisFrame.Clear();
            }

            private void OnTriggerStay2D(Collider2D collision)
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    int enemyID = enemy.GetInstanceID();
                    if (!_hitThisFrame.Contains(enemyID))
                    {
                        enemy.TakeDamage(weapon.GetDamage());
                        _hitThisFrame.Add(enemyID);
                    }
                }
            }
        }
    }
}
