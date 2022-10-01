using System.Collections.Generic;
using CodeGen.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGen.Infrastructure
{
    public interface IWithInheritance
    {
        IWithInheritance WithInheritance(List<string> inheritanceLis);
        IWithInheritance ImplementMediatorHandlerInheritance(string responseTypeName, string requestTypeName, StatementSyntax[] statements = null);
        IWithInheritance ImplementControllerHandlerAction(DomainModel.ModelProperty attribute, string operation, string responseTypeName, List<DomainModel.ModelProperty> requestProperties, StatementSyntax[] statements = null, bool plural = false);
        IWithInheritance AddConstructor(IEnumerable<DomainModel.ModelProperty> properties);     
        IWithInheritance AddProperty(string propType, string name, SyntaxKind accessModifier);
        IWithInheritance AddProperties(IEnumerable<DomainModel.ModelProperty> properties);

        IWithInheritance AddStartupConfigureServices();
        IWithInheritance AddStartupConfigure();
        IWithInheritance AddMethod(SyntaxToken[] modifiers,
            TypeSyntax returnType,
            string name,
            ParameterSyntax[] parameterArray,
            List<StatementSyntax> bodyStatementArray);

        void Build();
        void Generate(string absolutePath, string className);
        void BuildController();
    }
}