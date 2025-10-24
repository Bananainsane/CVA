using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CVA.Core;

namespace CVA.UI
{
    /// <summary>
    /// Displays XP progress with a fill bar.
    /// </summary>
    public class XPBar : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _xpText;

        [Header("Experience Manager")]
        [SerializeField] private ExperienceManager _experienceManager;
        [SerializeField] private bool _autoFind = true;

        #region Unity Lifecycle

        private void Start()
        {
            if (_autoFind && _experienceManager == null)
            {
                _experienceManager = FindFirstObjectByType<ExperienceManager>();
            }

            if (_experienceManager != null)
            {
                _experienceManager.OnXPChanged.AddListener(UpdateXPDisplay);
                _experienceManager.OnLevelUp.AddListener(UpdateLevelDisplay);

                UpdateXPDisplay(_experienceManager.CurrentXP, _experienceManager.XPRequired);
                UpdateLevelDisplay(_experienceManager.CurrentLevel);
            }
        }

        private void OnDestroy()
        {
            if (_experienceManager != null)
            {
                _experienceManager.OnXPChanged.RemoveListener(UpdateXPDisplay);
                _experienceManager.OnLevelUp.RemoveListener(UpdateLevelDisplay);
            }
        }

        #endregion

        #region UI Update

        private void UpdateXPDisplay(int currentXP, int requiredXP)
        {
            // Update fill bar
            if (_fillImage != null)
            {
                _fillImage.fillAmount = (float)currentXP / requiredXP;
            }

            // Update text
            if (_xpText != null)
            {
                _xpText.text = $"{currentXP}/{requiredXP} XP";
            }
        }

        private void UpdateLevelDisplay(int level)
        {
            if (_levelText != null)
            {
                _levelText.text = $"Level {level}";
            }
        }

        #endregion
    }
}
