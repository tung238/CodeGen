using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using CodeGen.Enums;
using CodeGen.Infrastructure;
using CodeGen.Models;
using CodeGen.Resolver;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeGen.Builders
{
    public static class BuildCommandHandler
    {
        public static void Build(DomainModel model, string concern, string operation, bool plural = false)
        {
            var properties = new List<DomainModel.ModelProperty>() { new DomainModel.ModelProperty("Context", "Common.Interfaces.IApplicationDbContext") };
            var p = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentUICulture);

            var tInObjectName = $"{operation}{concern}CommandHandler";
            var commandName = $"{operation}{concern}Command";

            var operationTypeNamespace = NamespaceResolver.Resolve(concern, "Commands");

            properties.Add(new DomainModel.ModelProperty("Logger", $"ILogger<{tInObjectName}>"));
            ClassAssembler
                .ConfigureHandler(model, concern, operation, PatternDirectoryType.CommandHandlers, plural)
                .ImportNamespaces(new List<NamespaceModel>
                {
                    new NamespaceModel("MediatR"),
                    new NamespaceModel("Microsoft.Extensions.Logging"),
                    new NamespaceModel(operationTypeNamespace,true),
                    new NamespaceModel($"{NamespaceResolver.Resolve(concern,"Responses")}",true),
                    new NamespaceModel("System.Collections.Generic"),
                    new NamespaceModel("System.Threading"),
                    new NamespaceModel("System.Threading.Tasks")
                })
                .CreateNamespace()
                .CreateClass(new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) }, tInObjectName)
                .WithInheritance(new List<string>
                {
                    $"IRequestHandler<{commandName} ,{ResponseTypeResolver.Resolve(concern, operation, OperationType.Command, plural)}>"
                })
                .AddProperties(properties)
                .AddConstructor(properties)
                .ImplementMediatorHandlerInheritance($"Task<{ResponseTypeResolver.Resolve(concern, operation, OperationType.Command, plural)}>", commandName)
                .Build()
                ;
        }


    }
}
