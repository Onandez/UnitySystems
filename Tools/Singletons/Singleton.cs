using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Tools
{
    // Singleton pattern.
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;

        // Singleton 
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        // Initialize our instance. Make sure to call base.Awake() in override if you need awake.
        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            _instance = this as T;
        }
    }
}
