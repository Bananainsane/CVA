using UnityEngine;
using System.Collections.Generic;
using CVA.Data;
using CVA.Combat;
using System;

namespace CVA.Player
{
    /// <summary>
    /// Manages player weapons.
    /// </summary>
    public class WeaponManager : MonoBehaviour
    {
        [Header("Starting Weapons")]
        [SerializeField] private List<WeaponData> _startingWeapons = new List<WeaponData>();

        [Header("Weapon Slots")]
        [SerializeField] private Transform _weaponContainer;

        // Active weapons
        private List<GameObject> _activeWeapons = new List<GameObject>();
        private List<Type> _unlockedWeaponTypes = new List<Type>();

        #region Unity Lifecycle

        private void Start()
        {
            // Equip starting weapons
            foreach (var weaponData in _startingWeapons)
            {
                AddWeapon(weaponData);
            }
        }

        #endregion

        #region Weapon Management

        /// <summary>
        /// Add a new weapon to the player.
        /// </summary>
        public void AddWeapon(WeaponData weaponData)
        {
            if (weaponData == null)
                return;

            // Create weapon instance
            GameObject weaponGO = new GameObject(weaponData.weaponName);
            weaponGO.transform.parent = _weaponContainer != null ? _weaponContainer : transform;
            weaponGO.transform.localPosition = Vector3.zero;

            _activeWeapons.Add(weaponGO);
        }

        /// <summary>
        /// Get all active weapons.
        /// </summary>
        public List<GameObject> GetActiveWeapons()
        {
            return _activeWeapons;
        }

        /// <summary>
        /// Unlock a weapon by adding its component to the player.
        /// </summary>
        public void UnlockWeapon(Type weaponType)
        {
            if (weaponType == null)
            {
                return;
            }

            // Check if already unlocked
            if (_unlockedWeaponTypes.Contains(weaponType))
            {
                return;
            }

            // Add weapon component to player
            Component weapon = gameObject.AddComponent(weaponType);
            if (weapon != null)
            {
                _unlockedWeaponTypes.Add(weaponType);
            }
        }

        /// <summary>
        /// Unlock a weapon by name string (for powerup system).
        /// </summary>
        public void UnlockWeaponByName(string weaponName)
        {
            Type weaponType = null;

            switch (weaponName)
            {
                case "Laser":
                    weaponType = typeof(LaserWeapon);
                    break;
                case "Lightning":
                    weaponType = typeof(LightningWeapon);
                    break;
                case "GroundAOE":
                case "HolyFire":
                    weaponType = typeof(GroundAOEWeapon);
                    break;
                case "Orbiting":
                case "Blade":
                    weaponType = typeof(OrbitingWeapon);
                    break;
                case "Explosion":
                case "Nova":
                    weaponType = typeof(ExplosionWeapon);
                    break;
                default:
                    return;
            }

            UnlockWeapon(weaponType);
        }

        #endregion
    }
}
