using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Tools
{
    public static class CoroutineHelper
    {
        public static IEnumerator WaitForFrames(int frameCount)
        {
            while (frameCount > 0)
            {
                frameCount--;
                yield return null;
            }
        }
    }
}
