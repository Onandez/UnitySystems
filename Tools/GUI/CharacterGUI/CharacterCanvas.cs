using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.TopDownEngine;

namespace GameKit.Tools
{
    ///Bonus and penaltys Dont follow must be in canvas!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public class CharacterCanvas : MonoBehaviour
    {
        public enum TimeScales
        {
            UnscaledTime, Time
        }

        public TimeScales TimeScale = TimeScales.UnscaledTime;

        public bool BumpScaleOnChange = true;


        //[Header("Death")]
        //public GameObject InstantiatedOnDeath;          /// Gameobject (usually a particle system) to instantiate when the healthbar reaches zero


        [Header("Display")]
        public bool AlwaysVisible = false;
        public float DisplayDurationOnHit = 1f;
        public bool HideBarAtZero = true;
        public float HideBarAtZeroDelay = 1f;

        //Storage
        //protected FollowTarget _followTransform;
        public ProgressBar _healthProgressBar;  //in follow transform. combine with FollwoTarget
        public ProgressBar _staminaProgressBar; //in follow transform. combine with FollwoTarget

        public List<GameObject> _characterstateBonus;
        public List<GameObject> _characterstateMalus;

        public enum UpdateModes
        {
            Update, FixedUpdate, LateUpdate
        }
        //FollowTarget
        [Header("Target")]
        public UpdateModes UpdateMode = UpdateModes.LateUpdate;
        public Transform targetPostion;     //Target to follow
        public Transform targetRotation;
        public Vector3 Offset;
        public bool FollowPosition = true;
        public bool FollowRotation = true; // if false apply camera torataion

        [Header("Interpolation")]
        public bool InterpolatePosition = true;
        public bool InterpolateRotation = true;
        public float FollowPositionSpeed = 10f;
        public float FollowRotationSpeed = 10f;

        [Header("Axis")]
        public bool FollowPositionX = true;
        public bool FollowPositionY = true;
        public bool FollowPositionZ = true;

        [Header("Distances")]
        /// whether or not to force a minimum distance between the object and its target before it starts following
        public bool UseMinimumDistanceBeforeFollow = false;
        /// the minimum distance to keep between the object and its target
        public float MinimumDistanceBeforeFollow = 1f;
        /// whether or not we want to make sure the object is never too far away from its target
        public bool UseMaximumDistance = false;
        /// the maximum distance at which the object can be away from its target
        public float MaximumDistance = 1f;

        //Storage
        protected Character m_Character;
        protected float _lastShowTimestamp = 0f;
        protected bool _showBar = false;
        protected Image _backgroundImage = null;
        protected Image _borderImage = null;
        protected Image _foregroundImage = null;
        protected Image _delayedImage = null;
        protected bool _finalHideStarted = false;
        //Storage
        protected Vector3 _newTargetPosition;
        protected Vector3 _initialPosition;
        protected Vector3 _lastTargetPosition;
        protected Vector3 _direction;
        protected Quaternion _newTargetRotation;
        protected Quaternion _initialRotation;

        protected virtual void Start()
        {
            Initialization();
        }

        // sets the health bar up
        public virtual void Initialization()
        {
            if (!AlwaysVisible)
            {
                gameObject.SetActive(false);
                //_healthProgressBar.gameObject.SetActive(false);
                //_staminaProgressBar.gameObject.SetActive(false);
            }

            SetInitialPosition();
            SetOffset(false, false, false);

            //GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            //GetComponent<Canvas>().
        }

        // Prevents the object from following the target anymore
        public virtual void StopFollowing()
        {
            FollowPosition = false;
        }

        // Makes the object follow the target
        public virtual void StartFollowing()
        {
            FollowPosition = true;
            SetInitialPosition();
        }

        // Stores the initial position
        protected virtual void SetInitialPosition()
        {
            _initialPosition = this.transform.position;
            _initialRotation = this.transform.rotation;
            _lastTargetPosition = this.transform.position;
        }

        public virtual void SetupCharacter(Character character)
        {
            m_Character = character;
        }

        // Adds initial offset to the offset if needed
        protected virtual void SetOffset(bool AddInitialDistanceXToXOffset, bool AddInitialDistanceYToYOffset, bool AddInitialDistanceZToZOffset)
        {
            if (targetPostion == null) return;

            Vector3 difference = this.transform.position - targetPostion.transform.position;
            Offset.x = AddInitialDistanceXToXOffset ? difference.x : Offset.x;
            Offset.y = AddInitialDistanceYToYOffset ? difference.y : Offset.y;
            Offset.z = AddInitialDistanceZToZOffset ? difference.z : Offset.z;
        }

        // Hide or show our healthbar based on our current status
        protected virtual void Update()
        {
            if (_healthProgressBar == null) return;

            if (_finalHideStarted) return;

            //UpdateDrawnColors();

            if (AlwaysVisible) return;

            if (_showBar)
            {
                _healthProgressBar.gameObject.SetActive(true);
                _staminaProgressBar.gameObject.SetActive(true);
                float currentTime = (TimeScale == TimeScales.UnscaledTime) ? Time.unscaledTime : Time.time;
                if (currentTime - _lastShowTimestamp > DisplayDurationOnHit)
                {
                    _showBar = false;
                }
            }
            else
            {
                _healthProgressBar.gameObject.SetActive(false);
                _staminaProgressBar.gameObject.SetActive(false);
            }

            //if (targetPostion == null) return;

            if (UpdateMode == UpdateModes.Update)
            {
                FollowTargetRotation();
                FollowTargetPosition();
            }
        }

        // Follow target
        protected virtual void FixedUpdate()
        {
            if (targetPostion == null) return;

            if (UpdateMode == UpdateModes.FixedUpdate)
            {
                FollowTargetRotation();
                FollowTargetPosition();
            }
        }

        // Follow target
        protected virtual void LateUpdate()
        {
            if (targetPostion == null) return;

            if (UpdateMode == UpdateModes.LateUpdate)
            {
                FollowTargetRotation();
                FollowTargetPosition();
            }
        }


        // Follows the target, lerping the position or not based on what's been defined in the inspector
        protected virtual void FollowTargetPosition()
        {
            if (targetPostion == null) return;

            if (!FollowPosition) return;

            _newTargetPosition = targetPostion.position + Offset;
            if (!FollowPositionX) { _newTargetPosition.x = _initialPosition.x; }
            if (!FollowPositionY) { _newTargetPosition.y = _initialPosition.y; }
            if (!FollowPositionZ) { _newTargetPosition.z = _initialPosition.z; }

            float trueDistance = Vector3.Distance(this.transform.position, _newTargetPosition);
            _direction = (_newTargetPosition - this.transform.position).normalized;

            float interpolatedDistance = trueDistance;
            if (InterpolatePosition)
            {
                interpolatedDistance = Mathf.Lerp(0f, trueDistance, Time.deltaTime * FollowPositionSpeed);
            }

            if (UseMinimumDistanceBeforeFollow)
            {
                if (trueDistance - interpolatedDistance < MinimumDistanceBeforeFollow)
                {
                    interpolatedDistance = 0f;
                }
            }

            if (UseMaximumDistance)
            {
                if (trueDistance - interpolatedDistance >= MaximumDistance)
                {
                    interpolatedDistance = trueDistance - MaximumDistance;
                }
            }
            this.transform.Translate(_direction * interpolatedDistance, Space.World);
        }

        // Object follow target's rotation
        protected virtual void FollowTargetRotation()
        {
            if (targetRotation == null) return;

            if (!FollowRotation) return;

            //transform.LookAt(targetRotation);
            //return;

            _newTargetRotation = targetRotation.rotation;

            if (InterpolateRotation)
            {
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, _newTargetRotation, Time.deltaTime * FollowRotationSpeed);
            }
            else
            {
                this.transform.rotation = _newTargetRotation;
            }
        }

        // Updates the bar
        public virtual void UpdateHealthBar(float currentHealth, float minHealth, float maxHealth, bool show)
        {
            if (_healthProgressBar != null)
            {
                _healthProgressBar.UpdateBar(currentHealth, minHealth, maxHealth);

                if (HideBarAtZero && _healthProgressBar.BarProgress <= 0)
                {
                    StartCoroutine(FinalHideBar());
                }

                if (BumpScaleOnChange)
                {
                    _healthProgressBar.Bump();
                }
            }

            // if the healthbar isn't supposed to be always displayed, we turn it on for the specified duration
            if (!AlwaysVisible && show)
            {
                _showBar = true;
                _lastShowTimestamp = (TimeScale == TimeScales.UnscaledTime) ? Time.unscaledTime : Time.time;
            }
        }

        // Updates the bar
        public virtual void UpdateStaminaBar(float currentStamina, float minStamina, float maxStamina, bool show)
        {
            if (_staminaProgressBar != null)
            {
                _staminaProgressBar.UpdateBar(currentStamina, minStamina, maxStamina);

                if (HideBarAtZero && _healthProgressBar.BarProgress <= 0)
                {
                    //StartCoroutine(FinalHideBar());
                }

                if (BumpScaleOnChange)
                {
                    _staminaProgressBar.Bump();
                }
            }

            // if the healthbar isn't supposed to be always displayed, we turn it on for the specified duration
            if (!AlwaysVisible && show)
            {
                _showBar = true;
                _lastShowTimestamp = (TimeScale == TimeScales.UnscaledTime) ? Time.unscaledTime : Time.time;
            }
        }

        // Hides the bar when it reaches zero
        protected virtual IEnumerator FinalHideBar()
        {
            _finalHideStarted = true;
            if (HideBarAtZeroDelay == 0)
            {
                _showBar = false;
                _healthProgressBar.gameObject.SetActive(false);
                _staminaProgressBar.gameObject.SetActive(false);
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(HideBarAtZeroDelay);
                _showBar = false;
                _healthProgressBar.gameObject.SetActive(false);
                _staminaProgressBar.gameObject.SetActive(false);
            }
        }
    }
}
