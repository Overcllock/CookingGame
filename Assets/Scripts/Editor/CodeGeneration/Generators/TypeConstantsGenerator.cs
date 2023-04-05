using Game.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Infrastructure.CodeGeneration
{
    [IgnoreGeneratorView]
    public class TypeConstantsGenerator : BaseCodeGenerator
    {
        private ConstantsInfo<int> _constants;

        public TypeConstantsGenerator()
        {
        }

        public TypeConstantsGenerator(ConstantsInfo<int> constants)
        {
            _constants = constants;
        }

        public void GenerateConstantsForEntryScrobject(Type type)
        {
            var assets = AssetDatabaseUtility.GetAssets(type);

            string name = CodeGenerationUtility.GetScrobjectConstantsName(type);

            ConstantsInfo<string> constants = new ConstantsInfo<string>(name, type.Namespace);
            
            MonoScript script = MonoScript.FromScriptableObject((ScriptableObject)assets.First());
            var scriptPath = AssetDatabase.GetAssetPath(script);

            constants.path = System.IO.Path.GetDirectoryName(scriptPath).SlashSafe();
            
            foreach (var asset in assets)
            {
                var config = (IEntrySource)asset;

                if (string.IsNullOrEmpty(config.id))
                {
                    Debug.LogWarning($"Could not create constant for config - id is not specified");
                    continue;
                }

                constants.fields.Add((StringUtility.ToScreamingCaps(config.id), config.id));
            }

            GenerateType(constants);
        }

        public void GenerateType(string className, List<(string name, int id)> info)
        {
            ConstantsInfo<int> constantInfo = new ConstantsInfo<int>(className);
            constantInfo.Add(info);

            GenerateType(constantInfo);
        }

        public void GenerateType<T>(ConstantsInfo<T> constantInfo)
        {
            ClassGenerationInfo info = GetClassInfo(constantInfo);
            GenerateSourceFile(info, true);
        }

        protected override List<ClassGenerationInfo> PrepareData()
        {
            if (_constants == null) return default;

            return new List<ClassGenerationInfo>
            {
                GetClassInfo(_constants)
            };
        }

        private ClassGenerationInfo GetClassInfo<T>(ConstantsInfo<T> constantInfo)
        {
            if (string.IsNullOrEmpty(constantInfo.className))
            {
                UnityEngine.Debug.LogWarning($"Could not generate type constants - class name is empty.");
                return default;
            }

            string path = string.IsNullOrEmpty(constantInfo.path) ?
                CodeGenerationConsts.CONTENT_GENERATED_PATH :
                constantInfo.path;

            return new ClassGenerationInfo
            {
                Folder = IOUtility.GetAbsolutePath(path),

                Namespace = CodeGenerationFactory.CreateNamespace(constantInfo.@namespace),
                Class = CodeGenerationFactory.CreateClass(constantInfo.className, TypeAttributes.Public),
                Fields = CodeGenerationFactory.CreateConstFields(constantInfo.fields, false)
            };
        }
    }
}