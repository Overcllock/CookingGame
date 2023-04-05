using System;
using System.Collections.Generic;

namespace Infrastructure.CodeGeneration
{
    public class ConstantsInfo<T>
    {
        public string className;
        public string @namespace;
        public string path;

        public List<(string name, T value)> fields;

        public ConstantsInfo()
        {
            Init();
        }

        public ConstantsInfo(List<(string name, T value)> values)
        {
            Init();
            Add(values);
        }

        public ConstantsInfo(string className, string @namespace = "")
        {
            this.className = className;
            this.@namespace = @namespace;

            Init();
        }

        private void Init()
        {
            fields = new List<(string, T)>();
            fields.Add(("UNDEFINED", GetDefaultValue()));
        }

        public void Add((string name, T value) tuple)
        {
            fields.Add(tuple);
        }

        public void Add(List<(string name, T value)> values)
        {
            fields.AddRange(values);
        }

        private T GetDefaultValue()
        {
            if (typeof(T) == typeof(string)) return (T)(object)"Undefined";

            return default(T);
        }

        public override string ToString()
        {
            Func<(string, T), string> getter =
                (x) => $"FieldName: [{x.Item1}] FieldValue: [{x.Item2}]";

            return $"Class name: [{className}] " +
                $"Namespace: [{@namespace}] " +
                $"\nFields: {StringUtility.GetCompositeString(fields, true, getter, numerate: false)}";
        }
    }
}