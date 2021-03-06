﻿using nl.SWEG.Willow.GameWorld;
using nl.SWEG.Willow.Utils.Functions;
using System.Collections.Generic;
using UnityEngine;

namespace nl.SWEG.Willow.UI.CameraEffects.Opacity
{
    /// <summary>
    /// Manages Opacity for Large Objects. This can handle many Opacity-Objects (Max 64), but is heavier on the GPU
    /// <para>
    /// To be used with the OpacityLargeShader
    /// </para>
    /// </summary>
    public class OpacityManagerLargeBatch : OpacityManager
    {
        #region Variables
        /// <summary>
        /// Cached Array for Positions
        /// </summary>
        private readonly float[] cachedPos = new float[128];
        /// <summary>
        /// Cached Array for Radii
        /// </summary>
        private readonly float[] cachedRad = new float[64];
        /// <summary>
        /// Materials to apply Opacity to
        /// </summary>
        private Material[] materials;

        #region ShaderProperties
        /// <summary>
        /// ID for _UseSeeThrough-Property
        /// </summary>
        private readonly int useSeeThroughID = Shader.PropertyToID("_UseSeeThrough");
        /// <summary>
        /// ID for _SeeThroughLength-Property
        /// </summary>
        private readonly int seeThroughLengthID = Shader.PropertyToID("_SeeThroughLength");
        /// <summary>
        /// ID for centers-Property
        /// </summary>
        private readonly int centersID = Shader.PropertyToID("centers");
        /// <summary>
        /// ID for radii-Property
        /// </summary>
        private readonly int radiiID = Shader.PropertyToID("radii");
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Sets up Materials
        /// </summary>
        private void Start()
        {
            materials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                materials[i] = renderers[i].material;
                renderers[i].material = materials[i]; // Break Batching, Create Materials
            }
        }

        /// <summary>
        /// Sets Opacity to Materials
        /// </summary>
        /// <param name="objects">Objects to set Opacity for (max 64)</param>
        protected override void SetToShader(List<OpacityObject> objects)
        {
            if (!CameraManager.Exists)
                return;
            Camera cam = CameraManager.Instance.Camera;
            int amount = Mathf.Min(objects.Count, 64);
            for (int i = 0; i < amount; i++)
            {
                OpacityObject obj = objects[i];
                cachedRad[i] = ResolutionMath.ConvertForWidth(obj.Opacity.OpacityRadius);
                // Move to ScreenSpace
                Vector3 pos = cam.WorldToScreenPoint(obj.Transform.position + (Vector3)obj.Opacity.OpacityOffset);
                cachedPos[i * 2] = pos.x;
                cachedPos[i * 2 + 1] = pos.y;
            }
            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i];
                mat.SetInt(useSeeThroughID, 1);
                mat.SetInt(seeThroughLengthID, amount);
                mat.SetFloatArray(centersID, cachedPos);
                mat.SetFloatArray(radiiID, cachedRad);
            }
        }
        #endregion
    }
}