using UnityEngine;

namespace CVA.Data
{
    /// <summary>
    /// Defines a powerup that can be applied to the player.
    /// </summary>
    [CreateAssetMenu(fileName = "New Powerup", menuName = "CVA/Data/Powerup Data")]
    public class PowerupData : ScriptableObject
    {
        [Header("Display")]
        [SerializeField] private string _powerupName = "Powerup";
        [SerializeField, TextArea] private string _description = "Increases stats";
        [SerializeField] private Sprite _icon;

        [Header("Effect")]
        [SerializeField] private PowerupType _type = PowerupType.Damage;
        [SerializeField] private float _value = 10f;
        [SerializeField] private bool _isPercentage = false;

        [Header("Weapon Unlock (only for WeaponUnlock type)")]
        [SerializeField] private string _weaponName = "";

        [Header("Stacking")]
        [SerializeField] private bool _stackable = true;
        [SerializeField] private int _maxStacks = 5;

        // Properties
        public string PowerupName => _powerupName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public PowerupType Type => _type;
        public float Value => _value;
        public bool IsPercentage => _isPercentage;
        public bool Stackable => _stackable;
        public int MaxStacks => _maxStacks;
        public string WeaponName => _weaponName;

        /// <summary>
        /// Get formatted description with value replaced.
        /// </summary>
        public string GetFormattedDescription()
        {
            string valueStr = _isPercentage ? $"{_value}%" : _value.ToString("F1");
            return _description.Replace("{value}", valueStr);
        }
    }

    /// <summary>
    /// Types of powerups available.
    /// </summary>
    public enum PowerupType
    {
        // Stat Upgrades
        Damage,
        FireRate,
        MoveSpeed,
        MaxHealth,
        HealthRegen,
        PickupRange,
        ProjectileSpeed,
        Area,
        Duration,
        Cooldown,
        Amount,
        Magnet,

        // Weapon Upgrades
        AdditionalProjectile,
        Pierce,
        Bounce,
        Chain,

        // Weapon Unlocks
        WeaponUnlock
    }
}
