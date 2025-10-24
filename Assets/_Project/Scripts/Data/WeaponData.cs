using UnityEngine;

namespace CVA.Data
{
    /// <summary>
    /// ScriptableObject that defines weapon stats.
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon", menuName = "CVA/Data/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [Header("Display Info")]
        public string weaponName = "Weapon";
        [TextArea] public string description = "Basic weapon";
        public Sprite icon;

        [Header("Stats")]
        public float damage = 10f;
        public float fireRate = 1f; // Attacks per second
        public float attackRange = 10f;
        public float projectileSpeed = 15f;

        [Header("Projectile")]
        public GameObject projectilePrefab;
        public float projectileLifetime = 2f;

        [Header("Upgrade")]
        public bool canUpgrade = true;
        public int maxLevel = 5;
    }
}
