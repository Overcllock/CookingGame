using System;
using System.CodeDom;

namespace Infrastructure.CodeGeneration
{
    public static class CodeGenerationExtensions
    {
        public static void AddSummary(this CodeMemberField member, string summary)
        {
            member.Comments.Add(new CodeCommentStatement("<summary>", true));
            member.Comments.Add(new CodeCommentStatement(summary, true));
            member.Comments.Add(new CodeCommentStatement("</summary>", true));
        }

        public static void AddAttribute(this CodeTypeMember member, Type attributeType, params object[] arguments)
        {
            var attribute = new CodeAttributeDeclaration(new CodeTypeReference(attributeType));

            if (arguments != null)
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    var argument = new CodeAttributeArgument(new CodePrimitiveExpression(arguments[i]));
                    attribute.Arguments.Add(argument);
                }
            }

            member.CustomAttributes.Add(attribute);
        }

        public static void AddAttributeWithSnippet(this CodeTypeMember member, Type attributeType, string snippetArgument)
        {
            var name = attributeType.Name.Replace("Attribute", "");
            var attribute = new CodeAttributeDeclaration(name);

            if (!snippetArgument.IsNullOrWhiteSpace())
            {
                var expression = new CodeSnippetExpression(snippetArgument);
                var argument = new CodeAttributeArgument(expression);
                attribute.Arguments.Add(argument);
            }

            member.CustomAttributes.Add(attribute);
        }
    }
}