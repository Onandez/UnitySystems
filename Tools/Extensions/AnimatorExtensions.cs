using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameKit.Tools
{
    // Animator extensions
    public static class AnimatorExtensions
    {
		// Determines if an animator contains a certain parameter, based on a type and a name
		public static bool HasParameterOfType(this Animator self, string name, AnimatorControllerParameterType type)
        {
            if (string.IsNullOrEmpty(name)) { return false; }
            AnimatorControllerParameter[] parameters = self.parameters;
            foreach (AnimatorControllerParameter currParam in parameters)
            {
                if (currParam.type == type && currParam.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        // Adds an animator parameter name to a parameter list if that parameter exists.
        public static void AddAnimatorParameterIfExists(Animator animator, string parameterName, out int parameter, AnimatorControllerParameterType type, List<int> parameterList)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                parameter = -1;
                return;
            }

            parameter = Animator.StringToHash(parameterName);

            if (animator.HasParameterOfType(parameterName, type))
            {
                parameterList.Add(parameter);
            }
        }

        // Adds an animator parameter name to a parameter list if that parameter exists.
        public static void AddAnimatorParameterIfExists(Animator animator, string parameterName, AnimatorControllerParameterType type, List<string> parameterList)
        {
            if (animator.HasParameterOfType(parameterName, type))
            {
                parameterList.Add(parameterName);
            }
        }

        // Updates the animator bool.
        public static void UpdateAnimatorBool(Animator animator, string parameterName, bool value)
        {
            animator.SetBool(parameterName, value);
        }

        // Updates the animator bool.
        public static void UpdateAnimatorBool(Animator animator, int parameter, bool value, List<int> parameterList)
        {
            if (parameterList.Contains(parameter))
            {
                animator.SetBool(parameter, value);
            }
        }

        // Updates the animator bool.
        public static void UpdateAnimatorBool(Animator animator, string parameterName, bool value, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetBool(parameterName, value);
            }
        }

        // Sets an animator's trigger of the int parameter specified
        public static void UpdateAnimatorTrigger(Animator animator, int parameter, List<int> parameterList)
        {
            if (parameterList.Contains(parameter))
            {
                animator.SetTrigger(parameter);
            }
        }

        // Sets an animator's trigger of the string parameter name specified
        public static void UpdateAnimatorTrigger(Animator animator, string parameterName, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetTrigger(parameterName);
            }
        }

        // Triggers an animator trigger.
        public static void SetAnimatorTrigger(Animator animator, int parameter, List<int> parameterList)
        {
            if (parameterList.Contains(parameter))
            {
                animator.SetTrigger(parameter);
            }
        }

		// Triggers an animator trigger.
		public static void SetAnimatorTrigger(Animator animator, string parameterName, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetTrigger(parameterName);
            }
        }

        // Updates the animator's float 
        public static void UpdateAnimatorFloat(Animator animator, string parameterName, float value)
        {
            animator.SetFloat(parameterName, value);
        }

        // Updates the animator float.
        public static void UpdateAnimatorFloat(Animator animator, int parameter, float value, List<int> parameterList)
        {
            if (parameterList.Contains(parameter))
            {
                animator.SetFloat(parameter, value);
            }
        }

		// Updates the animator float.
		public static void UpdateAnimatorFloat(Animator animator, string parameterName, float value, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetFloat(parameterName, value);
            }
        }

        // Updates the animator integer.
        public static void UpdateAnimatorInteger(Animator animator, string parameterName, int value)
        {
            animator.SetInteger(parameterName, value);
        }

        // Updates the animator integer.
        public static void UpdateAnimatorInteger(Animator animator, int parameter, int value, List<int> parameterList)
        {
            if (parameterList.Contains(parameter))
            {
                animator.SetInteger(parameter, value);
            }
        }

        // Updates the animator integer.
        public static void UpdateAnimatorInteger(Animator animator, string parameterName, int value, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetInteger(parameterName, value);
            }
        }

        // Updates the animator bool after checking the parameter's existence.
        public static void UpdateAnimatorBoolIfExists(Animator animator, string parameterName, bool value)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Bool))
            {
                animator.SetBool(parameterName, value);
            }
        }

        // Updates an animator trigger if it exists
        public static void UpdateAnimatorTriggerIfExists(Animator animator, string parameterName)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(parameterName);
            }
        }

        // Triggers an animator trigger after checking for the parameter's existence.
        public static void SetAnimatorTriggerIfExists(Animator animator, string parameterName)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(parameterName);
            }
        }

        // Updates the animator float after checking for the parameter's existence.
        public static void UpdateAnimatorFloatIfExists(Animator animator, string parameterName, float value)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Float))
            {
                animator.SetFloat(parameterName, value);
            }
        }

        // Updates the animator integer after checking for the parameter's existence.
        public static void UpdateAnimatorIntegerIfExists(Animator animator, string parameterName, int value)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Int))
            {
                animator.SetInteger(parameterName, value);
            }
        }
    }
}
