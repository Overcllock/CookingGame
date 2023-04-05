using System;
using System.Runtime.CompilerServices;

namespace Game.Utilities
{
    public static class Precondition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MustBeTrue(bool condition, string message = "Condition is not true.")
        {
            if (!condition)
                throw new Exception(message);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MustBeFalse(bool condition, string message = "Condition is not false.")
        {
            if (condition)
                throw new Exception(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MustBeNull(object obj, string message = "Reference is not null.")
        {
#if UNITY_5_3_OR_NEWER
            // https://docs.unity3d.com/ScriptReference/Object-operator_eq.html
            if (obj is UnityEngine.Object unityObject)
            {
                if (unityObject != null)
                    throw new Exception(message);
            }
#endif
            
            if (obj != null)
                throw new Exception(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MustNotBeNull(object obj, string message = "Reference is null.")
        {
#if UNITY_5_3_OR_NEWER
            // https://docs.unity3d.com/ScriptReference/Object-operator_eq.html
            if (obj is UnityEngine.Object unityObject)
            {
                if (unityObject == null)
                    throw new Exception(message);
            }
#endif
            
            if (obj == null)
                throw new Exception(message);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MustNotBeNullOrEmpty(string str, string message = "String is null or empty.")
        {
            if (string.IsNullOrEmpty(str))
                throw new Exception(message);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgumentMustNotBeNull(object obj, string paramName = "paramName", string message = "Argument is null.")
        {
#if UNITY_5_3_OR_NEWER
            // https://docs.unity3d.com/ScriptReference/Object-operator_eq.html
            if (obj is UnityEngine.Object unityObject)
            {
                if (unityObject == null)
                    throw new ArgumentNullException(paramName, message);
            }
#endif
            
            if (obj == null)
                throw new ArgumentNullException(paramName, message);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArgumentMustNotBeNullOrEmpty(string str, string paramName = "paramName", string message = "Argument string is null or empty.")
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException(message, paramName);
        }
    }
}