using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.UI
{
    [RequireComponent(typeof(RectTransform))]
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    public class UIBaseLayout : MonoBehaviour
    {
        [ReadOnly]
        public GameObject rootObject;

        [ReadOnly]
        [PropertySpace(0, 10)]
        public RectTransform rootRect;

        [ContextMenu("Reset Root")]
        protected virtual void Reset()
        {
            rootObject = gameObject;
            rootRect = (RectTransform) transform;
        }
    }
}