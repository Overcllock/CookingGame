using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Infrastructure.CodeGeneration
{
    /// <summary>
    /// Base class for any code generator implementation.
    /// Naming should be similar to "MyFeatureGenerator", where
    /// first part should represent generated content 
    /// and could potentially be used for editor tools.
    /// </summary>
    public abstract class BaseCodeGenerator
    {
        public virtual float priority { get { return 0; } }

        protected abstract List<ClassGenerationInfo> PrepareData();
        public virtual void Initialize() { }

        public void Generate(bool recompile = false)
        {
            List<ClassGenerationInfo> data = PrepareData();
            GenerateSourceFiles(data, recompile);
        }

        protected void GenerateSourceFiles(bool recompile, params ClassGenerationInfo[] data)
        {
            GenerateSourceFiles(data.ToList(), recompile);
        }

        protected void GenerateSourceFiles(List<ClassGenerationInfo> data, bool recompile = false)
        {
            if (data == null) return;

            GetComponents(out CodeDomProvider provider, out CodeGeneratorOptions options);

            for (int i = 0; i < data.Count; i++)
            {
                ClassGenerationInfo info = data[i];
                GenerateSourceFile(info, provider, options);
            }

            if (recompile)
            {
                AssetDatabase.Refresh();
            }
        }

        protected void GenerateSourceFile(ClassGenerationInfo info, bool recompile = false)
        {
            GetComponents(out CodeDomProvider provider, out CodeGeneratorOptions options);
            GenerateSourceFile(info, provider, options);

            if (recompile)
            {
                AssetDatabase.Refresh();
            }
        }

        protected void GenerateSourceFile(ClassGenerationInfo info, CodeDomProvider provider, CodeGeneratorOptions options)
        {
            CodeCompileUnit unit = CodeGenerationFactory.CreateUnit(info);

            if (!Directory.Exists(info.Folder))
            {
                Directory.CreateDirectory(info.Folder);
            }

            string filePath = Path.Combine(info.Folder, $"{info.Class.Name}.cs");

            using (StreamWriter sourceWriter = new StreamWriter(filePath))
            {
                provider.GenerateCodeFromCompileUnit(unit, sourceWriter, options);
            }

            UnityEngine.Debug.Log($"Successfully generated class: [<color=white>{filePath}</color>]");
        }

        private void GetComponents(out CodeDomProvider provider, out CodeGeneratorOptions options)
        {
            provider = CodeDomProvider.CreateProvider("CSharp");
            options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
        }
    }
}