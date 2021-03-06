﻿using nl.SWEG.Willow.Player;
using nl.SWEG.Willow.Sorcery.Spells;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace nl.SWEG.Willow.UI.Game
{
    /// <summary>
    /// Heads-up Display for player. This controls the Overlay that is shown to the player during Game-Play
    /// </summary>
    public class PlayerHUD : MonoBehaviour
    {
        #region Variables
        #pragma warning disable 0649 // Hide Null-Warning for Editor-Variables
        #region Health
        /// <summary>
        /// Fill-UI for HealthBar
        /// </summary>
        [Header("Health")]
        [SerializeField]
        [Tooltip("Fill-UI for HealthBar")]
        private Image healthFillBar;
        #endregion

        #region Items
        /// <summary>
        /// Text-UI for Dust-Amount
        /// </summary>
        [Header("Items")]
        [SerializeField]
        [Tooltip("Text-UI for Dust-Amount")]
        private TextMeshProUGUI dustText;
        /// <summary>
        /// Text-UI for Gold-Amount
        /// </summary>
        [SerializeField]
        [Tooltip("Text-UI for Gold-Amount")]
        private TextMeshProUGUI goldText;
        #endregion

        #region Spells
        /// <summary>
        /// HUD-Objects for Selected Spells
        /// </summary>
        [Header("Spells")]
        [SerializeField]
        [Tooltip("HUD-Objects for Selected Spells")]
        private SpellHUD[] spellHuds;
        #endregion
        #pragma warning restore 0649 // Restore Null-Warning after Editor-Variables
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Initializes HUD
        /// </summary>
        public void Initialize()
        {
            OnEnable();
        }
        #endregion

        #region Unity
        /// <summary>
        /// Registers this UI to Events
        /// </summary>
        private void OnEnable()
        {
            if (PlayerManager.Exists)
            {
                PlayerManager player = PlayerManager.Instance;
                player.AddHealthChangeListener(UpdateHealthBar);
                player.Inventory?.AddDustListener(UpdateDustAmount);
                player.Inventory?.AddGoldListener(UpdateGoldAmount);
                player.CastingManager?.AddSelectionListener(UpdateSpellSelection);
                player.CastingManager?.AddSpellChangeListener(UpdateSpellUI);
                player.CastingManager?.AddCastListener(UpdateSpellCooldown);
            }
        }

        /// <summary>
        /// Unregisters this UI from Events
        /// </summary>
        private void OnDisable()
        {
            if (PlayerManager.Exists)
            {
                PlayerManager player = PlayerManager.Instance;
                player.RemoveHealthChangeListener(UpdateHealthBar);
                player.Inventory?.RemoveDustListener(UpdateDustAmount);
                player.Inventory?.RemoveGoldListener(UpdateGoldAmount);
                player.CastingManager?.RemoveSelectionListener(UpdateSpellSelection);
                player.CastingManager?.RemoveSpellChangeListener(UpdateSpellUI);
                player.CastingManager?.RemoveCastListener(UpdateSpellCooldown);
            }
        }
        #endregion

        #region Private
        #region Health
        /// <summary>
        /// Updates HealthBar
        /// </summary>
        /// <param name="newHealth">New Value for Health</param>
        /// <param name="maxHealth">Max Value for Health</param>
        /// <param name="change">Change in Value from previous</param>
        private void UpdateHealthBar(ushort newHealth, ushort maxHealth, short change)
        {
            float healthPercentage = (float)newHealth / maxHealth;
            healthFillBar.fillAmount = healthPercentage;
            if (change != 0)
            {
                // TODO: Change-Popup/Effect?
            }
        }
        #endregion

        #region Inventory
        /// <summary>
        /// Updates Dust-Amount
        /// </summary>
        /// <param name="newAmount">New amount for Dust</param>
        /// <param name="change">Change in amount</param>
        private void UpdateDustAmount(uint newAmount, int change)
        {
            dustText.text = "X " + newAmount.ToString();
            if (change != 0)
            {
                // TODO: Change-Popup/Effect?
            }
        }
        /// <summary>
        /// Updates Gold-Amount
        /// </summary>
        /// <param name="newAmount">New amount for Gold</param>
        /// <param name="change">Change in amount</param>
        private void UpdateGoldAmount(uint newAmount, int change)
        {
            goldText.text = "X " + newAmount.ToString();
            if (change != 0)
            {
                // TODO: Change-Popup/Effect?
            }
        }
        #endregion

        #region Spells
        /// <summary>
        /// Updates UI for Spell-Selection (Outline)
        /// </summary>
        /// <param name="newSelection">Index for Selection</param>
        private void UpdateSpellSelection(ushort newSelection)
        {
            for (ushort i = 0; i < spellHuds.Length; i++)
            {
                if (i == newSelection)
                    spellHuds[i].Select();
                else
                    spellHuds[i].Deselect();
            }
        }

        /// <summary>
        /// Updates UI after a Spell has been switched out
        /// </summary>
        /// <param name="index">Index for Spell that was switched</param>
        /// <param name="spellData">Data for new Spell</param>
        private void UpdateSpellUI(ushort index, SpellData spellData)
        {
            spellHuds[index].SetSpell(spellData);
            UpdateSpellCooldown(index, 0); // Set cooldown to 0 after switching
        }

        /// <summary>
        /// Updates UI for Cooldown after Casting a Spell
        /// </summary>
        /// <param name="index">Index for Spell</param>
        /// <param name="cooldown">Duration of Cooldown</param>
        private void UpdateSpellCooldown(ushort index, float cooldown)
        {
            spellHuds[index].RunCooldown(cooldown);
        }
        #endregion
        #endregion
        #endregion
    }
}