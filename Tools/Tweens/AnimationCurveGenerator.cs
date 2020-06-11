using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameKit.Tools
{
    /// This class will let you create and save a .curves asset in the specified path
    /// This asset will include curves (anti or not) from the MMTween library, to use anywhere animation curves are required
    public class AnimationCurveGenerator : MonoBehaviour
    {
        [Header("Save settings")]
        /// the path to save the asset at
        public string AnimationCurveFilePath = "Assets/MMTools/MMTween/Editor/";
        /// the name of the asset
        public string AnimationCurveFileName = "MMCurves.curves";

        [Header("Animation Curves")]
        /// the dots resolution (higher is better)
        public int Resolution = 50;
        /// whether to generate anti curves (y goes from 1 to 0) or regular ones (y goes from 0 to 1)
        public bool GenerateAntiCurves = false;

        public bool GenerateAnimationCurvesButton;

        protected Type _scriptableObjectType;
        protected Keyframe _keyframe = new Keyframe();
        protected MethodInfo _addMethodInfo;
        protected object[] _parameters;

        /// <summary>
        /// Generates the asset and saves it at the requested path
        /// </summary>
        public virtual void GenerateAnimationCurvesAsset()
        {
            // we get the method to add to our object
            _scriptableObjectType = Type.GetType("UnityEditor.CurvePresetLibrary, UnityEditor");
            _addMethodInfo = _scriptableObjectType.GetMethod("Add");

            // we create a new instance of our curve asset
            ScriptableObject curveAsset = ScriptableObject.CreateInstance(_scriptableObjectType);

            // for each type of curve, we create an animation curve
            foreach (Tween.TweenCurve curve in Enum.GetValues(typeof(Tween.TweenCurve)))
            {
                CreateAnimationCurve(curveAsset, curve, Resolution, GenerateAntiCurves);
            }

            // we save it to file
#if UNITY_EDITOR
            AssetDatabase.CreateAsset(curveAsset, AnimationCurveFilePath + AnimationCurveFileName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        // Creates an animation curve of the specified type and resolution, and adds it to the specified asset
        protected virtual void CreateAnimationCurve(ScriptableObject asset, Tween.TweenCurve curveType, int curveResolution, bool anti)
        {
            // generates an animation curve
            AnimationCurve animationCurve = new AnimationCurve();

            for (int i = 0; i < curveResolution; i++)
            {
                _keyframe.time = i / (curveResolution - 1f);
                if (anti)
                {
                    _keyframe.value = Tween.TweenConstructor(_keyframe.time, 0f, 1f, 1f, 0f, curveType);
                }
                else
                {
                    _keyframe.value = Tween.TweenConstructor(_keyframe.time, 0f, 1f, 0f, 1f, curveType);
                }
                animationCurve.AddKey(_keyframe);
            }
            // smoothes the curve's tangents
            for (int j = 0; j < curveResolution; j++)
            {
                animationCurve.SmoothTangents(j, 0f);
            }

            // we add the curve to the scriptable object
            _parameters = new object[] { animationCurve, curveType.ToString() };
            _addMethodInfo.Invoke(asset, _parameters);

        }
    }
}

