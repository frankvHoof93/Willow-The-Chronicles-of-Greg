﻿using nl.SWEG.Willow.Sorcery;
using nl.SWEG.Willow.Sorcery.Spells;
using nl.SWEG.Willow.Utils.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace nl.SWEG.Willow.Player.Inventory
{
    /// <summary>
    /// Inventory for Player, containing Dust, Gold and SpellPages
    /// </summary>
    public class PlayerInventory : MonoBehaviour, IStorable, IJSON<PlayerInventory>
    {
        #region InnerTypes
        /// <summary>
        /// Delegate for Changes to Inventory
        /// </summary>
        /// <param name="newAmount">Amount after Change</param>
        /// <param name="change">Change that was applied</param>
        public delegate void OnInventoryChange(uint newAmount, int change);
        /// <summary>
        /// Delegate for Inventory-Events with a SpellPage
        /// </summary>
        /// <param name="page">SpellPage for Event</param>
        public delegate void OnInventorySpell(SpellPage page);
        #endregion

        #region Variables
        #region Public
        /// <summary>
        /// Amount of Dust in Inventory
        /// </summary>
        public uint Dust { get; private set; }
        /// <summary>
        /// Amount of Gold in Inventory
        /// </summary>
        public uint Gold { get; private set; }
        /// <summary>
        /// Pages in Inventory
        /// </summary>
        public IReadOnlyList<SpellPage> Pages => pages.AsReadOnly();
        #endregion

        #region Editor
        #pragma warning disable 0649 // Hide Null-Warning for Editor-Variables
        /// <summary>
        /// Base spell the player has access too
        /// </summary>
        [SerializeField]
        [Tooltip("Base spell the player has access too")]
        private SpellData baseSpell;
        #pragma warning restore 0649 // Restore Null-Warning after Editor-Variables
        #endregion

        #region Private
        /// <summary>
        /// Pages in Inventory
        /// </summary>
        private readonly List<SpellPage> pages = new List<SpellPage>();

        #region Events
        /// <summary>
        /// Event called when Dust-Amount changes
        /// </summary>
        private event OnInventoryChange dustChangeEvent;
        /// <summary>
        /// Event called when Gold-Amount changes
        /// </summary>
        private event OnInventoryChange goldChangeEvent;
        /// <summary>
        /// Event called when Page-Amount changes
        /// </summary>
        private event OnInventorySpell pageChangeEvent;
        /// <summary>
        /// Event called when Spell is Unlocked
        /// </summary>
        private event OnInventorySpell spellUnlocked;
        #endregion
        #endregion
        #endregion

        #region Methods
        #region Public
        #region EventListeners
        /// <summary>
        /// Adds Listener to Dust-Event
        /// </summary>
        /// <param name="listener">Listener to Add</param>
        public void AddDustListener(OnInventoryChange listener)
        {
            dustChangeEvent += listener;
        }

        /// <summary>
        /// Removes Listener from Dust-Event
        /// </summary>
        /// <param name="listener">Listener to Remove</param>
        public void RemoveDustListener(OnInventoryChange listener)
        {
            dustChangeEvent -= listener;
        }

        /// <summary>
        /// Adds Listener to Gold-Event
        /// </summary>
        /// <param name="listener">Listener to Add</param>
        public void AddGoldListener(OnInventoryChange listener)
        {
            goldChangeEvent += listener;
            // Set Initial Value
            listener.Invoke(Gold, 0);
        }

        /// <summary>
        /// Removes Listener from Gold-Event
        /// </summary>
        /// <param name="listener">Listener to Remove</param>
        public void RemoveGoldListener(OnInventoryChange listener)
        {
            goldChangeEvent -= listener;
        }

        /// <summary>
        /// Add Listener from Page-Event
        /// </summary>
        /// <param name="listener">Listener to Add</param>
        public void AddPageListener(OnInventorySpell listener)
        {
            pageChangeEvent += listener;
        }

        /// <summary>
        /// Remove Listener from Page-Event
        /// </summary>
        /// <param name="listener">Listener to Remove</param>
        public void RemovePageListener(OnInventorySpell listener)
        {
            pageChangeEvent -= listener;
        }

        /// <summary>
        /// Adds Listener to SpellUnlock-Event
        /// </summary>
        /// <param name="listener">Listener to Add</param>
        public void AddUnlockListener(OnInventorySpell listener)
        {
            spellUnlocked += listener;
        }

        /// <summary>
        /// Removes Listener from SpellUnlock-Event
        /// </summary>
        /// <param name="listener">Listener to Remove</param>
        public void RemoveUnlockListener(OnInventorySpell listener)
        {
            spellUnlocked -= listener;
        }
        #endregion

        #region Spells
        /// <summary>
        /// Unlocks a Page in the Inventory, but only if there is enough dust to pay for its cost
        /// </summary>
        /// <param name="page">Page to unlock</param>
        /// <returns>True if unlock was successful</returns>
        public bool UnlockSpell(SpellPage page)
        {
            if (Dust >= page.DustCost)
            {
                page.UnlockSpell();
                spellUnlocked?.Invoke(page);
                Dust -= page.DustCost;
                dustChangeEvent?.Invoke(Dust, (int)page.DustCost);
            }
            return page.Unlocked;
        }
        #endregion

        #region Storage
        /// <summary>
        /// TODO:
        /// Loads Inventory from File
        /// </summary>
        /// <param name="path">Path to load from</param>
        public void Load(string path)
        {
            string json = File.ReadAllText(path);
            PlayerInventory toLoad = FromJSON(json);
            // TODO: Load variables
            // Called with 0 to not perform animation
            dustChangeEvent?.Invoke(Dust, 0);
            goldChangeEvent?.Invoke(Gold, 0);
        }

        /// <summary>
        /// TODO:
        /// Saves Inventory to File
        /// </summary>
        public void Save(string path)
        {
            File.WriteAllText(path, ToJSON());
        }

        /// <summary>
        /// TODO:
        /// Creates JSON-representation for this Object
        /// </summary>
        /// <returns>JSON-String for this Inventory</returns>
        public string ToJSON()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// TODO:
        /// Loads Inventory-Values from JSON
        /// </summary>
        /// <returns>JSON-string to load from</returns>
        public PlayerInventory FromJSON(string json)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Getters
        /// <summary>
        /// Whether the player has this spell in his/her inventory (does not check for Unlocking)
        /// </summary>
        /// <param name="spell">Spell to check against</param>
        /// <returns>True if Spell exists in Inventory</returns>
        public bool HasSpell(SpellData spell)
        {
            return Pages.Any(p => ReferenceEquals(spell, p.Spell));
        }
        #endregion
        #endregion

        #region Unity
        /// <summary>
        /// Sets default Spell (Bookerang)
        /// </summary>
        private void Start()
        {
            SpellPage bookerang = new SpellPage(baseSpell, true);
            pages.Add(bookerang);
            PlayerManager.Instance.CastingManager.TryEquipSpell(bookerang.Spell);
        }
        #endregion

        #region Internal
        #region Add
        /// <summary>
        /// Adds Page to Inventory, if it does not exist in Inventory yet
        /// </summary>
        /// <param name="page">Page to add</param>
        internal bool AddPage(SpellPage page)
        {
            if (page != null && !HasSpell(page.Spell))
            {
                pages.Add(page);
                pageChangeEvent?.Invoke(page);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Adds Dust to Inventory
        /// </summary>
        /// <param name="amount">Amount of Dust to add (>= 0)</param>
        internal void AddDust(uint amount)
        {
            Dust += amount;
            dustChangeEvent?.Invoke(Dust, (int)amount);
        }

        /// <summary>
        /// Adds Gold to Inventory
        /// </summary>
        /// <param name="amount">Amount of Gold to add (>= 0)</param>
        internal void AddGold(uint amount)
        {
            Gold += amount;
            goldChangeEvent?.Invoke(Gold, (int)amount);
        }
        #endregion
        #endregion
        #endregion
    }
}
