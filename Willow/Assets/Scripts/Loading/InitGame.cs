﻿using UnityEngine;

namespace nl.SWEG.Willow.Loading
{
    /// <summary>
    /// Initializes Game
    /// </summary>
    public class InitGame : MonoBehaviour
    {
        /// <summary>
        /// Runs Intro, then Loads Main Menu
        /// </summary>
        private void Start()
        {
            // TODO: run Intro
            SceneLoader.Instance.LoadMenuScene(); // Start game by loading menu-scene
        }
    }
}