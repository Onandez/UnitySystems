using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameKit.Tools
{
    public class ProgressBar : MonoBehaviour
    {
        public enum FillModes
        {
            LocalScale, FillAmount,
            Width, Height
        }
        public enum BarDirections
        {
            LeftToRight, RightToLeft, UpToDown, DownToUp
        }
        public enum TimeScales
        {
            UnscaledTime, Time
        }

        [Header("Settings")]
        public float startValue = 0f;       //Value when bar is empty
        public float endValue = 1f;         //Value when bar is full
        public BarDirections barDirection = BarDirections.LeftToRight;
        public FillModes FillMode = FillModes.LocalScale;
        public TimeScales TimeScale = TimeScales.UnscaledTime;

        [Header("Foreground Bar Settings")]
        public bool LerpForegroundBar = true;           /// whether or not the foreground bar should lerp                                             
        public float LerpForegroundBarSpeed = 15f;      /// the speed at which to lerp the foreground bar

        [Header("Delayed Bar Settings")]
        public float Delay = 1f;                    /// the delay before the delayed bar moves (in seconds)
        public bool LerpDelayedBar = true;          /// whether or not the delayed bar's animation should lerp
        public float LerpDelayedBarSpeed = 30f;     /// the speed at which to lerp the delayed bar

        [Header("Bindings")]
        
        public string PlayerID;         /// optional - the ID of the player associated to this bar               
        public Transform DelayedBar;                              
        public Transform ForegroundBar;

        [Header("Bump")]
        /// whether or not the bar should "bump" when changing value
        public bool BumpScaleOnChange = true;
        /// whether or not the bar should bump when its value increases
        public bool BumpOnIncrease = false;
        /// the duration of the bump animation
        public float BumpDuration = 0.2f;
        /// whether or not the bar should flash when bumping
        public bool ChangeColorWhenBumping = true;
        /// the color to apply to the bar when bumping
        public Color BumpColor = Color.white;
        /// the curve to map the bump animation on
        public AnimationCurve BumpAnimationCurve = new AnimationCurve(new Keyframe(1, 1), new Keyframe(0.3f, 1.05f), new Keyframe(1, 1));
        /// the curve to map the bump animation color animation on
        public AnimationCurve BumpColorAnimationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0));
        /// whether or not the bar is bumping right now
        public bool Bumping { get; protected set; }

        [Header("Realtime")]
        public bool AutoUpdating = false;   //Should update itself every update
        [Range(0f, 1f)]
        public float BarProgress = 0.3f;   //Current progress bar
        public bool TestBumpButton;

        //Storage
        protected float _targetFill;
        protected Vector3 _targetLocalScale = Vector3.one;
        protected float _newPercent;
        protected float _lastPercent;
        protected float _lastUpdateTimestamp;
        protected bool _bump = false;
        protected Color _initialColor;
        protected Vector3 _initialScale;
        protected Vector3 _newScale;
        protected Image _foregroundImage;
        protected Image _delayedImage;
        protected bool _initialized;
        protected Vector2 _initialFrontBarSize;

        // Store image component
        protected virtual void Start()
        {
            _initialScale = this.transform.localScale;

            if (ForegroundBar != null)
            {
                _foregroundImage = ForegroundBar.GetComponent<Image>();
                _initialFrontBarSize = _foregroundImage.rectTransform.sizeDelta;
            }
            if (DelayedBar != null)
            {
                _delayedImage = DelayedBar.GetComponent<Image>();
            }
            _initialized = true;
        }

        // Update bars
        protected virtual void Update()
        {
            AutoUpdate();
            UpdateFrontBar();
            UpdateDelayedBar();
        }

        protected virtual void AutoUpdate()
        {
            if (!AutoUpdating) return;
          
            _newPercent = MathsHelper.Remap(BarProgress, 0f, 1f, startValue, endValue);
            _targetFill = _newPercent;
            _lastUpdateTimestamp = (TimeScale == TimeScales.Time) ? Time.time : Time.unscaledTime;
        }

        // Updates the front bar's scale
        protected virtual void UpdateFrontBar()
        {
            float currentDeltaTime = (TimeScale == TimeScales.Time) ? Time.deltaTime : Time.unscaledTime;

            if (ForegroundBar != null)
            {
                switch (FillMode)
                {
                    case FillModes.LocalScale:
                        _targetLocalScale = Vector3.one;
                        switch (barDirection)
                        {
                            case BarDirections.LeftToRight:
                                _targetLocalScale.x = _targetFill;
                                break;
                            case BarDirections.RightToLeft:
                                _targetLocalScale.x = 1f - _targetFill;
                                break;
                            case BarDirections.DownToUp:
                                _targetLocalScale.y = _targetFill;
                                break;
                            case BarDirections.UpToDown:
                                _targetLocalScale.y = 1f - _targetFill;
                                break;
                        }

                        if (LerpForegroundBar)
                        {
                            _newScale = Vector3.Lerp(ForegroundBar.localScale, _targetLocalScale, currentDeltaTime * LerpForegroundBarSpeed);
                        }
                        else
                        {
                            _newScale = _targetLocalScale;
                        }

                        ForegroundBar.localScale = _newScale;
                        break;

                    case FillModes.Width:
                        if (_foregroundImage == null)
                        {
                            return;
                        }
                        float newSizeX = MathsHelper.Remap(_targetFill, 0f, 1f, 0, _initialFrontBarSize.x);
                        newSizeX = Mathf.Lerp(_foregroundImage.rectTransform.sizeDelta.x, newSizeX, currentDeltaTime * LerpForegroundBarSpeed);
                        _foregroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSizeX);
                        break;

                    case FillModes.Height:
                        if (_foregroundImage == null)
                        {
                            return;
                        }
                        float newSizeY = MathsHelper.Remap(_targetFill, 0f, 1f, 0, _initialFrontBarSize.y);
                        newSizeY = Mathf.Lerp(_foregroundImage.rectTransform.sizeDelta.x, newSizeY, currentDeltaTime * LerpForegroundBarSpeed);
                        _foregroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSizeY);
                        break;

                    case FillModes.FillAmount:
                        if (_foregroundImage == null)
                        {
                            return;
                        }
                        if (LerpForegroundBar)
                        {
                            _foregroundImage.fillAmount = Mathf.Lerp(_foregroundImage.fillAmount, _targetFill, currentDeltaTime * LerpForegroundBarSpeed);
                        }
                        else
                        {
                            _foregroundImage.fillAmount = _targetFill;
                        }
                        break;
                }
            }
        }

        // Updates the delayed bar's scale
        protected virtual void UpdateDelayedBar()
        {
            float currentDeltaTime = (TimeScale == TimeScales.Time) ? Time.deltaTime : Time.unscaledDeltaTime;
            float currentTime = (TimeScale == TimeScales.Time) ? Time.time : Time.unscaledTime;

            if (DelayedBar != null)
            {
                if (currentTime - _lastUpdateTimestamp > Delay)
                {
                    if (FillMode == FillModes.LocalScale)
                    {
                        _targetLocalScale = Vector3.one;

                        switch (barDirection)
                        {
                            case BarDirections.LeftToRight:
                                _targetLocalScale.x = _targetFill;
                                break;
                            case BarDirections.RightToLeft:
                                _targetLocalScale.x = 1f - _targetFill;
                                break;
                            case BarDirections.DownToUp:
                                _targetLocalScale.y = _targetFill;
                                break;
                            case BarDirections.UpToDown:
                                _targetLocalScale.y = 1f - _targetFill;
                                break;
                        }

                        if (LerpDelayedBar)
                        {
                            _newScale = Vector3.Lerp(DelayedBar.localScale, _targetLocalScale, currentDeltaTime * LerpDelayedBarSpeed);
                        }
                        else
                        {
                            _newScale = _targetLocalScale;
                        }
                        DelayedBar.localScale = _newScale;
                    }

                    if ((FillMode == FillModes.FillAmount) && (_delayedImage != null))
                    {
                        if (LerpDelayedBar)
                        {
                            _delayedImage.fillAmount = Mathf.Lerp(_delayedImage.fillAmount, _targetFill, currentDeltaTime * LerpDelayedBarSpeed);
                        }
                        else
                        {
                            _delayedImage.fillAmount = _targetFill;
                        }
                    }
                }
            }
        }

        // Updates the bar's values 
        public virtual void UpdateBar(float currentValue, float minValue, float maxValue)
        {
            _newPercent = MathsHelper.Remap(currentValue, minValue, maxValue, startValue, endValue);
            if ((_newPercent != BarProgress) && !Bumping)
            {
                Bump();
            }
            BarProgress = _newPercent;
            _targetFill = _newPercent;
            _lastUpdateTimestamp = (TimeScale == TimeScales.Time) ? Time.time : Time.unscaledTime;
            _lastPercent = _newPercent;
        }

        // Triggers a camera bump
        public virtual void Bump()
        {
            if (!BumpScaleOnChange || !_initialized)
            {
                return;
            }
            if (!BumpOnIncrease && (_lastPercent < _newPercent))
            {
                return;
            }
            if (this.gameObject.activeInHierarchy)
            {
                StartCoroutine(BumpCoroutine());
            }
        }

        // Coroutine changes the scale of the bar 
        protected virtual IEnumerator BumpCoroutine()
        {
            float journey = 0f;
            float currentDeltaTime = (TimeScale == TimeScales.Time) ? Time.deltaTime : Time.unscaledDeltaTime;

            Bumping = true;
            if (_foregroundImage != null)
            {
                _initialColor = _foregroundImage.color;
            }

            while (journey <= BumpDuration)
            {
                journey = journey + currentDeltaTime;
                float percent = Mathf.Clamp01(journey / BumpDuration);
                float curvePercent = BumpAnimationCurve.Evaluate(percent);
                float colorCurvePercent = BumpColorAnimationCurve.Evaluate(percent);
                this.transform.localScale = curvePercent * _initialScale;

                if (ChangeColorWhenBumping && (_foregroundImage != null))
                {
                    _foregroundImage.color = Color.Lerp(_initialColor, BumpColor, colorCurvePercent);
                }

                yield return null;
            }
            _foregroundImage.color = _initialColor;
            Bumping = false;
            yield return null;

        }
    }
}
