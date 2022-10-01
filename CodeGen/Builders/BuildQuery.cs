using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Linq;
using CodeGen.Enums;
using CodeGen.Infrastructure;
using CodeGen.Models;
using CodeGen.Resolver;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeGen.Builders
{
    public static class BuildQuery
    {
        public static void Build(DomainModel model, string concern, string operation, IEnumerable<DomainModel.ModelProperty> properties, bool plural = false)
        {
            var p = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentUICulture);

            ClassAssembler
                .ConfigureHandler(model, concern,operation, PatternDirectoryType.Queries, plural)
                .ImportNamespaces(new List<NamespaceModel>
                {
                    new NamespaceModel("MediatR"),
                    new NamespaceModel($"{NamespaceResolver.Resolve(concern,"Responses")}",true),
                    new NamespaceModel($"{model.BaseNamespace}.Application.Common.Models"),
                    new NamespaceModel("System.Collections.Generic"),
                })
                .CreateNamespace()
                .CreateClass(new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) })
                .WithInheritance(new List<string>
                {
                     $"IRequest<{ResponseTypeResolver.Resolve(concern, operation, OperationType.Query, plural)}>"
                })
                .AddProperties(properties)
                .AddConstructor(properties)
                .Build()
                ;
        }
    }
}
