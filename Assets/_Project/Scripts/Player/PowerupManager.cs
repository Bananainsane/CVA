using UnityEngine;
using System.Collections.Generic;
using CVA.Data;

namespace CVA.Player
{
    /// <summary>
    /// Manages player powerups and stat modifications.
    /// </summary>
    public class PowerupManager : MonoBehaviour
    {
        [Header("Available Powerups")]
        [SerializeField] private List<PowerupData> _availablePowerups = new List<PowerupData>();

        [Header("Stats")]
        private float _damageMultiplier = 1f;
        private float _fireRateMultiplier = 1f;
        private float _moveSpeedMultiplier = 1f;
        private float _magnetRange = 2f;
        private float _projectileSpeedMultiplier = 1f;
        private int _additionalProjectiles = 0;

        // Tracking applied powerups
        private Dictionary<PowerupData, int> _appliedPowerups = new Dictionary<PowerupData, int>();

        // Component references
        private PlayerController _playerController;
        private PlayerHealth _playerHealth;
        private WeaponManager _weaponManager;

        // Properties
        public float DamageMultiplier => _damageMultiplier;
        public float FireRateMultiplier => _fireRateMultiplier;
        public float MoveSpeedMultiplier => _moveSpeedMultiplier;
        public float MagnetRange => _magnetRange;
        public float ProjectileSpeedMultiplier => _projectileSpeedMultiplier;
        public int AdditionalProjectiles => _additionalProjectiles;
        public int ActivePowerupCount => _appliedPowerups.Count;

        #region Unity Lifecycle

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _playerHealth = GetComponent<PlayerHealth>();
            _weaponManager = GetComponent<WeaponManager>();
        }

        #endregion

        #region Powerup Selection

        /// <summary>
        /// Get random powerup choices for level up.
        /// </summary>
        public List<PowerupData> GetRandomPowerupChoices(int count)
        {
            List<PowerupData> choices = new List<PowerupData>();
            List<PowerupData> availablePool = new List<PowerupData>(_availablePowerups);

            // Filter out maxed powerups
            availablePool.RemoveAll(p =>
            {
                if (!p.Stackable)
                    return _appliedPowerups.ContainsKey(p);

                if (_appliedPowerups.ContainsKey(p))
                    return _appliedPowerups[p] >= p.MaxStacks;

                return false;
            });

            if (availablePool.Count == 0)
            {
                return choices;
            }

            // Pick random powerups
            count = Mathf.Min(count, availablePool.Count);
            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(0, availablePool.Count);
                choices.Add(availablePool[randomIndex]);
                availablePool.RemoveAt(randomIndex);
            }

            return choices;
        }

        #endregion

        #region Apply Powerup

        /// <summary>
        /// Apply a powerup to the player.
        /// </summary>
        public void ApplyPowerup(PowerupData powerup)
        {
            if (powerup == null)
                return;

            // Track powerup application
            if (_appliedPowerups.ContainsKey(powerup))
            {
                _appliedPowerups[powerup]++;
            }
            else
            {
                _appliedPowerups[powerup] = 1;
            }

            // Apply effect based on type
            switch (powerup.Type)
            {
                case PowerupType.Damage:
                    ApplyDamage(powerup);
                    break;
                case PowerupType.FireRate:
                    ApplyFireRate(powerup);
                    break;
                case PowerupType.MoveSpeed:
                    ApplyMoveSpeed(powerup);
                    break;
                case PowerupType.MaxHealth:
                    ApplyMaxHealth(powerup);
                    break;
                case PowerupType.PickupRange:
                case PowerupType.Magnet:
                    ApplyMagnetRange(powerup);
                    break;
                case PowerupType.ProjectileSpeed:
                    ApplyProjectileSpeed(powerup);
                    break;

                // Weapon Upgrades
                case PowerupType.AdditionalProjectile:
                    ApplyAdditionalProjectile(powerup);
                    break;
                case PowerupType.Pierce:
                    ApplyPierce(powerup);
                    break;
                case PowerupType.Bounce:
                    ApplyBounce(powerup);
                    break;
                case PowerupType.Chain:
                    ApplyChain(powerup);
                    break;

                // Weapon Unlocks
                case PowerupType.WeaponUnlock:
                    ApplyWeaponUnlock(powerup);
                    break;

                default:
                    break;
            }
        }

        private void ApplyDamage(PowerupData powerup)
        {
            if (powerup.IsPercentage)
            {
                _damageMultiplier *= (1f + powerup.Value / 100f);
            }
            else
            {
                _damageMultiplier += powerup.Value;
            }
        }

        private void ApplyFireRate(PowerupData powerup)
        {
            if (powerup.IsPercentage)
            {
                _fireRateMultiplier *= (1f + powerup.Value / 100f);
            }
            else
            {
                _fireRateMultiplier += powerup.Value;
            }
        }

        private void ApplyMoveSpeed(PowerupData powerup)
        {
            if (powerup.IsPercentage)
            {
                _moveSpeedMultiplier *= (1f + powerup.Value / 100f);
            }
            else
            {
                _moveSpeedMultiplier += powerup.Value;
            }

            // Apply to PlayerController
            if (_playerController != null)
            {
                float baseSpeed = 5f; // You might want to store this
                _playerController.SetMoveSpeed(baseSpeed * _moveSpeedMultiplier);
            }
        }

        private void ApplyMaxHealth(PowerupData powerup)
        {
            if (_playerHealth != null)
            {
                _playerHealth.AddMaxHealth(powerup.Value);
            }
        }

        private void ApplyMagnetRange(PowerupData powerup)
        {
            if (powerup.IsPercentage)
            {
                _magnetRange *= (1f + powerup.Value / 100f);
            }
            else
            {
                _magnetRange += powerup.Value;
            }

            _magnetRange = Mathf.Max(0.5f, _magnetRange);
        }

        private void ApplyProjectileSpeed(PowerupData powerup)
        {
            if (powerup.IsPercentage)
            {
                _projectileSpeedMultiplier *= (1f + powerup.Value / 100f);
            }
            else
            {
                _projectileSpeedMultiplier += powerup.Value;
            }
        }

        private void ApplyAdditionalProjectile(PowerupData powerup)
        {
            // Just increment the global counter
            // All weapons will read this value when they need it
            _additionalProjectiles += (int)powerup.Value;
        }

        private void ApplyPierce(PowerupData powerup)
        {
            // Find all weapons on player
            CVA.Combat.ProjectileWeapon[] weapons = GetComponentsInChildren<CVA.Combat.ProjectileWeapon>();
            foreach (var weapon in weapons)
            {
                weapon.AddPierce((int)powerup.Value);
            }
        }

        private void ApplyBounce(PowerupData powerup)
        {
            // Find all weapons on player
            CVA.Combat.ProjectileWeapon[] weapons = GetComponentsInChildren<CVA.Combat.ProjectileWeapon>();
            foreach (var weapon in weapons)
            {
                weapon.AddBounce((int)powerup.Value);
            }
        }

        private void ApplyChain(PowerupData powerup)
        {
            // Find all weapons on player
            CVA.Combat.ProjectileWeapon[] weapons = GetComponentsInChildren<CVA.Combat.ProjectileWeapon>();
            foreach (var weapon in weapons)
            {
                weapon.AddChain((int)powerup.Value);
            }
        }

        private void ApplyWeaponUnlock(PowerupData powerup)
        {
            if (_weaponManager != null)
            {
                _weaponManager.UnlockWeaponByName(powerup.WeaponName);
            }
        }

        #endregion
    }
}
