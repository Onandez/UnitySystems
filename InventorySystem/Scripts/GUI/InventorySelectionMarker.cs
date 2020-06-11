using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using UnityEngine.EventSystems;

namespace InventorySystem
{
    [RequireComponent(typeof(RectTransform))]
    // Handles the selection marker, that will mark the currently selected slot
    public class InventorySelectionMarker : MonoBehaviour
    {
        public float TransitionSpeed = 5f;                  // the speed at which the selection marker will move from one slot to the other
        public float MinimalTransitionDistance = 0.01f;     // the threshold distance at which the marker will stop moving

        protected RectTransform _rectTransform;
        protected GameObject _currentSelection;
        protected Vector3 _originPosition;
        protected Vector3 _originLocalScale;
        protected Vector3 _originSizeDelta;
        protected float _originTime;
        protected bool _originIsNull = true;
        protected float _deltaTime;

        // Get the associated rect transform
        void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        // Get the current selected object, and we move the marker to it if necessary
        void Update()
        {
            _currentSelection = EventSystem.current.currentSelectedGameObject;
            if (_currentSelection == null)
            {
                return;
            }
            if (Vector3.Distance(transform.position, _currentSelection.transform.position) > MinimalTransitionDistance)
            {
                if (_originIsNull)
                {
                    _originIsNull = false;
                    _originPosition = transform.position;
                    _originLocalScale = _rectTransform.localScale;
                    _originSizeDelta = _rectTransform.sizeDelta;
                    _originTime = Time.unscaledTime;
                }
                _deltaTime = (Time.unscaledTime - _originTime) * TransitionSpeed;
                transform.position = Vector3.Lerp(_originPosition, _currentSelection.transform.position, _deltaTime);
                _rectTransform.localScale = Vector3.Lerp(_originLocalScale, _currentSelection.GetComponent<RectTransform>().localScale, _deltaTime);
                _rectTransform.sizeDelta = Vector3.Lerp(_originSizeDelta, _currentSelection.GetComponent<RectTransform>().sizeDelta, _deltaTime);
            }
            else
            {
                _originIsNull = true;
            }
        }
    }
}
