using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using CodeGen.Enums;
using CodeGen.Infrastructure;
using CodeGen.Models;
using CodeGen.Resolver;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeGen.Builders
{
    public static class BuildQueryHandler
    {
        public static void Build(DomainModel model, string concern, string operation, bool plural = false)
        {
            var properties = new List<DomainModel.ModelProperty>() { 
                new DomainModel.ModelProperty("Context", "Common.Interfaces.IApplicationDbContext"),
                new DomainModel.ModelProperty("Mapper", "IMapper"),

            };
            var p = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentUICulture);
           
            var tInObjectName =  $"{operation}{concern}QueryHandler";
            var queryName = $"{operation}{concern}Query";

            var operationTypeNamespace = NamespaceResolver.Resolve(concern, "Queries");
            if (plural)
            {
                tInObjectName = $"{operation}{p.Pluralize(concern)}QueryHandler";
                queryName = $"{operation}{p.Pluralize(concern)}Query";
            }
                    
            properties.Add(new DomainModel.ModelProperty("Logger", $"ILogger<{tInObjectName}>"));
            ClassAssembler
                .ConfigureHandler(model, concern, operation, PatternDirectoryType.QueryHandlers, plural)
                .ImportNamespaces(new List<NamespaceModel>
                {
                    new NamespaceModel("MediatR"),
                    new NamespaceModel("Microsoft.Extensions.Logging"),
                    new NamespaceModel(operationTypeNamespace,true),
                    new NamespaceModel($"{NamespaceResolver.Resolve(concern,"Responses")}",true),
                    new NamespaceModel($"{model.BaseNamespace}.Application.Common.Models"),
                    new NamespaceModel("System.Collections.Generic"),
                    new NamespaceModel("AutoMapper"),

                    new NamespaceModel("System.Threading"),
                    new NamespaceModel("System.Threading.Tasks")
                })
                .CreateNamespace()
                .CreateClass(new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) }, tInObjectName)
                .WithInheritance(new List<string>
                {
                    $"IRequestHandler<{queryName}, {ResponseTypeResolver.Resolve(concern,operation, OperationType.Query, plural)}>"
                })
                .AddProperties(properties)
                .AddConstructor(properties)
                .ImplementMediatorHandlerInheritance($"Task<{ResponseTypeResolver.Resolve(concern,operation, OperationType.Query, plural)}>" , queryName)
                .Build()
                ;
        }

  
    }
}
