using UnityEngine;
using System.Collections.Generic;
using CVA.Enemies;
using CVA.Player;

namespace CVA.Combat
{
    /// <summary>
    /// Rotating laser beam that continuously damages enemies.
    /// </summary>
    public class LaserWeapon : MonoBehaviour
    {
        [Header("Laser Settings")]
        [SerializeField] private float _rotationSpeed = 90f; // Degrees per second
        [SerializeField] private float _beamLength = 8f;
        [SerializeField] private float _damagePerSecond = 20f;
        [SerializeField] private float _beamWidth = 0.1f;

        [Header("Visual")]
        [SerializeField] private Color _laserColor = new Color(1f, 0.2f, 0.2f); // Red laser

        private List<LineRenderer> _beams = new List<LineRenderer>();
        private PowerupManager _powerupManager;
        private float _currentAngle = 0f;

        private void Awake()
        {
            _powerupManager = GetComponent<PowerupManager>();
            CreateBeam(); // Create initial beam
        }

        private void Update()
        {
            // Rotate laser (scale with fire rate for faster hits)
            float fireRateMultiplier = _powerupManager != null ? _powerupManager.FireRateMultiplier : 1f;
            _currentAngle += _rotationSpeed * Time.deltaTime * fireRateMultiplier;
            if (_currentAngle >= 360f) _currentAngle -= 360f;

            // Ensure we have the right number of beams based on global powerup
            EnsureCorrectBeamCount();

            // Update all beams
            UpdateAllBeams();
        }

        private void EnsureCorrectBeamCount()
        {
            int additionalBeams = _powerupManager != null ? _powerupManager.AdditionalProjectiles : 0;
            int requiredBeams = 1 + additionalBeams;

            while (_beams.Count < requiredBeams)
            {
                CreateBeam();
            }
        }

        private void CreateBeam()
        {
            GameObject laserObj = new GameObject($"LaserBeam_{_beams.Count}");
            laserObj.transform.SetParent(transform);
            laserObj.transform.localPosition = Vector3.zero;

            LineRenderer lr = laserObj.AddComponent<LineRenderer>();
            lr.startWidth = _beamWidth;
            lr.endWidth = _beamWidth;
            lr.positionCount = 2;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = _laserColor;
            lr.endColor = _laserColor;
            lr.sortingOrder = 5;

            _beams.Add(lr);
        }

        private void UpdateAllBeams()
        {
            int additionalBeams = _powerupManager != null ? _powerupManager.AdditionalProjectiles : 0;
            int totalBeams = 1 + additionalBeams;
            float angleStep = 360f / totalBeams;

            for (int i = 0; i < _beams.Count; i++)
            {
                float beamAngle = _currentAngle + (i * angleStep);
                UpdateBeamVisual(_beams[i], beamAngle);
                CheckBeamHits(beamAngle);
            }
        }

        private void UpdateBeamVisual(LineRenderer beam, float angle)
        {
            Vector3 start = transform.position;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;
            Vector3 end = start + direction * _beamLength;

            beam.SetPosition(0, start);
            beam.SetPosition(1, end);
        }

        private void CheckBeamHits(float angle)
        {
            Vector3 start = transform.position;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.right;

            RaycastHit2D[] hits = Physics2D.RaycastAll(start, direction, _beamLength);

            float damage = _damagePerSecond * Time.deltaTime;
            if (_powerupManager != null)
            {
                damage *= _powerupManager.DamageMultiplier;
            }

            foreach (RaycastHit2D hit in hits)
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
    }
}
