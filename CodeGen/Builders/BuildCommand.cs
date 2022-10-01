using System.Collections.Generic;
using CodeGen.Enums;
using CodeGen.Infrastructure;
using CodeGen.Models;
using CodeGen.Resolver;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeGen.Builders
{
    public static class BuildCommand
    {
        public static void Build(DomainModel model, string concern, string operation, List<DomainModel.ModelProperty> properties)
        {
            ClassAssembler
                .ConfigureHandler(model, concern, operation, PatternDirectoryType.Commands)
                .ImportNamespaces(new List<NamespaceModel>
                {
                    new NamespaceModel("MediatR"),
                })
                .CreateNamespace()
                .CreateClass(new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) })
                .WithInheritance(new List<string>
                {
                    $"IRequest<{ResponseTypeResolver.Resolve(concern, operation, OperationType.Command)}>"
                })
                .AddProperties(properties)
                .AddConstructor(properties)
                .Build()
                ;
        }
    }
}
