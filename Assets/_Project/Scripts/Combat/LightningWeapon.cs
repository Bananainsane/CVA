using UnityEngine;
using System.Collections.Generic;
using CVA.Enemies;
using CVA.Player;

namespace CVA.Combat
{
    /// <summary>
    /// Lightning that chains between nearby enemies.
    /// </summary>
    public class LightningWeapon : MonoBehaviour
    {
        [Header("Lightning Settings")]
        [SerializeField] private float _strikeInterval = 2f;
        [SerializeField] private float _damage = 30f;
        [SerializeField] private float _chainRange = 5f;
        [SerializeField] private int _maxChains = 4;
        [SerializeField] private float _strikeDuration = 0.3f;

        [Header("Visual")]
        [SerializeField] private Color _lightningColor = new Color(0.3f, 0.8f, 1f); // Cyan

        private PowerupManager _powerupManager;
        private float _strikeTimer;
        private List<LineRenderer> _lightningBolts = new List<LineRenderer>();

        private void Awake()
        {
            _powerupManager = GetComponent<PowerupManager>();
            _strikeTimer = _strikeInterval;

            // Create enough line renderers for multiple strikes with chains
            // Initial pool: 1 strike with max chains
            CreateLightningBolts(_maxChains);
        }

        private void CreateLightningBolts(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject boltObj = new GameObject($"LightningBolt_{_lightningBolts.Count}");
                boltObj.transform.SetParent(transform);
                LineRenderer lr = boltObj.AddComponent<LineRenderer>();
                lr.startWidth = 0.15f;
                lr.endWidth = 0.05f;
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = _lightningColor;
                lr.endColor = _lightningColor;
                lr.sortingOrder = 6;
                lr.enabled = false;
                _lightningBolts.Add(lr);
            }
        }

        private void Update()
        {
            // Ensure we have enough line renderers for current powerup level
            int additionalStrikes = _powerupManager != null ? _powerupManager.AdditionalProjectiles : 0;
            int requiredBolts = (1 + additionalStrikes) * _maxChains;
            while (_lightningBolts.Count < requiredBolts)
            {
                CreateLightningBolts(1);
            }

            _strikeTimer -= Time.deltaTime;

            if (_strikeTimer <= 0f)
            {
                StrikeLightning();
                // Apply fire rate multiplier to strike interval
                float fireRateMultiplier = _powerupManager != null ? _powerupManager.FireRateMultiplier : 1f;
                _strikeTimer = _strikeInterval / fireRateMultiplier;
            }

            // Fade out lightning bolts
            foreach (var bolt in _lightningBolts)
            {
                if (bolt.enabled)
                {
                    Color c = bolt.startColor;
                    c.a -= Time.deltaTime / _strikeDuration;
                    if (c.a <= 0)
                    {
                        bolt.enabled = false;
                    }
                    else
                    {
                        bolt.startColor = c;
                        bolt.endColor = c;
                    }
                }
            }
        }

        private void StrikeLightning()
        {
            // Find nearest enemies
            Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            if (allEnemies.Length == 0) return;

            float damage = _damage;
            if (_powerupManager != null)
            {
                damage *= _powerupManager.DamageMultiplier;
            }

            // Find N nearest enemies to strike (1 + additional strikes from powerup)
            int additionalStrikes = _powerupManager != null ? _powerupManager.AdditionalProjectiles : 0;
            int totalStrikes = 1 + additionalStrikes;
            List<Enemy> initialTargets = FindNearestEnemies(allEnemies, totalStrikes);

            int boltIndex = 0;
            List<Enemy> globalHitList = new List<Enemy>(); // Track all hit enemies across all strikes

            // Strike each initial target and chain from them
            foreach (Enemy initialTarget in initialTargets)
            {
                if (initialTarget == null) continue;

                List<Enemy> localHitList = new List<Enemy>();
                Vector3 currentPos = transform.position;
                Enemy currentTarget = initialTarget;

                for (int i = 0; i < _maxChains && currentTarget != null && boltIndex < _lightningBolts.Count; i++)
                {
                    // Damage enemy
                    currentTarget.TakeDamage(damage);
                    localHitList.Add(currentTarget);
                    globalHitList.Add(currentTarget);

                    // Draw lightning bolt with zigzag
                    DrawLightningBolt(boltIndex, currentPos, currentTarget.transform.position);
                    boltIndex++;

                    // Find next chain target (exclude all globally hit enemies)
                    Vector3 lastPos = currentTarget.transform.position;
                    currentTarget = FindNearestEnemyExcluding(lastPos, globalHitList);
                    currentPos = lastPos;

                    if (currentTarget != null)
                    {
                        float dist = Vector3.Distance(currentPos, currentTarget.transform.position);
                        if (dist > _chainRange)
                        {
                            break; // Too far to chain
                        }
                    }
                }
            }
        }

        private List<Enemy> FindNearestEnemies(Enemy[] allEnemies, int count)
        {
            List<Enemy> nearest = new List<Enemy>();
            List<Enemy> pool = new List<Enemy>(allEnemies);

            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                Enemy closest = null;
                float closestDist = float.MaxValue;

                foreach (Enemy enemy in pool)
                {
                    float dist = Vector3.Distance(transform.position, enemy.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = enemy;
                    }
                }

                if (closest != null)
                {
                    nearest.Add(closest);
                    pool.Remove(closest);
                }
            }

            return nearest;
        }

        private Enemy FindNearestEnemyExcluding(Vector3 position, List<Enemy> exclude)
        {
            Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            Enemy nearest = null;
            float nearestDist = float.MaxValue;

            foreach (Enemy enemy in allEnemies)
            {
                if (exclude.Contains(enemy)) continue;

                float dist = Vector3.Distance(position, enemy.transform.position);
                if (dist < nearestDist && dist <= _chainRange)
                {
                    nearestDist = dist;
                    nearest = enemy;
                }
            }

            return nearest;
        }

        private void DrawLightningBolt(int index, Vector3 start, Vector3 end)
        {
            if (index >= _lightningBolts.Count) return;

            LineRenderer lr = _lightningBolts[index];
            lr.enabled = true;

            // Create zigzag pattern
            int segments = 5;
            lr.positionCount = segments + 1;
            lr.SetPosition(0, start);

            Vector3 direction = (end - start) / segments;
            Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0).normalized;

            for (int i = 1; i < segments; i++)
            {
                Vector3 point = start + direction * i;
                point += perpendicular * Random.Range(-0.3f, 0.3f);
                lr.SetPosition(i, point);
            }

            lr.SetPosition(segments, end);

            // Reset color
            Color c = _lightningColor;
            c.a = 1f;
            lr.startColor = c;
            lr.endColor = c;
        }
    }
}
