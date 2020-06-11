using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Tools
{
    public class FollowTarget : MonoBehaviour
    {
        public enum UpdateModes
        {
            Update, FixedUpdate, LateUpdate
        }
        public UpdateModes UpdateMode = UpdateModes.Update;

        [Header("Activity")]
        public bool FollowPosition = true;
        public bool FollowRotation = true;      // if false apply camera torataion

        [Header("Target")]
        public Transform TargetPostion;     //Target to follow
        public Transform targetRotation;
        public Vector3 Offset;
        ///whether or not to add the initial x distance to the offset
        public bool AddInitialDistanceXToXOffset = false;
        ///whether or not to add the initial y distance to the offset
        public bool AddInitialDistanceYToYOffset = false;
        ///whether or not to add the initial z distance to the offset
        public bool AddInitialDistanceZToZOffset = false;

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

        // Initializes the follow
        public virtual void Initialization()
        {
            SetInitialPosition();
            SetOffset();
        }

        // Prevents follow the target
        public virtual void StopFollowing()
        {
            FollowPosition = false;
        }

        // Makes follow the target
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

        // Adds initial offset to the offset if needed
        protected virtual void SetOffset()
        {
            if (TargetPostion == null) return;

            Vector3 difference = this.transform.position - TargetPostion.transform.position;
            Offset.x = AddInitialDistanceXToXOffset ? difference.x : Offset.x;
            Offset.y = AddInitialDistanceYToYOffset ? difference.y : Offset.y;
            Offset.z = AddInitialDistanceZToZOffset ? difference.z : Offset.z;
        }

        #region Update

        protected virtual void Update()
        {
            if (TargetPostion == null) return;

            if (UpdateMode == UpdateModes.Update)
            {
                FollowTargetRotation();
                FollowTargetPosition();
            }
        }

        // Follow target
        protected virtual void FixedUpdate()
        {
            if (UpdateMode == UpdateModes.FixedUpdate)
            {
                FollowTargetRotation();
                FollowTargetPosition();
            }
        }

        // Follow target
        protected virtual void LateUpdate()
        {
            if (UpdateMode == UpdateModes.LateUpdate)
            {
                FollowTargetRotation();
                FollowTargetPosition();
            }
        }

        // Follows target position
        protected virtual void FollowTargetPosition()
        {
            if (TargetPostion == null) return;

            if (!FollowPosition) return;

            _newTargetPosition = TargetPostion.position + Offset;
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

        // Follow target rotation
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
        #endregion
    }
}

