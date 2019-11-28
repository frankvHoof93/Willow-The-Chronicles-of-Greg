﻿using nl.SWEG.RPGWizardry.PlayerInput;
using nl.SWEG.RPGWizardry.Sorcery.Spells;
using System.Collections;
using UnityEngine;

namespace nl.SWEG.RPGWizardry.Avatar.Combat
{
    [RequireComponent(typeof(InputState))]
    public class CastingManager : MonoBehaviour
    {
        #region Variables
        #region Public
        /// <summary>
        /// Prototype projectile; fill this with selected spell later
        /// </summary>
        public SpellData CurrentSpell;
        #endregion

        #region Private
        /// <summary>
        /// Transform of the object the projectiles need to spawn from
        /// </summary>
        [SerializeField]
        private Transform spawnLocation;

        [SerializeField]
        private LayerMask targetingMask;
        /// <summary>
        /// Inputstate for getting button states
        /// </summary>
        private InputState inputState;
        /// <summary>
        /// Cooldown state during which you cannot cast spells
        /// </summary>
        private bool cooldown;
        #endregion
        #endregion

        #region Methods
        #region Unity
        /// <summary>
        /// Grabs inputstate reference for button presses
        /// </summary>
        void Start()
        {
            inputState = GetComponent<InputState>();
        }

        /// <summary>
        /// If the button is pressed and there's no cooldown, fire the spell
        /// </summary>
        void Update()
        {
            if (inputState.Cast1)
            {
                if (!cooldown)
                {
                    SpawnProjectile();
                }
            }
        }
        #endregion

        #region Private

        /// <summary>
        /// Spawns a projectile, then puts the casting system on cooldown based on the spell
        /// </summary>
        /// <param name="projectile">Projectile of the spell to cast; contains cooldown value</param>
        private void SpawnProjectile()
        {
            //spawn projectile at book's location
            CurrentSpell.SpawnSpell(spawnLocation.position, spawnLocation.up, targetingMask);
            //start cooldown
            cooldown = true;
            StartCoroutine(Cooldown(1f));
        }

        /// <summary>
        /// Cooldown state during which no spells may be cast
        /// </summary>
        /// <param name="coolSeconds">Seconds the cooldown should remain active</param>
        /// <returns></returns>
        IEnumerator Cooldown(float coolSeconds)
        {
            yield return new WaitForSeconds(coolSeconds);
            cooldown = false;

        }
        #endregion
        #endregion
    }
}