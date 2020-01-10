﻿using nl.SWEG.RPGWizardry.GameWorld;
using nl.SWEG.RPGWizardry.Loading;
using nl.SWEG.RPGWizardry.UI.GameUI;
using nl.SWEG.RPGWizardry.Utils;
using nl.SWEG.RPGWizardry.Utils.Behaviours;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace nl.SWEG.RPGWizardry
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        [SerializeField]
        private Texture2D crosshair;
        [SerializeField]
        private Texture2D cursor;

        private Vector2 crosshairHotspot;

        #region InnerTypes
        public enum GameState
        {
            Menu = 0,
            GamePlay = 1,
            GameOver = 2
        }
        #endregion

        #region Variables
        /// <summary>
        /// Current GameState
        /// </summary>
        public GameState State { get; private set; } = GameState.Menu;
        /// <summary>
        /// Whether the Game is currently Paused
        /// </summary>
        public bool Paused { get; private set; } = false;

        #region Editor
        /// <summary>
        /// Prefab for Player
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab for Player")]
        private GameObject playerPrefab;
        #endregion
        #endregion

        #region Methods
        #region Public
        /// <summary>
        /// Spawns Player
        /// </summary>
        /// <param name="position">Position (WorldSpace) to spawn Player at</param>
        public void SpawnPlayer(Vector3 position)
        {
            GameObject player = Instantiate(playerPrefab);
            player.transform.position = position;
            GameUIManager.Instance.HUD.Initialize();
        }
        /// <summary>
        /// Toggles Game-Pause
        /// </summary>
        public void TogglePause(bool setTimeScale = true)
        {
            Paused = !Paused;
            if (setTimeScale)
                Time.timeScale = Paused ? 0f : 1f; // TODO: Find a better way to pause

            //Set cursor to cursor if paused, crosshair if unpaused
            Cursor.SetCursor(Paused ? cursor : crosshair, Paused ? Vector2.zero : crosshairHotspot, CursorMode.Auto);
        }

        public void EndGame(bool gameOver)
        {
            State = GameState.GameOver;
            Cursor.visible = true;
            if (gameOver)
            {
                StartCoroutine(GameOver());
            }
            else
            {
                // TODO: Save Game
                // TODO: Load Main Menu
            }
        }
        #endregion

        #region SceneLoad
        /// <summary>
        /// Initiazes Game after GameScene has finished Loading
        /// </summary>
        /// <param name="arg0">Scene that was Loaded</param>
        /// <param name="arg1"></param>
        internal void InitGame(Scene arg0, LoadSceneMode arg1)
        {
            SceneManager.sceneLoaded -= InitGame;
            if (arg0.name != Constants.GameSceneName)
                return; // GameScene was not loaded Scene
            if (arg1 != LoadSceneMode.Single)
                return; // GameScene was not loaded Single (Menu-Exit)
            FloorManager.Instance.LoadFloor();
            State = GameState.GamePlay;
            
            Cursor.SetCursor(crosshair,crosshairHotspot,CursorMode.Auto);
        }

        /// <summary>
        /// Restarts game (un-pauses) after menu-exit
        /// </summary>
        /// <param name="arg0">Scene that was unloaded (Menu-Scene)</param>
        /// <param name="arg1">Loading-Mode for unloaded scene</param>
        internal void OnExitMenu(Scene arg0)
        {
            SceneManager.sceneUnloaded -= OnExitMenu;
            if (CameraManager.Exists && !CameraManager.Instance.AudioListener.enabled)
                CameraManager.Instance.ToggleAudio();
        }
        #endregion

        #region Unity
        private void Start()
        {
            crosshairHotspot = new Vector2(crosshair.width / 2f, crosshair.height / 2f);
        }
        #endregion

        #region Private
        private IEnumerator GameOver()
        {
            // TODO: Animation
            // TODO: Delete save game
            yield return new WaitForSeconds(2f);
            CameraManager.Instance.ToggleAudio();
            SceneLoader.Instance.LoadGameOverScene();
        }
        #endregion
        #endregion
    }
}