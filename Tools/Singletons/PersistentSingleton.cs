using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Tools
{
    // Persistent singleton pattern.
    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;
        protected bool _enabled;


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

        //Check if there's already a copy of the object in the scene. If there's one, we destroy it.
        protected virtual void Awake()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (_instance == null)
            {
                //If is first instance, make it Singleton
                _instance = this as T;
                DontDestroyOnLoad(transform.gameObject);
                _enabled = true;
            }
            else
            {
                //If a Singleton already exists and find another reference in scene, destroy it
                if (this != _instance)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
