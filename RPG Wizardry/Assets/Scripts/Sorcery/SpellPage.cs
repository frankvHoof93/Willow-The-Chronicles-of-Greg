﻿using UnityEngine;

namespace nl.SWEG.RPGWizardry.Sorcery
{
    public class SpellPage
    {
        #region Variables
        /// <summary>
        /// Whether the Spell on this Page has been Unlocked
        /// </summary>
        public bool Unlocked { get; private set; }

        /// <summary>
        /// The dust cost for the spell to unlock.
        /// </summary>
        public uint DustCost { get; private set; }
        /// <summary>
        /// Data for Spell
        /// </summary>
        [SerializeField]
        [Tooltip("Data for Spell")]
        private Spell spell;
        #endregion

        #region Methods
        /// <summary>
        /// Unlocks Spell on this Page (after crafting)
        /// </summary>
        internal void UnlockSpell()
        {
            Unlocked = true;
        }



        /// <summary>
        /// Retrievies the Dust cost from the Spell on this Page
        /// </summary>
        /// <returns>The Dust cost of the spell</returns>
        internal uint GetDustCost()
        {
            return spell.DustCost;
        }
        #endregion
    }
}