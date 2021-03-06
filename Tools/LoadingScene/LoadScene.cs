﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameKit.Tools
{
    /// <summary>
    /// Add this component on an object, specify a scene name in its inspector, and call LoadScene() to load the desired scene.
    /// </summary>
    public class LoadScene : MonoBehaviour
    {
        /// the possible modes to load scenes. Either Unity's native API, or MoreMountains' LoadingSceneManager
        public enum LoadingSceneModes { UnityNative, MMLoadingSceneManager }

        /// the name of the scene that needs to be loaded when LoadScene gets called
        public string SceneName;
        /// defines whether the scene will be loaded using Unity's native API or MoreMountains' way
        public LoadingSceneModes LoadingSceneMode = LoadingSceneModes.UnityNative;

        /// <summary>
        /// Loads the scene specified in the inspector
        /// </summary>
        public virtual void LoadSceneProcess()
        {
            if (LoadingSceneMode == LoadingSceneModes.UnityNative)
            {
                SceneManager.LoadScene(SceneName);
            }
            if (LoadingSceneMode == LoadingSceneModes.MMLoadingSceneManager)
            {
                LoadingSceneManager.LoadScene(SceneName);
            }
        }
    }
}
