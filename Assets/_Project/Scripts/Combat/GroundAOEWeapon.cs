using UnityEngine;
using System.Collections.Generic;
using CVA.Enemies;
using CVA.Player;

namespace CVA.Combat
{
    /// <summary>
    /// Creates damaging ground zones at random positions near player.
    /// </summary>
    public class GroundAOEWeapon : MonoBehaviour
    {
        [Header("AOE Settings")]
        [SerializeField] private float _spawnInterval = 5f;
        [SerializeField] private float _damagePerSecond = 20f;
        [SerializeField] private float _zoneDuration = 2.5f;
        [SerializeField] private float _zoneRadius = 1.5f;
        [SerializeField] private float _spawnDistance = 4f;
        [SerializeField] private int _maxActiveZones = 3;

        [Header("Visual")]
        [SerializeField] private Color _zoneColor = new Color(1f, 0.5f, 0f, 0.5f); // Orange

        private PowerupManager _powerupManager;
        private float _spawnTimer;
        private List<AOEZone> _activeZones = new List<AOEZone>();

        private void Awake()
        {
            _powerupManager = GetComponent<PowerupManager>();
            _spawnTimer = _spawnInterval;
        }

        private void Update()
        {
            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer <= 0f)
            {
                // Spawn multiple zones at once (1 + additional from powerup)
                int additionalZones = _powerupManager != null ? _powerupManager.AdditionalProjectiles : 0;
                int zonesToSpawn = 1 + additionalZones;
                for (int i = 0; i < zonesToSpawn && _activeZones.Count < _maxActiveZones; i++)
                {
                    SpawnAOEZone();
                }

                // Apply fire rate multiplier to spawn interval
                float fireRateMultiplier = _powerupManager != null ? _powerupManager.FireRateMultiplier : 1f;
                _spawnTimer = _spawnInterval / fireRateMultiplier;
            }

            // Update existing zones
            for (int i = _activeZones.Count - 1; i >= 0; i--)
            {
                _activeZones[i].lifetime += Time.deltaTime;

                if (_activeZones[i].lifetime >= _zoneDuration)
                {
                    Destroy(_activeZones[i].visualObject);
                    _activeZones.RemoveAt(i);
                }
                else
                {
                    DamageEnemiesInZone(_activeZones[i]);
                    UpdateZoneVisual(_activeZones[i]);
                }
            }
        }

        private void SpawnAOEZone()
        {
            // Random position near player
            Vector2 randomOffset = Random.insideUnitCircle * _spawnDistance;
            Vector3 spawnPos = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            // Create visual - just a simple GameObject with LineRenderer circle
            GameObject zoneObj = new GameObject("GroundAOE");
            zoneObj.transform.position = spawnPos;

            // Create circle using LineRenderer
            LineRenderer lr = zoneObj.AddComponent<LineRenderer>();
            lr.startWidth = 0.2f;
            lr.endWidth = 0.2f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = _zoneColor;
            lr.endColor = _zoneColor;
            lr.sortingOrder = 1;
            lr.useWorldSpace = false;

            // Create circle points
            int segments = 40;
            lr.positionCount = segments + 1;
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 360f / segments * Mathf.Deg2Rad;
                Vector3 point = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * _zoneRadius;
                lr.SetPosition(i, point);
            }

            AOEZone zone = new AOEZone
            {
                position = spawnPos,
                radius = _zoneRadius,
                lifetime = 0f,
                visualObject = zoneObj,
                lineRenderer = lr
            };

            _activeZones.Add(zone);
        }

        private void DamageEnemiesInZone(AOEZone zone)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(zone.position, zone.radius);

            float damage = _damagePerSecond * Time.deltaTime;
            if (_powerupManager != null)
            {
                damage *= _powerupManager.DamageMultiplier;
            }

            foreach (Collider2D hit in hits)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        private void UpdateZoneVisual(AOEZone zone)
        {
            // Pulse effect - update the circle size
            float pulseScale = 1f + Mathf.Sin(zone.lifetime * 5f) * 0.1f;
            float currentRadius = _zoneRadius * pulseScale;

            // Update circle points with pulse
            int segments = zone.lineRenderer.positionCount - 1;
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 360f / segments * Mathf.Deg2Rad;
                Vector3 point = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * currentRadius;
                zone.lineRenderer.SetPosition(i, point);
            }

            // Fade out near end
            float fadeStart = _zoneDuration * 0.7f;
            if (zone.lifetime > fadeStart)
            {
                float fade = 1f - (zone.lifetime - fadeStart) / (_zoneDuration - fadeStart);
                Color c = _zoneColor;
                c.a = fade * _zoneColor.a;
                zone.lineRenderer.startColor = c;
                zone.lineRenderer.endColor = c;
            }
        }

        private class AOEZone
        {
            public Vector3 position;
            public float radius;
            public float lifetime;
            public GameObject visualObject;
            public LineRenderer lineRenderer;
        }
    }
}
