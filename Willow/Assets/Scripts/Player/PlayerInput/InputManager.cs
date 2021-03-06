﻿using nl.SWEG.Willow.GameWorld;
using UnityEngine;
using static nl.SWEG.Willow.GameWorld.GameManager;

namespace nl.SWEG.Willow.Player.PlayerInput
{
    /// <summary>
    /// Reads and Stores Input from Input-Devices
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        #region Variables
        #region Public
        /// <summary>
        /// Current State for Input
        /// </summary>
        public InputState State { get; private set; }
        #endregion

        #region Editor
        #pragma warning disable 0649 // Hide Null-Warning for Editor-Variables
        /// <summary>
        /// ControlScheme for Input-Reading
        /// </summary>
        [SerializeField]
        [Tooltip("ControlScheme for Input-Reading")]
        private ControlScheme controlScheme = ControlScheme.Keyboard;
        
        /// <summary>
        /// Pivot on which the book rotates; Necessary to aim at the mouse properly
        /// </summary>
        [SerializeField]
        [Tooltip("Pivot on which the book rotates")]
        private Transform bookPivot;
        #pragma warning restore 0649 // Restore Null-Warning after Editor-Variables
        #endregion
        #endregion

        #region Methods
        #region Unity
        /// <summary>
        /// Checks Input-Devices based on GameState
        /// </summary>
        private void Update()
        {
            InputState newState = new InputState();
            switch (GameManager.Instance.State)
            {
                case GameState.GamePlay:
                    MovementInputs(ref newState);
                    AimingInputs(ref newState);
                    ButtonInputs(ref newState);
                    break;
            }
            State = newState;
        }
        #endregion

        #region Private
        /// <summary>
        /// Collects Movement-Input
        /// </summary>
        private void MovementInputs(ref InputState inputState)
        {
            // Same implementation for keyboard and controller
            Vector2 movementDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            // Normalize if needed (Can have length <= 1)
            if (movementDir.magnitude > 1)
                movementDir.Normalize();
            inputState.MovementDirection = movementDir;
        }

        /// <summary>
        /// Collects Aiming-Input
        /// </summary>
        /// <param name="inputState">InputState to write to</param>
        private void AimingInputs(ref InputState inputState)
        {
            switch (controlScheme)
            {
                // On controller, use the right stick
                case ControlScheme.Controller:
                    inputState.AimDirection = new Vector2(Input.GetAxis("RightX"), Input.GetAxis("RightY")).normalized;
                    break;
                // On keyboard, use the mouse
                default:
                    Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    Vector2 lookPos = CameraManager.Instance.Camera.ScreenToWorldPoint(mousePos);
                    inputState.AimDirection = (lookPos - (Vector2)bookPivot.transform.position).normalized;
                    break;
            }
        }

        /// <summary>
        /// Collects button/key states
        /// </summary>
        /// <param name="inputState">InputState to write to</param>
        private void ButtonInputs(ref InputState inputState)
        {
            switch (controlScheme)
            {
                case ControlScheme.Controller:
                    int? index = null;
                    for (int i = 4; i > 0; i--) // Check from highest to lowest index, to ensure lowest held index is set in State
                        if (Input.GetButton($"Fire{i}"))
                        {
                            inputState.Cast = true;
                            index = i;
                        }
                    inputState.CastIndex = index;
                    break;
                default: // Keyboard
                    inputState.Cast = Input.GetButton("Fire1");
                    // Spell-Selection
                    if (Input.GetKeyDown(KeyCode.Alpha1)) // Select Index 1
                        inputState.SelectSpell = InputState.SpellSelection.Index1;
                    else if (Input.GetKeyDown(KeyCode.Alpha2)) // Select Index 2
                        inputState.SelectSpell = InputState.SpellSelection.Index2;
                    else if (Input.GetKeyDown(KeyCode.Alpha3)) // Select Index 3
                        inputState.SelectSpell = InputState.SpellSelection.Index3;
                    else if (Input.GetKeyDown(KeyCode.Alpha4)) // Select Index 4
                        inputState.SelectSpell = InputState.SpellSelection.Index4;
                    else if (Input.mouseScrollDelta.y > 0) // Select Next (Scroll Up)
                        inputState.SelectSpell = InputState.SpellSelection.SelectPrevious;
                    else if (Input.mouseScrollDelta.y < 0) // Select Previous (Scroll Down)
                        inputState.SelectSpell = InputState.SpellSelection.SelectNext;
                    break;
            }
        }
        #endregion
        #endregion
    }
}