using UnityEngine;
using UnityEngine.Events;

namespace CVA.Core
{
    /// <summary>
    /// Manages player XP and leveling system.
    /// Triggers powerup selection on level up.
    /// </summary>
    public class ExperienceManager : MonoBehaviour
    {
        [Header("Leveling")]
        [SerializeField] private int _currentLevel = 1;
        [SerializeField] private int _currentXP = 0;
        [SerializeField] private int _baseXPRequired = 10;
        [SerializeField] private float _xpScalingFactor = 1.5f;

        [Header("Events")]
        public UnityEvent<int, int> OnXPChanged = new UnityEvent<int, int>(); // current XP, required XP
        public UnityEvent<int> OnLevelUp = new UnityEvent<int>(); // new level
        public UnityEvent OnPowerupChoice = new UnityEvent(); // triggers powerup selection UI

        // Cached values
        private int _xpRequired;

        // Properties
        public int CurrentLevel => _currentLevel;
        public int CurrentXP => _currentXP;
        public int XPRequired => _xpRequired;

        #region Unity Lifecycle

        private void Start()
        {
            RecalculateXPRequired();
            OnXPChanged?.Invoke(_currentXP, _xpRequired);
        }

        #endregion

        #region XP Management

        /// <summary>
        /// Add XP to the player.
        /// </summary>
        public void AddXP(int amount)
        {
            _currentXP += amount;

            // Check for level up
            while (_currentXP >= _xpRequired)
            {
                LevelUp();
            }

            OnXPChanged?.Invoke(_currentXP, _xpRequired);
        }

        private void LevelUp()
        {
            _currentXP -= _xpRequired;
            _currentLevel++;

            RecalculateXPRequired();

            OnLevelUp?.Invoke(_currentLevel);

            // Trigger powerup selection
            OnPowerupChoice?.Invoke();

            // VFX
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                VFXManager.LevelUp(player.transform.position);
            }
        }

        private void RecalculateXPRequired()
        {
            _xpRequired = Mathf.RoundToInt(_baseXPRequired * Mathf.Pow(_xpScalingFactor, _currentLevel - 1));
        }

        #endregion
    }
}
