using System.Collections;
using UnityEngine;

namespace Game.Events
{
    /// <summary>
    /// MonoBehaviour that allows user to execute coroutines
    /// externally.
    ///
    /// You only need one instance per scene.
    /// </summary>
    public class CoroutinesExecutionComponent : MonoBehaviour
    {
        public void ExecuteCoroutine(IEnumerator routine)
        {
            if (routine != null)
            {
                StartCoroutine(routine);
            }
            else
            {
                Debug.LogWarning($"Could not execute null routine!");
            }
        }

        public void CancelCoroutine(IEnumerator routine)
        {
            if (routine != null)
            {
                StopCoroutine(routine);
            }
            else
            {
                Debug.LogWarning($"Could not cancel null routine!");
            }
        }

        public void TerminateCoroutines()
        {
            StopAllCoroutines();
            Debug.LogWarning($"All ongoing coroutines have been terminated!");
        }
    }
}