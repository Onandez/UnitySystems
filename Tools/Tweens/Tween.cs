using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Tools
{
    /// <summary>
    /// The formulas described here are (loosely) based on Robert Penner's easing equations http://robertpenner.com/easing/
    /// I recommend reading this blog post if you're interested in the subject : http://blog.moagrius.com/actionscript/jsas-understanding-easing/
    /// </summary>

    public class Tween : MonoBehaviour
    {
        // A list of all the possible curves you can tween a value along
        public enum TweenCurve
        {
            LinearTween,
            EaseInQuadratic,    EaseOutQuadratic,   EaseInOutQuadratic,
            EaseInCubic,        EaseOutCubic,       EaseInOutCubic,
            EaseInQuartic,      EaseOutQuartic,     EaseInOutQuartic,
            EaseInQuintic,      EaseOutQuintic,     EaseInOutQuintic,
            EaseInSinusoidal,   EaseOutSinusoidal,  EaseInOutSinusoidal,
            EaseInBounce,       EaseOutBounce,      EaseInOutBounce,
            EaseInOverhead,     EaseOutOverhead,    EaseInOutOverhead,
            EaseInExponential,  EaseOutExponential, EaseInOutExponential,
            EaseInElastic,      EaseOutElastic,     EaseInOutElastic,
            EaseInCircular,     EaseOutCircular,    EaseInOutCircular,
            AntiLinearTween
        }

        // Core methods ---------------------------------------------------------------------------------------------------------------

        // Moves a value between a startValue and an endValue based on a currentTime, along the specified tween curve
        public static float TweenConstructor(float currentTime, float initialTime, float endTime, float startValue, float endValue, TweenCurve curve)
        {
            currentTime = MathsHelper.Remap(currentTime, initialTime, endTime, 0f, 1f);
            switch (curve)
            {
                case TweenCurve.LinearTween: currentTime = TweenDefinitions.Linear_Tween(currentTime); break;
                case TweenCurve.AntiLinearTween: currentTime = TweenDefinitions.LinearAnti_Tween(currentTime); break;

                case TweenCurve.EaseInQuadratic: currentTime = TweenDefinitions.EaseIn_Quadratic(currentTime); break;
                case TweenCurve.EaseOutQuadratic: currentTime = TweenDefinitions.EaseOut_Quadratic(currentTime); break;
                case TweenCurve.EaseInOutQuadratic: currentTime = TweenDefinitions.EaseInOut_Quadratic(currentTime); break;

                case TweenCurve.EaseInCubic: currentTime = TweenDefinitions.EaseIn_Cubic(currentTime); break;
                case TweenCurve.EaseOutCubic: currentTime = TweenDefinitions.EaseOut_Cubic(currentTime); break;
                case TweenCurve.EaseInOutCubic: currentTime = TweenDefinitions.EaseInOut_Cubic(currentTime); break;

                case TweenCurve.EaseInQuartic: currentTime = TweenDefinitions.EaseIn_Quartic(currentTime); break;
                case TweenCurve.EaseOutQuartic: currentTime = TweenDefinitions.EaseOut_Quartic(currentTime); break;
                case TweenCurve.EaseInOutQuartic: currentTime = TweenDefinitions.EaseInOut_Quartic(currentTime); break;

                case TweenCurve.EaseInQuintic: currentTime = TweenDefinitions.EaseIn_Quintic(currentTime); break;
                case TweenCurve.EaseOutQuintic: currentTime = TweenDefinitions.EaseOut_Quintic(currentTime); break;
                case TweenCurve.EaseInOutQuintic: currentTime = TweenDefinitions.EaseInOut_Quintic(currentTime); break;

                case TweenCurve.EaseInSinusoidal: currentTime = TweenDefinitions.EaseIn_Sinusoidal(currentTime); break;
                case TweenCurve.EaseOutSinusoidal: currentTime = TweenDefinitions.EaseOut_Sinusoidal(currentTime); break;
                case TweenCurve.EaseInOutSinusoidal: currentTime = TweenDefinitions.EaseInOut_Sinusoidal(currentTime); break;

                case TweenCurve.EaseInBounce: currentTime = TweenDefinitions.EaseIn_Bounce(currentTime); break;
                case TweenCurve.EaseOutBounce: currentTime = TweenDefinitions.EaseOut_Bounce(currentTime); break;
                case TweenCurve.EaseInOutBounce: currentTime = TweenDefinitions.EaseInOut_Bounce(currentTime); break;

                case TweenCurve.EaseInOverhead: currentTime = TweenDefinitions.EaseIn_Overhead(currentTime); break;
                case TweenCurve.EaseOutOverhead: currentTime = TweenDefinitions.EaseOut_Overhead(currentTime); break;
                case TweenCurve.EaseInOutOverhead: currentTime = TweenDefinitions.EaseInOut_Overhead(currentTime); break;

                case TweenCurve.EaseInExponential: currentTime = TweenDefinitions.EaseIn_Exponential(currentTime); break;
                case TweenCurve.EaseOutExponential: currentTime = TweenDefinitions.EaseOut_Exponential(currentTime); break;
                case TweenCurve.EaseInOutExponential: currentTime = TweenDefinitions.EaseInOut_Exponential(currentTime); break;

                case TweenCurve.EaseInElastic: currentTime = TweenDefinitions.EaseIn_Elastic(currentTime); break;
                case TweenCurve.EaseOutElastic: currentTime = TweenDefinitions.EaseOut_Elastic(currentTime); break;
                case TweenCurve.EaseInOutElastic: currentTime = TweenDefinitions.EaseInOut_Elastic(currentTime); break;

                case TweenCurve.EaseInCircular: currentTime = TweenDefinitions.EaseIn_Circular(currentTime); break;
                case TweenCurve.EaseOutCircular: currentTime = TweenDefinitions.EaseOut_Circular(currentTime); break;
                case TweenCurve.EaseInOutCircular: currentTime = TweenDefinitions.EaseInOut_Circular(currentTime); break;

            }
            return startValue + currentTime * (endValue - startValue);
        }

        public static Vector2 TweenConstructor(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, TweenCurve curve)
        {
            startValue.x = TweenConstructor(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = TweenConstructor(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            return startValue;
        }

        public static Vector3 TweenConstructor(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, TweenCurve curve)
        {
            startValue.x = TweenConstructor(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = TweenConstructor(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            startValue.z = TweenConstructor(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
            return startValue;
        }

        public static Quaternion TweenConstructor(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, TweenCurve curve)
        {
            float turningRate = TweenConstructor(currentTime, initialTime, endTime, 0f, 1f, curve);
            startValue = Quaternion.Slerp(startValue, endValue, turningRate);
            return startValue;
        }

        // Animation curve methods --------------------------------------------------------------------------------------------------------------

        public static float TweenConstructor(float currentTime, float initialTime, float endTime, float startValue, float endValue, AnimationCurve curve)
        {
            currentTime = MathsHelper.Remap(currentTime, initialTime, endTime, 0f, 1f);
            currentTime = curve.Evaluate(currentTime);
            return startValue + currentTime * (endValue - startValue);
        }

        public static Vector2 TweenConstructor(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, AnimationCurve curve)
        {
            startValue.x = TweenConstructor(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = TweenConstructor(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            return startValue;
        }

        public static Vector3 TweenConstructor(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, AnimationCurve curve)
        {
            startValue.x = TweenConstructor(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
            startValue.y = TweenConstructor(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
            startValue.z = TweenConstructor(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
            return startValue;
        }

        public static Quaternion TweenConstructor(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, AnimationCurve curve)
        {
            float turningRate = TweenConstructor(currentTime, initialTime, endTime, 0f, 1f, curve);
            startValue = Quaternion.Slerp(startValue, endValue, turningRate);
            return startValue;
        }

        // Tween type methods ------------------------------------------------------------------------------------------------------------------------

        public static float TweenConstructor(float currentTime, float initialTime, float endTime, float startValue, float endValue, TweenType tweenType)
        {
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.MMTween)
            {
                return TweenConstructor(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
            }
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return TweenConstructor(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return 0f;
        }
        public static Vector2 TweenConstructor(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, TweenType tweenType)
        {
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.MMTween)
            {
                return TweenConstructor(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
            }
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return TweenConstructor(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return Vector2.zero;
        }
        public static Vector3 TweenConstructor(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, TweenType tweenType)
        {
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.MMTween)
            {
                return TweenConstructor(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
            }
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return TweenConstructor(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return Vector3.zero;
        }
        public static Quaternion TweenConstructor(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, TweenType tweenType)
        {
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.MMTween)
            {
                return TweenConstructor(currentTime, initialTime, endTime, startValue, endValue, tweenType.TweenCurve);
            }
            if (tweenType.TweenDefinitionType == TweenDefinitionTypes.AnimationCurve)
            {
                return TweenConstructor(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
            }
            return Quaternion.identity;
        }

        // MOVE METHODS ---------------------------------------------------------------------------------------------------------

        public static Coroutine MoveTransform(MonoBehaviour mono, Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration, TweenCurve curve, bool updatePosition = true, bool updateRotation = true)
        {
            return mono.StartCoroutine(MoveTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, updatePosition, updateRotation));
        }

        public static Coroutine RotateTransformAround(MonoBehaviour mono, Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration, float duration, TweenCurve curve)
        {
            return mono.StartCoroutine(RotateTransformAroundCo(targetTransform, center, destination, angle, delay, delayDuration, duration, curve));
        }

        protected static IEnumerator MoveTransformCo(Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration, TweenCurve curve, bool updatePosition = true, bool updateRotation = true)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }
            float timeLeft = duration;
            while (timeLeft > 0f)
            {
                if (updatePosition)
                {
                    targetTransform.transform.position = TweenConstructor(duration - timeLeft, 0f, duration, origin.position, destination.position, curve);
                }
                if (updateRotation)
                {
                    targetTransform.transform.rotation = TweenConstructor(duration - timeLeft, 0f, duration, origin.rotation, destination.rotation, curve);
                }
                timeLeft -= Time.deltaTime;
                yield return null;
            }
            if (updatePosition) { targetTransform.transform.position = destination.position; }
            if (updateRotation) { targetTransform.transform.localEulerAngles = destination.localEulerAngles; }
        }

        protected static IEnumerator RotateTransformAroundCo(Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration, float duration, TweenCurve curve)
        {
            if (delayDuration > 0f)
            {
                yield return delay;
            }

            Vector3 initialRotationPosition = targetTransform.transform.position;
            Quaternion initialRotationRotation = targetTransform.transform.rotation;

            float rate = 1f / duration;

            float timeSpent = 0f;
            while (timeSpent < duration)
            {

                float newAngle = TweenConstructor(timeSpent, 0f, duration, 0f, angle, curve);

                targetTransform.transform.position = initialRotationPosition;
                initialRotationRotation = targetTransform.transform.rotation;
                targetTransform.RotateAround(center.transform.position, center.transform.up, newAngle);
                targetTransform.transform.rotation = initialRotationRotation;

                timeSpent += Time.deltaTime;
                yield return null;
            }
            targetTransform.transform.position = destination.position;
        }
    }
}
