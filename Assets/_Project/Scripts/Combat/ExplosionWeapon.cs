using UnityEngine;
using System.Collections.Generic;
using CVA.Enemies;
using CVA.Player;

namespace CVA.Combat
{
    /// <summary>
    /// Creates periodic expanding damage waves from player position.
    /// </summary>
    public class ExplosionWeapon : MonoBehaviour
    {
        [Header("Explosion Settings")]
        [SerializeField] private float _explosionInterval = 4f;
        [SerializeField] private float _damage = 40f;
        [SerializeField] private float _maxRadius = 6f;
        [SerializeField] private float _expansionSpeed = 8f;

        [Header("Visual")]
        [SerializeField] private Color _explosionColor = new Color(1f, 0.3f, 0.1f); // Orange-red

        private PowerupManager _powerupManager;
        private float _explosionTimer;
        private List<ExplosionWave> _activeWaves = new List<ExplosionWave>();

        private void Awake()
        {
            _powerupManager = GetComponent<PowerupManager>();
            _explosionTimer = _explosionInterval;
        }

        private void Update()
        {
            _explosionTimer -= Time.deltaTime;

            if (_explosionTimer <= 0f)
            {
                // Trigger multiple waves (1 + additional from powerup)
                int additionalWaves = _powerupManager != null ? _powerupManager.AdditionalProjectiles : 0;
                int totalWaves = 1 + additionalWaves;
                for (int i = 0; i < totalWaves; i++)
                {
                    TriggerExplosion();
                }

                // Apply fire rate multiplier to explosion interval
                float fireRateMultiplier = _powerupManager != null ? _powerupManager.FireRateMultiplier : 1f;
                _explosionTimer = _explosionInterval / fireRateMultiplier;
            }

            // Update active waves
            for (int i = _activeWaves.Count - 1; i >= 0; i--)
            {
                ExplosionWave wave = _activeWaves[i];
                wave.currentRadius += _expansionSpeed * Time.deltaTime;

                if (wave.currentRadius >= _maxRadius)
                {
                    Destroy(wave.visualObject);
                    _activeWaves.RemoveAt(i);
                }
                else
                {
                    UpdateWaveVisual(wave);
                    DamageEnemiesInWave(wave);
                }
            }
        }

        private void TriggerExplosion()
        {
            // Create visual
            GameObject waveObj = new GameObject("ExplosionWave");
            waveObj.transform.position = transform.position;
            waveObj.transform.SetParent(transform.parent); // Don't parent to player so it stays in place

            LineRenderer lr = waveObj.AddComponent<LineRenderer>();
            lr.startWidth = 0.3f;
            lr.endWidth = 0.3f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = _explosionColor;
            lr.endColor = _explosionColor;
            lr.sortingOrder = 4;
            lr.useWorldSpace = true;

            // Create circle
            int segments = 40;
            lr.positionCount = segments + 1;

            ExplosionWave wave = new ExplosionWave
            {
                center = transform.position,
                currentRadius = 0.5f,
                visualObject = waveObj,
                lineRenderer = lr,
                hitEnemies = new HashSet<int>()
            };

            _activeWaves.Add(wave);
        }

        private void UpdateWaveVisual(ExplosionWave wave)
        {
            int segments = wave.lineRenderer.positionCount - 1;

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 360f / segments * Mathf.Deg2Rad;
                Vector3 point = wave.center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * wave.currentRadius;
                wave.lineRenderer.SetPosition(i, point);
            }

            // Fade out as it expands
            float fade = 1f - (wave.currentRadius / _maxRadius);
            Color c = _explosionColor;
            c.a = fade;
            wave.lineRenderer.startColor = c;
            wave.lineRenderer.endColor = c;
        }

        private void DamageEnemiesInWave(ExplosionWave wave)
        {
            // Check enemies in expanding ring
            Collider2D[] hits = Physics2D.OverlapCircleAll(wave.center, wave.currentRadius);

            float damage = _damage;
            if (_powerupManager != null)
            {
                damage *= _powerupManager.DamageMultiplier;
            }

            foreach (Collider2D hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    int enemyID = enemy.GetInstanceID();
                    if (!wave.hitEnemies.Contains(enemyID))
                    {
                        // Check if enemy is roughly at the wave edge (within threshold)
                        float distToCenter = Vector3.Distance(wave.center, enemy.transform.position);
                        float threshold = 1f; // How thick the wave damage zone is

                        if (Mathf.Abs(distToCenter - wave.currentRadius) < threshold)
                        {
                            enemy.TakeDamage(damage);
                            wave.hitEnemies.Add(enemyID);
                        }
                    }
                }
            }
        }

        private class ExplosionWave
        {
            public Vector3 center;
            public float currentRadius;
            public GameObject visualObject;
            public LineRenderer lineRenderer;
            public HashSet<int> hitEnemies;
        }
    }
}
