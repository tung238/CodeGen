using Microsoft.CodeAnalysis;

namespace CodeGen.Infrastructure
{
    public interface IWithNamespace
    {
        IWithNamespace CreateNamespace(string name = null);
        IWithInheritance CreateClass(SyntaxToken[] modifiers, string name = null);
    }
}