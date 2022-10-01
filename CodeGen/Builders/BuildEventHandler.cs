using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using CodeGen.Enums;
using CodeGen.Infrastructure;
using CodeGen.Models;
using CodeGen.Resolver;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGen.Builders
{
    public static class BuildEventHandler
    {
        public static void Build(DomainModel model, string concern, string operation, bool plural = false)
        {
            var properties = new List<DomainModel.ModelProperty>() { new DomainModel.ModelProperty("Context", "Common.Interfaces.IApplicationDbContext") };
            var p = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentUICulture);
           
            var tInObjectName =  $"{concern}{operation}dEventHandler";
            var eventName = $"{concern}{operation}dEvent";

            var operationTypeNamespace = NamespaceResolver.Resolve(concern, "Events");

            var statements = new StatementSyntax[] {
                SyntaxFactory.ParseStatement("var domainEvent = request.DomainEvent;"),
                SyntaxFactory.ParseStatement("Logger.LogInformation(\"Focus.Booking Domain Event: {DomainEvent}\", domainEvent.GetType().Name);"),
                SyntaxFactory.ParseStatement("return Task.CompletedTask;"),
            };

            properties.Add(new DomainModel.ModelProperty("Logger", $"ILogger<{tInObjectName}>"));
            ClassAssembler
                .ConfigureHandler(model, concern, operation, PatternDirectoryType.EventHandlers, plural)
                .ImportNamespaces(new List<NamespaceModel>
                {
                    new NamespaceModel("MediatR"),
                    new NamespaceModel("Microsoft.Extensions.Logging"),
                    new NamespaceModel(operationTypeNamespace,true),
                    new NamespaceModel($"{model.BaseNamespace}.Application.Common.Models"),
                    
                    new NamespaceModel("System.Collections.Generic"),
                    new NamespaceModel("System.Threading"),
                    new NamespaceModel("System.Threading.Tasks")
                })
                .CreateNamespace()
                .CreateClass(new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) }, tInObjectName)
                .WithInheritance(new List<string>
                {
                    $"INotificationHandler<DomainEventNotification<{eventName}>>"
                })
                .AddProperties(properties)
                .AddConstructor(properties)
                .ImplementMediatorHandlerInheritance("Task", $"DomainEventNotification<{eventName}>", statements)
                .Build()
                ;
        }

  
    }
}
