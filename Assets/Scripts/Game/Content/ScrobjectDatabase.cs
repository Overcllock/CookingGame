using Sirenix.OdinInspector;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.Utilities;
#endif
using UnityEditor;
using UnityEngine;

namespace Game.Content
{
    public abstract class ScrobjectDatabase : ScriptableObject, IContentSource
    {
        [SerializeField]
        [HideInInspector]
        protected List<BaseSettingsScrobject> _allScrobjects;

        [PropertySpace(10)]
        [PropertyOrder(50)]
        [ShowInInspector]
        [PropertyTooltip("Database scrobjects:")]
        [ListDrawerSettings(HideAddButton = true, DraggableItems = false, IsReadOnly = true, NumberOfItemsPerPage = 30)]
        public List<BaseSettingsScrobject> allScrobjects { get { return _allScrobjects; } }

#if UNITY_EDITOR
        [Button(50)]
        [PropertySpace(15)]
        [PropertyOrder(100)]
        protected void UpdateContent()
        {
            UpdateContent(true);
        }

        public void UpdateContent(bool saveAssets = false)
        {
            UpdateScrobjects();

            OnUpdateContent();
            EditorUtility.SetDirty(this);

            if (saveAssets)
            {
                AssetDatabase.SaveAssets();
            }

            string message = $"[ {GetType().Name} ] content is updated.";

            if (!allScrobjects.IsNullOrEmpty())
            {
                message += $"\n Current scrobjects: \n{StringUtility.GetCompositeString(allScrobjects)}";
            }

            Debug.Log(message);
        }

        private void UpdateScrobjects()
        {
            var moduleName = GetType().Namespace;

            _allScrobjects = new List<BaseSettingsScrobject>();
            var scrobjects = AssetDatabaseUtility.GetAssets<BaseSettingsScrobject>();
            foreach (var scrobject in scrobjects)
            {
                if (scrobject.GetType().Namespace == moduleName)
                {
                    _allScrobjects.Add(scrobject);
                }
            }
        }
        protected virtual void OnUpdateContent()
        {
        }
#endif
    }
}