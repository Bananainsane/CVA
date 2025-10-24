using UnityEngine;

namespace CVA.Data
{
    /// <summary>
    /// Defines different types of XP orbs with varying values and rarities.
    /// Used for dropped collectibles when enemies die.
    /// </summary>
    [CreateAssetMenu(fileName = "New XP Orb", menuName = "CVA/Data/XP Orb Data")]
    public class XPOrbData : ScriptableObject
    {
        [Header("XP Value")]
        [SerializeField] private int _xpValue = 1;

        [Header("Visual Properties")]
        [SerializeField] private OrbRarity _rarity = OrbRarity.Common;
        [SerializeField] private Color _orbColor = Color.cyan;
        [SerializeField] private float _orbSize = 1.0f;

        [Header("Drop Chance")]
        [Tooltip("Higher weight = more common. Common:70, Uncommon:20, Rare:10")]
        [SerializeField] private int _spawnWeight = 100;

        // Public Properties
        public int XPValue => _xpValue;
        public OrbRarity Rarity => _rarity;
        public Color OrbColor => _orbColor;
        public float OrbSize => _orbSize;
        public int SpawnWeight => _spawnWeight;
    }

    /// <summary>
    /// Rarity tiers for XP orbs.
    /// </summary>
    public enum OrbRarity
    {
        Common,     // Small blue orbs (5 XP)
        Uncommon,   // Medium green orbs (10 XP)
        Rare,       // Large purple orbs (25 XP)
        Epic,       // Gold orbs (50 XP)
        Legendary   // Rainbow orbs (100 XP)
    }
}
