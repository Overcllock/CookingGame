using Game.Utilities;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Infrastructure.CodeGeneration
{
    public static class CodeGenerationFactory
    {
        public static CodeTypeDeclaration CreateClass(string name, TypeAttributes accessModifiers, bool isPartial = false)
        {
            CodeTypeDeclaration targetClass = new CodeTypeDeclaration(name);
            targetClass.IsClass = true;
            targetClass.IsPartial = isPartial;
            targetClass.TypeAttributes = accessModifiers;

            return targetClass;
        }

        public static CodeConstructor CreateConstructor(List<EditorFieldInfo> fields)
        {
            CodeConstructor constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            for (int i = 0; i < fields.Count; i++)
            {
                EditorFieldInfo fieldInfo = fields[i];
                string paramName = fieldInfo.Name.ToCamelCase();

                CodeParameterDeclarationExpression param = fieldInfo.ResultType.HasAlias() ?
                    new CodeParameterDeclarationExpression(fieldInfo.ResultType, paramName) :
                    new CodeParameterDeclarationExpression(fieldInfo.ResultType.GetValidName(), paramName);
                constructor.Parameters.Add(param);

                CodeFieldReferenceExpression reference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldInfo.Name);
                CodeAssignStatement assign = new CodeAssignStatement(reference, new CodeArgumentReferenceExpression(paramName));
                constructor.Statements.Add(assign);
            }

            return constructor;
        }

        public static List<CodeMemberField> CreateFields<T>(List<(string Name, T DefaultValue)> fieldsInfo,
            MemberAttributes attributes, bool validateName = true)
        {
            List<CodeMemberField> fields = new List<CodeMemberField>();
            for (int i = 0; i < fieldsInfo.Count; i++)
            {
                var nextInfo = fieldsInfo[i];

                if (nextInfo.DefaultValue != null && !nextInfo.Equals(default(T)))
                {
                    fields.Add(CreateField(nextInfo.Name, attributes, nextInfo.DefaultValue, validateName: validateName));
                }
            }

            return fields;
        }

        public static List<CodeMemberField> CreateFields<T>(List<string> fieldNames, MemberAttributes attributes, List<T> defaultValues = default)
        {
            List<CodeMemberField> fields = new List<CodeMemberField>();
            for (int i = 0; i < fieldNames.Count; i++)
            {
                string nextName = fieldNames[i];
                fields.Add(CreateField(nextName, attributes, defaultValues[i]));
            }

            return fields;
        }

        public static List<CodeMemberField> CreateFields(Type type, BindingFlags flags, MemberAttributes attributes)
        {
            FieldInfo[] fieldsInfo = type.GetFields(flags);
            return CreateFields(fieldsInfo, attributes);
        }

        public static List<CodeMemberField> CreateFields(MemberInfo[] fieldsInfo, MemberAttributes attributes)
        {
            List<CodeMemberField> fields = new List<CodeMemberField>(fieldsInfo.Length);
            for (int i = 0; i < fieldsInfo.Length; i++)
            {
                MemberInfo info = fieldsInfo[i];
                CodeMemberField field = CreateField(info.GetUnderlyingType(), info.Name, attributes);

                fields.Add(field);
            }

            return fields;
        }

        public static CodeMemberField CreateField<T>(string fieldName, MemberAttributes attributes,
            T defaultValue = default, bool useFullName = false, bool validateName = true)
        {
            return CreateField(typeof(T), fieldName, attributes, defaultValue, useFullName, validateName);
        }

        public static CodeMemberField CreateField(Type type, string fieldName, MemberAttributes attributes,
            object defaultValue = default, bool useFullName = false, bool validateName = true)
        {
            fieldName = fieldName.Replace(" ", string.Empty);
            if (validateName)
            {
                fieldName = Regex.Replace(fieldName, "[^0-9a-zA-Z]+", "");
            }

            CodeMemberField field = useFullName || type.ContainsAlias() ?
                new CodeMemberField(type, fieldName) :
                new CodeMemberField(type.GetValidName(), fieldName);

            field.Attributes = attributes;

            if (defaultValue != null && !defaultValue.Equals(default))
            {
                field.InitExpression = new CodePrimitiveExpression(defaultValue);
            }

            return field;
        }

        public static CodeMemberField CreateField(string type, string fieldName, MemberAttributes attributes, object defaultValue = default)
        {
            fieldName = fieldName.Replace(" ", string.Empty);
            fieldName = Regex.Replace(fieldName, "[^0-9a-zA-Z]+", "");

            CodeMemberField field = new CodeMemberField(type, fieldName);
            field.Attributes = attributes;

            if (defaultValue != null && !defaultValue.Equals(default))
            {
                field.InitExpression = new CodePrimitiveExpression(defaultValue);
            }

            return field;
        }

        public static CodeMemberField CreateReadonlyField(Type type, string fieldName, MemberAttributes attributes, CodeExpression initExpression)
        {
            fieldName = fieldName.Replace(" ", string.Empty);
            fieldName = Regex.Replace(fieldName, "[^0-9a-zA-Z]+", "");

            string refType = type.HasAlias() ? type.GetAlias() : type.GetValidName();
            refType = "readonly " + refType;

            CodeMemberField field = new CodeMemberField(new CodeTypeReference(refType), fieldName);
            field.Attributes = attributes;

            field.InitExpression = initExpression;
            return field;
        }

        public static List<CodeMemberField> CreateConstFields<T>(List<(string Name, T DefaultValue)> fieldsInfo, bool validateName = true)
        {
            MemberAttributes attributes = MemberAttributes.Public | MemberAttributes.Const;
            return CreateFields(fieldsInfo, attributes, validateName: validateName);
        }

        public static List<CodeMemberField> CreateConstFields<T>(List<string> fieldNames, List<T> values)
        {
            if (values == null)
            {
                Debug.LogWarning("Could not generate const fields without values");
                return null;
            }

            MemberAttributes attributes = MemberAttributes.Public | MemberAttributes.Const;
            return CreateFields(fieldNames, attributes, values);
        }

        public static CodeMemberField CreateConstField<T>(string fieldName, T value, bool validateName = true)
        {
            return CreateConstField(typeof(T), fieldName, value, validateName);
        }

        public static CodeMemberField CreateConstField(Type type, string fieldName, object value, bool validateName = true)
        {
            if (value == null)
            {
                Debug.LogWarning("Could not generate const fields without value");
                return null;
            }

            MemberAttributes attributes = MemberAttributes.Public | MemberAttributes.Const;
            return CreateField(type, fieldName, attributes, value, true, validateName);
        }

        public static CodeMemberProperty CreateProperty<T>(string name,
            CodeMethodReturnStatement getStatement,
            CodeAssignStatement setStatnement = null,
            MemberAttributes attributes = MemberAttributes.Public | MemberAttributes.Final)
        {
            CodeMemberProperty prop = new CodeMemberProperty();

            prop.Attributes = attributes;
            prop.Name = name;
            prop.Type = new CodeTypeReference(typeof(T));
            prop.HasGet = true;
            prop.HasSet = setStatnement != null;

            prop.GetStatements.Add(getStatement);
            if (setStatnement != null)
            {
                prop.SetStatements.Add(setStatnement);
            }

            return prop;
        }

        public static CodeSnippetTypeMember CreateAutoProperty(string type, string propertyName, bool privateSet = true, int padding = 0)
        {
            CodeSnippetTypeMember snippet = new CodeSnippetTypeMember();

            string set = privateSet ? "private set" : "set";
            string text = $"public {type} {propertyName} {{ get; {set}; }}";
            snippet.Text = text.PadLeft(text.Length + padding);

            return snippet;
        }

        public static CodeNamespace CreateNamespace(params string[] usings)
        {
            return CreateNamespace(null, usings);
        }

        public static CodeNamespace CreateNamespace(string name, params string[] usings)
        {
            CodeNamespace targetNamespace = string.IsNullOrEmpty(name) ?
                new CodeNamespace() :
                new CodeNamespace(name);

            if (usings != null)
            {
                for (int i = 0; i < usings.Length; i++)
                {
                    targetNamespace.Imports.Add(new CodeNamespaceImport(usings[i]));
                }
            }

            return targetNamespace;
        }

        public static CodeMemberMethod CreateMethod(string methodName, MemberAttributes attributes, Type returnType = null)
        {
            CodeTypeReference returnRef = null;
            if (returnType != null)
            {
                returnRef = returnType.ContainsAlias() ?
                    new CodeTypeReference(returnType) :
                    new CodeTypeReference(returnType.GetValidName());
            }

            return CreateMethod(methodName, attributes, returnRef);
        }

        public static CodeMemberMethod CreateMethod(string methodName, MemberAttributes attributes, string returnType)
        {
            CodeTypeReference returnRef = !string.IsNullOrEmpty(returnType) ?
                new CodeTypeReference(returnType) :
                null;

            return CreateMethod(methodName, attributes, returnRef);
        }

        public static CodeMemberMethod CreateMethod(string methodName, MemberAttributes attributes, CodeTypeReference returnType)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Attributes = attributes;
            method.Name = methodName;

            if (returnType != null)
            {
                method.ReturnType = returnType;
            }

            return method;
        }

        public static CodeObjectCreateExpression CreateObjectInitialization(string objectType, List<EditorFieldInfo> fields, string sourceRef = "")
        {
            CodeTypeReference variableType = new CodeTypeReference(objectType);
            CodeExpression[] attributes = new CodeExpression[fields.Count];

            for (int i = 0; i < fields.Count; i++)
            {
                CodeExpression target = string.IsNullOrEmpty(sourceRef) ?
                    (CodeExpression)new CodeThisReferenceExpression() :
                    (CodeExpression)new CodeVariableReferenceExpression(sourceRef);

                CodeFieldReferenceExpression reference = new CodeFieldReferenceExpression(target, fields[i].Name);
                attributes[i] = reference;
            }

            return new CodeObjectCreateExpression(variableType, attributes);
        }

        public static CodeObjectCreateExpression CreateObjectInitialization(Type objectType, MemberInfo[] providedFields)
        {
            CodeTypeReference variableType = ValidateType(objectType);
            CodeExpression[] attributes = new CodeExpression[providedFields.Length];
            for (int i = 0; i < providedFields.Length; i++)
            {
                CodeFieldReferenceExpression reference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), providedFields[i].Name);
                attributes[i] = reference;
            }

            return new CodeObjectCreateExpression(variableType, attributes);
        }

        public static CodeAssignStatement CreateFieldInitialization(string fieldName, Type fieldType, CodeExpression targetObject, params object[] parameters)
        {
            CodeTypeReference variableType = ValidateType(fieldType);
            return CreateFieldInitialization(fieldName, variableType, targetObject, parameters);
        }

        public static CodeAssignStatement CreateFieldInitialization(string fieldName, string fieldType, CodeExpression targetObject, params object[] parameters)
        {
            CodeTypeReference variableType = new CodeTypeReference(fieldType);
            return CreateFieldInitialization(fieldName, variableType, targetObject, parameters);
        }

        public static CodeAssignStatement CreateFieldInitialization(string fieldName, CodeTypeReference variableType, CodeExpression targetObject, params object[] parameters)
        {
            CodeFieldReferenceExpression fieldRef = new CodeFieldReferenceExpression(targetObject, fieldName);
            CodeObjectCreateExpression variableInitialization = PopulateInitializationExpression(variableType, parameters);
            return new CodeAssignStatement(fieldRef, variableInitialization);
        }

        public static CodeVariableDeclarationStatement CreateVariableDeclaration(Type type, string name, params object[] parameters)
        {
            CodeTypeReference variableType = ValidateType(type);

            CodeObjectCreateExpression variableInitialization = PopulateInitializationExpression(variableType, parameters);
            return new CodeVariableDeclarationStatement(variableType, name, variableInitialization);
        }

        public static CodeArgumentReferenceExpression CreateCustomExpression(string customCode)
        {
            return new CodeArgumentReferenceExpression(customCode);
        }

        public static CodeCompileUnit CreateUnit(ClassGenerationInfo info)
        {
            CodeCompileUnit unit = new CodeCompileUnit();

            info.Namespace.Types.Add(info.Class);
            if (unit.Namespaces.Contains(info.Namespace))
            {
                int itemIndex = unit.Namespaces.IndexOf(info.Namespace);
                info.Namespace = unit.Namespaces[itemIndex];
            }
            else
            {
                unit.Namespaces.Add(info.Namespace);
            }

            TryAddMembers<CodeMemberField>(info.Class, info.Fields);
            TryAddMembers<CodeMemberProperty>(info.Class, info.Props);
            TryAddMembers<CodeMemberEvent>(info.Class, info.Events);
            TryAddMembers<CodeConstructor>(info.Class, info.Constructors);
            TryAddMembers<CodeMemberMethod>(info.Class, info.Methods);
            TryAddMembers<CodeSnippetTypeMember>(info.Class, info.SnippetMembers);

            return unit;
        }

        public static List<CodeCompileUnit> CreateUnits(List<ClassGenerationInfo> infos)
        {
            List<CodeCompileUnit> units = new List<CodeCompileUnit>(infos.Count);

            for (int i = 0; i < infos.Count; i++)
            {
                ClassGenerationInfo classInfo = infos[i];
                CodeCompileUnit unit = CreateUnit(classInfo);
                units.Add(unit);
            }

            return units;
        }

        private static void TryAddMembers<T>(CodeTypeDeclaration targetClass, List<T> members) where T : CodeTypeMember
        {
            if (targetClass == null || members == null) return;

            for (int i = 0; i < members.Count; i++)
            {
                var nextMember = members[i];
                targetClass.Members.Add(nextMember);
            }
        }

        private static CodeObjectCreateExpression PopulateInitializationExpression(CodeTypeReference variableType, params object[] parameters)
        {
            CodeExpression[] attributes = null;
            if (parameters != null && parameters.Length > 0)
            {
                attributes = new CodeExpression[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    CodePrimitiveExpression expression = new CodePrimitiveExpression(parameters[i]);
                    attributes[i] = expression;
                }
            }

            return attributes != null ?
                new CodeObjectCreateExpression(variableType, attributes) :
                new CodeObjectCreateExpression(variableType);
        }

        private static CodeTypeReference ValidateType(Type fieldType)
        {
            return fieldType.ContainsAlias() ?
                new CodeTypeReference(fieldType) :
                new CodeTypeReference(fieldType.GetValidName());
        }
    }
}