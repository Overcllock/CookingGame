using Game.Content;
using Infrastructure.CodeGeneration;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace UnityEditor
{
    [CustomEditor(typeof(BaseSettingsScrobject), true)]
    public class SettingsScrobjectInspector : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawDefinedConstants();
        }

        protected void DrawDefinedConstants()
        {
            if (!target.GetType().HasAttribute<DefinedConstantsAttribute>()) return;

            GUILayout.Space(20);
            if (GUILayout.Button("Generate types"))
            {
                TypeConstantsGenerator generator = new TypeConstantsGenerator();
                generator.GenerateConstantsForEntryScrobject(target.GetType());
            }
        }
    }
}