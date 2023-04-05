using System;
using System.Diagnostics;
using UnityEngine;

namespace Game.Content
{
    [Conditional("UNITY_EDITOR")]
    public class BoundIdAttribute : PropertyAttribute
    {
        public Type type { get; }
        public string typeName { get; }

        public BoundIdAttribute(Type type)
        {
            this.type = type;
        }

        public BoundIdAttribute(string typeName)
        {
            this.typeName = typeName;
        }
    }
}