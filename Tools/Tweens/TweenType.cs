using System;
using UnityEngine;

namespace GameKit.Tools
{
    public enum TweenDefinitionTypes { MMTween, AnimationCurve }

    [Serializable]
    public class TweenType
    {
        public TweenDefinitionTypes TweenDefinitionType = TweenDefinitionTypes.MMTween;
        public Tween.TweenCurve TweenCurve = Tween.TweenCurve.EaseInCubic;
        public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1f));

        public TweenType(Tween.TweenCurve newCurve)
        {
            TweenCurve = newCurve;
            TweenDefinitionType = TweenDefinitionTypes.MMTween;
        }
        public TweenType(AnimationCurve newCurve)
        {
            Curve = newCurve;
            TweenDefinitionType = TweenDefinitionTypes.AnimationCurve;
        }
    }
}
