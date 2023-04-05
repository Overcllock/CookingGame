using System;

namespace Infrastructure.CodeGeneration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class GeneratorLinkAttribute : System.Attribute
    {
        public Type GeneratorType { get; private set; }

        public GeneratorLinkAttribute(Type generatorType)
        {
            GeneratorType = generatorType;
        }
    }
}