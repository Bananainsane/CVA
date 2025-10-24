using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CVA.Data;

namespace CVA.UI
{
    /// <summary>
    /// Individual powerup choice button.
    /// </summary>
    public class PowerupChoiceButton : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        private PowerupData _powerupData;
        private PowerupSelectionUI _selectionUI;

        private void Awake()
        {
            // Auto-find components if not assigned
            if (_button == null)
                _button = GetComponent<Button>();
            if (_nameText == null)
                _nameText = GetComponentInChildren<TextMeshProUGUI>();
        }

        /// <summary>
        /// Setup this button with powerup data.
        /// </summary>
        public void Setup(PowerupData powerup, PowerupSelectionUI selectionUI)
        {
            _powerupData = powerup;
            _selectionUI = selectionUI;

            // Update UI elements
            if (_nameText != null)
            {
                _nameText.text = powerup.PowerupName;
            }

            if (_descriptionText != null)
            {
                _descriptionText.text = powerup.GetFormattedDescription();
            }

            if (_iconImage != null && powerup.Icon != null)
            {
                _iconImage.sprite = powerup.Icon;
                _iconImage.enabled = true;
            }
            else if (_iconImage != null)
            {
                _iconImage.enabled = false;
            }

            // Setup button click
            if (_button != null)
            {
                _button.onClick.RemoveAllListeners();
                _button.onClick.AddListener(OnClick);
            }
        }

        /// <summary>
        /// Called when button is clicked.
        /// </summary>
        private void OnClick()
        {
            if (_selectionUI != null && _powerupData != null)
            {
                _selectionUI.OnPowerupSelected(_powerupData);
            }
        }
    }
}
