using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Content
{
    [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
    public abstract class BaseSettingsScrobject : ScriptableObject, IEntrySource
    {
        [SerializeField]
        [HideIf("readOnlyId")]
        [Space(10)]
        protected string _id;

        [ShowInInspector]
        [ShowIf("readOnlyId")]
        [PropertyOrder(-1)]
        public string id { get { return _id; } }

        public void SetId(string id)
        {
            _id = id;
        }

#if UNITY_EDITOR
        protected virtual bool readOnlyId => false;
#endif
    }
}