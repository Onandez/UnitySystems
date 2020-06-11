using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace GameKit.Tools
{
    public class HealthBar : MonoBehaviour
    {
        /// the possible health bar types
		public enum HealthBarTypes
        {
            Prefab, Drawn
        }
        /// the possible timescales the bar can work on
        public enum TimeScales
        {
            UnscaledTime, Time
        }

        /// whether the healthbar uses a prefab or is drawn automatically
        public HealthBarTypes HealthBarType = HealthBarTypes.Prefab;
        /// defines whether the bar will work on scaled or unscaled time (whether or not it'll keep moving if time is slowed down for example)
        public TimeScales TimeScale = TimeScales.UnscaledTime;

        [Header("Select a Prefab")]
        /// the prefab to use as the health bar
        public ProgressBar HealthBarPrefab;

        [Header("Drawn Healthbar Settings ")]
        /// if the healthbar is drawn, its size in world units
        public Vector2 Size = new Vector2(1.5f, 0.3f);
        /// if the healthbar is drawn, the padding to apply to the foreground, in world units
        public Vector2 BackgroundPadding = new Vector2(0.03f, 0.03f);
        public Gradient BorderColor;
        public Gradient ForegroundColor;
        public Gradient DelayedColor;
        public Gradient BackgroundColor;
        public string SortingLayerName = "UI";
        /// the delay to apply to the delayed bar if drawn
        public float drawDelay = 0.5f;
        /// whether or not the front bar should lerp
        public bool LerpFrontBar = true;
        /// the speed at which the front bar lerps
        public float LerpFrontBarSpeed = 15f;
        /// whether or not the delayed bar should lerp
        public bool LerpDelayedBar = true;
        /// the speed at which the delayed bar lerps
        public float LerpDelayedBarSpeed = 15f;
        /// if this is true, bumps the scale of the healthbar when its value changes
        public bool BumpScaleOnChange = true;
        /// the duration of the bump animation
        public float BumpDuration = 0.2f;
        /// the animation curve to map the bump animation on
        public AnimationCurve BumpAnimationCurve = AnimationCurve.Constant(0, 1, 1);
        /// the mode the bar should follow the target in
        public FollowTarget.UpdateModes FollowTargetMode = FollowTarget.UpdateModes.LateUpdate;

        [Header("Death")]
        /// a gameobject (usually a particle system) to instantiate when the healthbar reaches zero
        public GameObject InstantiatedOnDeath;

        [Header("Offset")]
        /// the offset to apply to the healthbar compared to the object's center
        public Vector3 HealthBarOffset = new Vector3(0f, 2.5f, 0f);

        [Header("Display")]
        /// whether or not the bar should be permanently displayed
        public bool AlwaysVisible = true;
        /// the duration (in seconds) during which to display the bar
        public float DisplayDurationOnHit = 1f;
        /// if this is set to true the bar will hide itself when it reaches zero
        public bool HideBarAtZero = true;
        /// the delay (in seconds) after which to hide the bar
        public float HideBarAtZeroDelay = 1f;

        protected ProgressBar _progressBar;
        protected FollowTarget _followTransform;
        protected float _lastShowTimestamp = 0f;
        protected bool _showBar = false;
        protected Image _backgroundImage = null;
        protected Image _borderImage = null;
        protected Image _foregroundImage = null;
        protected Image _delayedImage = null;
        protected bool _finalHideStarted = false;

        
        protected virtual void Start()
        {
            Initialization();
        }

        // Creates or sets the health bar up
        public virtual void Initialization()
        {
            if (_progressBar != null)
            {
                _progressBar.gameObject.SetActive(true);
                return;
            }

            if (HealthBarType == HealthBarTypes.Prefab)
            {
                if (HealthBarPrefab == null)
                {
                    Debug.LogWarning(this.name + " : the HealthBar has no prefab associated to it, nothing will be displayed.");
                    return;
                }
                _progressBar = Instantiate(HealthBarPrefab, transform.position + HealthBarOffset, transform.rotation) as ProgressBar;
                _progressBar.transform.SetParent(this.transform);
                _progressBar.gameObject.name = "HealthBar";
            }

            if (HealthBarType == HealthBarTypes.Drawn)
            {
                DrawHealthBar();
                UpdateDrawnColors();
            }

            if (!AlwaysVisible)
            {
                _progressBar.gameObject.SetActive(false);
            }

            if (_progressBar != null)
            {
                _progressBar.UpdateBar(100f, 0f, 100f);
            }
        }

        // Draws the health bar.
        protected virtual void DrawHealthBar()
        {
            GameObject newGameObject = new GameObject();
            newGameObject.name = "HealthBar|" + this.gameObject.name;

            _progressBar = newGameObject.AddComponent<ProgressBar>();

            _followTransform = newGameObject.AddComponent<FollowTarget>();
            _followTransform.Offset = HealthBarOffset;
            _followTransform.TargetPostion = this.transform;
            _followTransform.InterpolatePosition = false;
            _followTransform.InterpolateRotation = false;
            _followTransform.UpdateMode = FollowTargetMode;

            Canvas newCanvas = newGameObject.AddComponent<Canvas>();
            newCanvas.renderMode = RenderMode.WorldSpace;
            newCanvas.transform.localScale = Vector3.one;
            newCanvas.GetComponent<RectTransform>().sizeDelta = Size;
            if (!string.IsNullOrEmpty(SortingLayerName))
            {
                newCanvas.sortingLayerName = SortingLayerName;
            }

            GameObject borderImageGameObject = new GameObject();
            borderImageGameObject.transform.SetParent(newGameObject.transform);
            borderImageGameObject.name = "HealthBar Border";
            _borderImage = borderImageGameObject.AddComponent<Image>();
            _borderImage.transform.position = Vector3.zero;
            _borderImage.transform.localScale = Vector3.one;
            _borderImage.GetComponent<RectTransform>().sizeDelta = Size;
            _borderImage.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            GameObject bgImageGameObject = new GameObject();
            bgImageGameObject.transform.SetParent(newGameObject.transform);
            bgImageGameObject.name = "HealthBar Background";
            _backgroundImage = bgImageGameObject.AddComponent<Image>();
            _backgroundImage.transform.position = Vector3.zero;
            _backgroundImage.transform.localScale = Vector3.one;
            _backgroundImage.GetComponent<RectTransform>().sizeDelta = Size - BackgroundPadding * 2;
            _backgroundImage.GetComponent<RectTransform>().anchoredPosition = -_backgroundImage.GetComponent<RectTransform>().sizeDelta / 2;
            _backgroundImage.GetComponent<RectTransform>().pivot = Vector2.zero;

            GameObject delayedImageGameObject = new GameObject();
            delayedImageGameObject.transform.SetParent(newGameObject.transform);
            delayedImageGameObject.name = "HealthBar Delayed Foreground";
            _delayedImage = delayedImageGameObject.AddComponent<Image>();
            _delayedImage.transform.position = Vector3.zero;
            _delayedImage.transform.localScale = Vector3.one;
            _delayedImage.GetComponent<RectTransform>().sizeDelta = Size - BackgroundPadding * 2;
            _delayedImage.GetComponent<RectTransform>().anchoredPosition = -_delayedImage.GetComponent<RectTransform>().sizeDelta / 2;
            _delayedImage.GetComponent<RectTransform>().pivot = Vector2.zero;

            GameObject frontImageGameObject = new GameObject();
            frontImageGameObject.transform.SetParent(newGameObject.transform);
            frontImageGameObject.name = "HealthBar Foreground";
            _foregroundImage = frontImageGameObject.AddComponent<Image>();
            _foregroundImage.transform.position = Vector3.zero;
            _foregroundImage.transform.localScale = Vector3.one;
            _foregroundImage.GetComponent<RectTransform>().sizeDelta = Size - BackgroundPadding * 2;
            _foregroundImage.GetComponent<RectTransform>().anchoredPosition = -_foregroundImage.GetComponent<RectTransform>().sizeDelta / 2;
            _foregroundImage.GetComponent<RectTransform>().pivot = Vector2.zero;

            _progressBar.LerpDelayedBar = LerpDelayedBar;
            _progressBar.LerpForegroundBar = LerpFrontBar;
            _progressBar.LerpDelayedBarSpeed = LerpDelayedBarSpeed;
            _progressBar.LerpForegroundBarSpeed = LerpFrontBarSpeed;
            _progressBar.ForegroundBar = _foregroundImage.transform;
            _progressBar.DelayedBar = _delayedImage.transform;
            _progressBar.Delay = drawDelay;
            _progressBar.BumpScaleOnChange = BumpScaleOnChange;
            _progressBar.BumpDuration = BumpDuration;
            _progressBar.BumpAnimationCurve = BumpAnimationCurve;
            _progressBar.TimeScale = (TimeScale == TimeScales.Time) ? ProgressBar.TimeScales.Time : ProgressBar.TimeScales.UnscaledTime;
        }

        // Hide or show our healthbar based on our current status
        protected virtual void Update()
        {
            if (_progressBar == null) return;

            if (_finalHideStarted) return;

            UpdateDrawnColors();

            if (AlwaysVisible) return;

            if (_showBar)
            {
                _progressBar.gameObject.SetActive(true);
                float currentTime = (TimeScale == TimeScales.UnscaledTime) ? Time.unscaledTime : Time.time;
                if (currentTime - _lastShowTimestamp > DisplayDurationOnHit)
                {
                    _showBar = false;
                }
            }
            else
            {
                _progressBar.gameObject.SetActive(false);
            }
        }

        // Hides the bar when it reaches zero
        protected virtual IEnumerator FinalHideBar()
        {
            _finalHideStarted = true;
            if (InstantiatedOnDeath != null)
            {
                Instantiate(InstantiatedOnDeath, this.transform.position + HealthBarOffset, this.transform.rotation);
            }
            if (HideBarAtZeroDelay == 0)
            {
                _showBar = false;
                _progressBar.gameObject.SetActive(false);
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(HideBarAtZeroDelay);
                _showBar = false;
                _progressBar.gameObject.SetActive(false);
            }
        }

        // Updates the colors of the bars
        protected virtual void UpdateDrawnColors()
        {
            if (HealthBarType != HealthBarTypes.Drawn)
            {
                return;
            }

            if (_progressBar.Bumping)
            {
                return;
            }

            if (_borderImage != null)
            {
                _borderImage.color = BorderColor.Evaluate(_progressBar.BarProgress);
            }

            if (_backgroundImage != null)
            {
                _backgroundImage.color = BackgroundColor.Evaluate(_progressBar.BarProgress);
            }

            if (_delayedImage != null)
            {
                _delayedImage.color = DelayedColor.Evaluate(_progressBar.BarProgress);
            }

            if (_foregroundImage != null)
            {
                _foregroundImage.color = ForegroundColor.Evaluate(_progressBar.BarProgress);
            }
        }

        // Updates the bar
        public virtual void UpdateBar(float currentHealth, float minHealth, float maxHealth, bool show)
        {
            if (_progressBar != null)
            {
                _progressBar.UpdateBar(currentHealth, minHealth, maxHealth);

                if (HideBarAtZero && _progressBar.BarProgress <= 0)
                {
                    StartCoroutine(FinalHideBar());
                }

                if (BumpScaleOnChange)
                {
                    _progressBar.Bump();
                }
            }

            // if the healthbar isn't supposed to be always displayed, we turn it on for the specified duration
            if (!AlwaysVisible && show)
            {
                _showBar = true;
                _lastShowTimestamp = (TimeScale == TimeScales.UnscaledTime) ? Time.unscaledTime : Time.time;
            }
        }
    }
}
