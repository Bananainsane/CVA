using UnityEngine;

namespace CVA.Data
{
    /// <summary>
    /// ScriptableObject that defines enemy stats and properties.
    /// </summary>
    [CreateAssetMenu(fileName = "New Enemy", menuName = "CVA/Data/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Display Info")]
        public string enemyName = "Enemy";
        [TextArea] public string description = "Basic enemy";

        [Header("Stats")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _contactDamage = 10f;
        [SerializeField] private int _xpValue = 5;

        [Header("Visual")]
        public Color enemyColor = Color.red;
        public float sizeScale = 1f;

        [Header("XP Drops")]
        [SerializeField] private XPOrbData _xpOrbData;

        // Properties
        public float MaxHealth => _maxHealth;
        public float MoveSpeed => _moveSpeed;
        public float ContactDamage => _contactDamage;
        public int XPValue => _xpValue;
        public XPOrbData GetXPOrbData() => _xpOrbData;
    }
}
