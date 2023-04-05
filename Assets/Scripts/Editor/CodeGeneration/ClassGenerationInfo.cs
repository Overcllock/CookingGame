using System.CodeDom;
using System.Collections.Generic;

namespace Infrastructure.CodeGeneration
{
    public struct ClassGenerationInfo
    {
        public string Folder;

        public CodeNamespace Namespace;
        public CodeTypeDeclaration Class;

        //members
        public List<CodeConstructor> Constructors;
        public List<CodeMemberField> Fields;
        public List<CodeMemberProperty> Props;
        public List<CodeMemberEvent> Events;
        public List<CodeMemberMethod> Methods;
        public List<CodeSnippetTypeMember> SnippetMembers;
    }
}