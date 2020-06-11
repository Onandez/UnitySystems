using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameKit.Tools
{
    /// <summary>
    /// Events used to trigger faders on or off
    /// </summary>
    public struct FadeEvent
    {
        /// an ID that has to match the one on the fader
        public int ID;
        /// the duration of the fade, in seconds
        public float Duration;
        /// the alpha to aim for
        public float TargetAlpha;
        /// the curve to apply to the fade
        public TweenType Curve;
        /// whether or not this fade should ignore timescale
        public bool IgnoreTimeScale;
        /// a world position for a target object. Useless for regular fades, but can be useful for alt implementations (circle fade for example)
        public Vector3 WorldPosition;


        /// <summary>
        /// Initializes a new instance of the <see cref="MoreMountains.MMInterface.MMFadeEvent"/> struct.
        public FadeEvent(float duration, float targetAlpha, TweenType tween, int id = 0,
            bool ignoreTimeScale = true, Vector3 worldPosition = new Vector3())
        {
            ID = id;
            Duration = duration;
            TargetAlpha = targetAlpha;
            Curve = tween;
            IgnoreTimeScale = ignoreTimeScale;
            WorldPosition = worldPosition;
        }
        static FadeEvent e;
        public static void Trigger(float duration, float targetAlpha, TweenType tween, int id = 0,
            bool ignoreTimeScale = true, Vector3 worldPosition = new Vector3())
        {
            e.ID = id;
            e.Duration = duration;
            e.TargetAlpha = targetAlpha;
            e.Curve = tween;
            e.IgnoreTimeScale = ignoreTimeScale;
            e.WorldPosition = worldPosition;
            EventManager.TriggerEvent(e);
        }
    }

    public struct FadeInEvent
    {
        /// an ID that has to match the one on the fader
        public int ID;
        /// the duration of the fade, in seconds
        public float Duration;
        /// the curve to apply to the fade
        public TweenType Curve;
        /// whether or not this fade should ignore timescale
        public bool IgnoreTimeScale;
        /// a world position for a target object. Useless for regular fades, but can be useful for alt implementations (circle fade for example)
        public Vector3 WorldPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoreMountains.MMInterface.MMFadeInEvent"/> struct.
        /// </summary>
        /// <param name="duration">Duration.</param>
        public FadeInEvent(float duration, TweenType tween, int id = 0,
            bool ignoreTimeScale = true, Vector3 worldPosition = new Vector3())
        {
            ID = id;
            Duration = duration;
            Curve = tween;
            IgnoreTimeScale = ignoreTimeScale;
            WorldPosition = worldPosition;
        }
        static FadeInEvent e;
        public static void Trigger(float duration, TweenType tween, int id = 0,
            bool ignoreTimeScale = true, Vector3 worldPosition = new Vector3())
        {
            e.ID = id;
            e.Duration = duration;
            e.Curve = tween;
            e.IgnoreTimeScale = ignoreTimeScale;
            e.WorldPosition = worldPosition;
            EventManager.TriggerEvent(e);
        }
    }

    public struct FadeOutEvent
    {
        /// an ID that has to match the one on the fader
        public int ID;
        /// the duration of the fade, in seconds
        public float Duration;
        /// the curve to apply to the fade
        public TweenType Curve;
        /// whether or not this fade should ignore timescale
        public bool IgnoreTimeScale;
        /// a world position for a target object. Useless for regular fades, but can be useful for alt implementations (circle fade for example)
        public Vector3 WorldPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="MoreMountains.MMInterface.MMFadeOutEvent"/> struct.
        /// </summary>
        /// <param name="duration">Duration.</param>
        public FadeOutEvent(float duration, TweenType tween, int id = 0,
            bool ignoreTimeScale = true, Vector3 worldPosition = new Vector3())
        {
            ID = id;
            Duration = duration;
            Curve = tween;
            IgnoreTimeScale = ignoreTimeScale;
            WorldPosition = worldPosition;
        }

        static FadeOutEvent e;
        public static void Trigger(float duration, TweenType tween, int id = 0,
            bool ignoreTimeScale = true, Vector3 worldPosition = new Vector3())
        {
            e.ID = id;
            e.Duration = duration;
            e.Curve = tween;
            e.IgnoreTimeScale = ignoreTimeScale;
            e.WorldPosition = worldPosition;
            EventManager.TriggerEvent(e);
        }
    }

    /// <summary>
    /// The Fader class can be put on an Image, and it'll intercept MMFadeEvents and turn itself on or off accordingly.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("More Mountains/Tools/GUI/MMFader")]
    public class Fader : MonoBehaviour, EventListener<FadeEvent>, EventListener<FadeInEvent>, EventListener<FadeOutEvent>
    {
        [Header("Identification")]
        /// the ID for this fader (0 is default), set more IDs if you need more than one fader
        public int ID;
        [Header("Opacity")]
        /// the opacity the fader should be at when inactive
        public float InactiveAlpha = 0f;
        /// the opacity the fader should be at when active
        public float ActiveAlpha = 1f;
        [Header("Timing")]
        /// the default duration of the fade in/out
        public float DefaultDuration = 0.2f;
        /// the default curve to use for this fader
        public TweenType DefaultTween = new TweenType(Tween.TweenCurve.LinearTween);
        /// whether or not the fade should happen in unscaled time 
        public bool IgnoreTimescale = true;
        [Header("Interaction")]
        /// whether or not the fader should block raycasts when visible
        public bool ShouldBlockRaycasts = false;

        [Header("Debug")]
        public bool FadeIn1SecondButton;
        public bool FadeOut1SecondButton;
        public bool DefaultFadeButton;
        public bool ResetFaderButton;

        protected CanvasGroup _canvasGroup;
        protected Image _image;

        protected float _initialAlpha;
        protected float _currentTargetAlpha;
        protected float _currentDuration;
        protected TweenType _currentCurve;

        protected bool _fading = false;
        protected float _fadeStartedAt;

        /// <summary>
        /// Test method triggered by an inspector button
        /// </summary>
        protected virtual void ResetFader()
        {
            _canvasGroup.alpha = InactiveAlpha;
        }

        /// <summary>
        /// Test method triggered by an inspector button
        /// </summary>
        protected virtual void DefaultFade()
        {
            FadeEvent.Trigger(DefaultDuration, ActiveAlpha, DefaultTween, ID);
        }

        /// <summary>
        /// Test method triggered by an inspector button
        /// </summary>
        protected virtual void FadeIn1Second()
        {
            FadeInEvent.Trigger(1f, new TweenType(Tween.TweenCurve.LinearTween));
        }

        /// <summary>
        /// Test method triggered by an inspector button
        /// </summary>
        protected virtual void FadeOut1Second()
        {
            FadeOutEvent.Trigger(1f, new TweenType(Tween.TweenCurve.LinearTween));
        }

        /// <summary>
        /// On Start, we initialize our fader
        /// </summary>
        protected virtual void Awake()
        {
            Initialization();
        }

        /// <summary>
        /// On init, we grab our components, and disable/hide everything
        /// </summary>
        protected virtual void Initialization()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = InactiveAlpha;

            _image = GetComponent<Image>();
            _image.enabled = false;
        }

        /// <summary>
        /// On Update, we update our alpha 
        /// </summary>
        protected virtual void Update()
        {
            if (_canvasGroup == null) { return; }

            if (_fading)
            {
                Fade();
            }
        }

        /// <summary>
        /// Fades the canvasgroup towards its target alpha
        /// </summary>
        protected virtual void Fade()
        {
            float currentTime = IgnoreTimescale ? Time.unscaledTime : Time.time;
            float endTime = _fadeStartedAt + _currentDuration;
            if (currentTime - _fadeStartedAt < _currentDuration)
            {
                float result = Tween.TweenConstructor(currentTime, _fadeStartedAt, endTime, _initialAlpha, _currentTargetAlpha, _currentCurve);
                _canvasGroup.alpha = result;
            }
            else
            {
                StopFading();
            }
        }

        /// <summary>
        /// Stops the fading.
        /// </summary>
        protected virtual void StopFading()
        {
            _canvasGroup.alpha = _currentTargetAlpha;
            _fading = false;
            if (_canvasGroup.alpha == InactiveAlpha)
            {
                DisableFader();
            }
        }

        /// <summary>
        /// Disables the fader.
        /// </summary>
        protected virtual void DisableFader()
        {
            _image.enabled = false;
            if (ShouldBlockRaycasts)
            {
                _canvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>
        /// Enables the fader.
        /// </summary>
        protected virtual void EnableFader()
        {
            _image.enabled = true;
            if (ShouldBlockRaycasts)
            {
                _canvasGroup.blocksRaycasts = true;
            }
        }

        protected virtual void StartFading(float initialAlpha, float endAlpha, float duration, TweenType curve, int id, bool ignoreTimeScale)
        {
            if (id != ID)
            {
                return;
            }
            IgnoreTimescale = ignoreTimeScale;
            EnableFader();
            _fading = true;
            _initialAlpha = initialAlpha;
            _currentTargetAlpha = endAlpha;
            _fadeStartedAt = IgnoreTimescale ? Time.unscaledTime : Time.time;
            _currentCurve = curve;
            _currentDuration = duration;
        }

        /// <summary>
        /// When catching a fade event, we fade our image in or out
        /// </summary>
        /// <param name="fadeEvent">Fade event.</param>
        public virtual void OnEvent(FadeEvent fadeEvent)
        {
            _currentTargetAlpha = (fadeEvent.TargetAlpha == -1) ? ActiveAlpha : fadeEvent.TargetAlpha;
            StartFading(_canvasGroup.alpha, _currentTargetAlpha, fadeEvent.Duration, fadeEvent.Curve, fadeEvent.ID, fadeEvent.IgnoreTimeScale);
        }

        /// <summary>
        /// When catching an MMFadeInEvent, we fade our image in
        /// </summary>
        /// <param name="fadeEvent">Fade event.</param>
        public virtual void OnEvent(FadeInEvent fadeEvent)
        {
            StartFading(InactiveAlpha, ActiveAlpha, fadeEvent.Duration, fadeEvent.Curve, fadeEvent.ID, fadeEvent.IgnoreTimeScale);
        }

        /// <summary>
        /// When catching an MMFadeOutEvent, we fade our image out
        /// </summary>
        /// <param name="fadeEvent">Fade event.</param>
        public virtual void OnEvent(FadeOutEvent fadeEvent)
        {
            StartFading(ActiveAlpha, InactiveAlpha, fadeEvent.Duration, fadeEvent.Curve, fadeEvent.ID, fadeEvent.IgnoreTimeScale);
        }

        // Start listening to events
        protected virtual void OnEnable()
        {
            this.EventStartListening<FadeEvent>();
            this.EventStartListening<FadeInEvent>();
            this.EventStartListening<FadeOutEvent>();
        }

        // Stop listening to events
        protected virtual void OnDisable()
        {
            this.EventStopListening<FadeEvent>();
            this.EventStopListening<FadeInEvent>();
            this.EventStopListening<FadeOutEvent>();
        }
    }
}
