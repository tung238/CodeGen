using System.Collections.Generic;
using CodeGen.Enums;
using CodeGen.Infrastructure;
using CodeGen.Models;
using CodeGen.Resolver;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeGen.Builders
{
    public static class BuildEvent
    {
        public static void Build(DomainModel model, string concern, string operation, List<DomainModel.ModelProperty> properties)
        {
            ClassAssembler
                .ConfigureHandler(model, concern, operation, PatternDirectoryType.Events)
                .ImportNamespaces(new List<NamespaceModel>
                {
                    new NamespaceModel("MediatR"),
                    new NamespaceModel($"{model.BaseNamespace}.Domain.Entities"),
                    new NamespaceModel($"{model.BaseNamespace}.Domain.Common"),

                })
                .CreateNamespace()
                .CreateClass(new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) })
                .WithInheritance(new List<string>
                {
                    $"Domain.Common.DomainEvent"
                })
                .AddProperties(properties)
                .AddConstructor(properties)
                .Build()
                ;
        }
    }
}
