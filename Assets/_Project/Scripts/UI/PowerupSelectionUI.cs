using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CVA.Data;
using CVA.Player;
using CVA.Core;

namespace CVA.UI
{
    /// <summary>
    /// Vampire Survivors-style powerup selection UI.
    /// Shows 3 powerup choices when player levels up.
    /// Pauses game while selection is active.
    /// </summary>
    public class PowerupSelectionUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("The main panel that contains all powerup choices")]
        [SerializeField] private GameObject _selectionPanel;

        [Tooltip("Powerup choice buttons (should be 3)")]
        [SerializeField] private PowerupChoiceButton[] _choiceButtons;

        [Header("References")]
        [Tooltip("PowerupManager on player")]
        [SerializeField] private PowerupManager _powerupManager;

        [Tooltip("ExperienceManager in scene")]
        [SerializeField] private ExperienceManager _experienceManager;

        #region Unity Lifecycle

        private void Start()
        {
            // Auto-find managers if not assigned
            if (_powerupManager == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    _powerupManager = player.GetComponent<PowerupManager>();
                }
            }

            if (_experienceManager == null)
            {
                _experienceManager = FindFirstObjectByType<ExperienceManager>();
            }

            // Validate
            if (_experienceManager == null)
            {
                enabled = false;
                return;
            }

            if (_powerupManager == null)
            {
                enabled = false;
                return;
            }

            // Subscribe to level-up event
            _experienceManager.OnPowerupChoice.AddListener(ShowPowerupSelection);

            // Hide panel initially
            if (_selectionPanel != null)
            {
                _selectionPanel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (_experienceManager != null)
            {
                _experienceManager.OnPowerupChoice.RemoveListener(ShowPowerupSelection);
            }
        }

        #endregion

        #region Powerup Selection

        /// <summary>
        /// Show the powerup selection UI with 3 random choices.
        /// </summary>
        private void ShowPowerupSelection()
        {
            if (_powerupManager == null || _choiceButtons == null || _choiceButtons.Length == 0)
            {
                return;
            }

            // Get 3 random powerup choices
            List<PowerupData> choices = _powerupManager.GetRandomPowerupChoices(3);

            if (choices.Count == 0)
            {
                return;
            }

            // Show panel
            if (_selectionPanel != null)
            {
                _selectionPanel.SetActive(true);
            }

            // Pause game
            Time.timeScale = 0f;

            // Setup buttons with powerup choices
            for (int i = 0; i < _choiceButtons.Length; i++)
            {
                if (_choiceButtons[i] != null)
                {
                    if (i < choices.Count)
                    {
                        // Setup button with powerup data
                        _choiceButtons[i].Setup(choices[i], this);
                        _choiceButtons[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        // Hide extra buttons if we have fewer than 3 choices
                        _choiceButtons[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// Called when player selects a powerup.
        /// </summary>
        public void OnPowerupSelected(PowerupData powerup)
        {
            if (powerup == null || _powerupManager == null)
                return;

            // Apply the selected powerup
            _powerupManager.ApplyPowerup(powerup);

            // Hide selection UI
            HideSelection();
        }

        /// <summary>
        /// Hide the powerup selection UI and resume game.
        /// </summary>
        private void HideSelection()
        {
            if (_selectionPanel != null)
            {
                _selectionPanel.SetActive(false);
            }

            // Resume game
            Time.timeScale = 1f;
        }

        #endregion
    }
}
