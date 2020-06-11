﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Tools
{
    /// <summary>
    /// Image helpers
    /// </summary>
    public class ImageHelper : MonoBehaviour
    {
        /// <summary>
        /// Coroutine used to make the character's sprite flicker (when hurt for example).
        /// </summary>
        public static IEnumerator Flicker(Renderer renderer, Color initialColor, Color flickerColor, float flickerSpeed, float flickerDuration)
        {
            if (renderer == null)
            {
                yield break;
            }

            if (!renderer.material.HasProperty("_Color"))
            {
                yield break;
            }

            if (initialColor == flickerColor)
            {
                yield break;
            }

            float flickerStop = Time.time + flickerDuration;

            while (Time.time < flickerStop)
            {
                renderer.material.color = flickerColor;
                yield return new WaitForSeconds(flickerSpeed);
                renderer.material.color = initialColor;
                yield return new WaitForSeconds(flickerSpeed);
            }

            renderer.material.color = initialColor;
        }
    }
}
