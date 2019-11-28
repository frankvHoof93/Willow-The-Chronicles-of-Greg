﻿using nl.SWEG.RPGWizardry.PlayerInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nl.SWEG.RPGWizardry.Avatar.Combat
{
    [RequireComponent(typeof(InputState))]
    public class AimingManager : MonoBehaviour
    {
        #region Variables
        /// <summary>
        /// Transform of the book's pivot
        /// </summary>
        [SerializeField]
        private Transform BookPivot;
        /// <summary>
        /// InputState for Aiming
        /// </summary>
        private InputState inputState;
        /// <summary>
        /// Animator for the book
        /// </summary>
        [SerializeField]
        private Animator bookAnimator;
        #endregion

        #region Methods
        #region Unity
        /// <summary>
        /// Grabs inputstate reference for aiming
        /// </summary>
        void Start()
        {
            inputState = GetComponent<InputState>();
        }

        /// <summary>
        /// Handles aiming based on Input
        /// </summary>
        void Update()
        {
            PivotToMouse();
        }

        #endregion

        #region Private
        /// <summary>
        /// Rotates a pivot to point at the mouse, aiming at it
        /// Also changes the Z position of the book to rotate around the player
        /// </summary>
        void PivotToMouse()
        {
            //Get location to look at
            Vector3 lookPos = inputState.AimingData;
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
            //Rotate to look at mouse/controller direction
            BookPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            //Change rotation parameter in animator, changing sprites
            bookAnimator.SetFloat("Rotation", BookPivot.rotation.z);

            //Change Z position, rotating around player appropriately
            if (BookPivot.rotation.z > 0)
            {
                if (BookPivot.localPosition.z < 1)
                {
                    BookPivot.localPosition = new Vector3(0, 0, 1);
                }
            }
            else if (BookPivot.rotation.z < 0)
            {
                if (BookPivot.localPosition.z > -1)
                {
                    BookPivot.localPosition = new Vector3(0, 0, -1);
                }
            }
        }
        #endregion
        #endregion
    }
}

