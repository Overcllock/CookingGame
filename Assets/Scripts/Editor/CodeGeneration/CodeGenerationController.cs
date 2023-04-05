using Game.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Infrastructure.CodeGeneration
{
    public class CodeGenerationController
    {
        private Dictionary<Type, BaseCodeGenerator> generators_;

        public Type[] GeneratorsTypes { get; private set; }

        public CodeGenerationController()
        {
            ReflectionUtility.editorMode = true;
            generators_ = ReflectionUtility.InstantiateAllTypesMap<BaseCodeGenerator>((x) => x.Initialize());
            ReflectionUtility.editorMode = false;

            GeneratorsTypes = generators_.Keys.ToArray();
            Array.Sort(GeneratorsTypes, SortTypes);
        }

        public void GenerateAll()
        {
            foreach (var generator in generators_.Values)
            {
                generator.Generate();
            }

            AssetDatabase.Refresh();
        }

        public bool TryGet(Type type, out BaseCodeGenerator generator)
        {
            return generators_.TryGetValue(type, out generator);
        }

        public bool TryGet<T>(out T generator) where T : BaseCodeGenerator
        {
            if (generators_.TryGetValue(typeof(T), out var stored))
            {
                generator = stored as T;
                return true;
            }

            generator = null;
            return false;
        }

        public void Generate<T>() where T : BaseCodeGenerator
        {
            Type type = typeof(T);
            Generate(type);
        }

        public void Generate(Type type)
        {
            if (generators_.TryGetValue(type, out var generator))
            {
                generator.Generate(true);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Could not find generator of type: {type.Name}");
            }
        }

        private int SortTypes(Type x, Type y)
        {
            var xGen = generators_[x];
            var yGen = generators_[y];

            return xGen.priority.CompareTo(yGen.priority);
        }
    }
}