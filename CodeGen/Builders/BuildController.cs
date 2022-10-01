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
    public static class BuildController
    {
        public static void Build(DomainModel model, string concern, bool plural = false)
        {
            var properties = new List<DomainModel.ModelProperty>() { new DomainModel.ModelProperty("Context", "Common.Interfaces.IApplicationDbContext") };
            var p = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentUICulture);

            //var tInObjectName = $"{operation}{concern}Handler";
            //var commandName = $"{operation}{concern}Command";

            //var operationTypeNamespace = NamespaceResolver.Resolve(concern, "Commands");

            //properties.Add(new DomainModel.ModelProperty("Logger", $"ILogger<{tInObjectName}>"));
            ClassAssembler
                .ConfigureHandler(model, concern, "", PatternDirectoryType.Controllers, plural)
                .ImportNamespaces(new List<NamespaceModel>
                {
                    new NamespaceModel("MediatR"),
                    new NamespaceModel("Microsoft.Extensions.Logging"),
                    new NamespaceModel($"{model.BaseNamespace}.WebUI.Controllers"),
                    new NamespaceModel($"{model.BaseNamespace}.Application.Common.Models"),
                    new NamespaceModel($"{model.BaseNamespace}.Application.{p.Pluralize(concern)}.Commands"),
                    new NamespaceModel($"{model.BaseNamespace}.Application.{p.Pluralize(concern)}.Responses"),
                    new NamespaceModel($"{model.BaseNamespace}.Application.{p.Pluralize(concern)}.Queries"),
                    new NamespaceModel("System.Collections.Generic"),
                    new NamespaceModel("System.Threading"),
                    new NamespaceModel("Microsoft.AspNetCore.Mvc"),
                    new NamespaceModel("System.Threading.Tasks")
                })
                .CreateNamespace()
                .CreateClass(new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) })
                .WithInheritance(new List<string>
                {
                    $"ApiControllerBase"
                })
                .AddProperties(new List<DomainModel.ModelProperty>())
                .AddConstructor(new List<DomainModel.ModelProperty>())
                .ImplementControllerHandlerAction(new DomainModel.ModelProperty("HttpPost", ""), Operation.Create.ToString(), "async Task<ActionResult<int>>", new List<DomainModel.ModelProperty>()
                {
                    new DomainModel.ModelProperty($"command", $"Create{concern}Command")
                }, new StatementSyntax[]{
                    SyntaxFactory.ParseStatement("return await Mediator.Send(command);")
                })
                .ImplementControllerHandlerAction(new DomainModel.ModelProperty("HttpPut", "{id}"), Operation.Update.ToString(), "async Task<ActionResult>", new List<DomainModel.ModelProperty>()
                {
                    new DomainModel.ModelProperty($"id", $"int"),
                    new DomainModel.ModelProperty($"command", $"Update{concern}Command")
                }, new StatementSyntax[]{
                    SyntaxFactory.ParseStatement(@"if (id != command.Id)
            {
                return BadRequest();
            }"),
                    SyntaxFactory.ParseStatement("await Mediator.Send(command);"),
                    SyntaxFactory.ParseStatement("return NoContent();")
                })
                 .ImplementControllerHandlerAction(new DomainModel.ModelProperty("HttpDelete", "{id}"), Operation.Delete.ToString(), "async Task<ActionResult>", new List<DomainModel.ModelProperty>()
                {
                    new DomainModel.ModelProperty($"id", $"int")
                }, new StatementSyntax[]{
                    SyntaxFactory.ParseStatement($"await Mediator.Send(new Delete{concern}Command(id));"),
                    SyntaxFactory.ParseStatement("return NoContent();")
                })
                 .ImplementControllerHandlerAction(new DomainModel.ModelProperty("HttpGet", "{id}"), Operation.Get.ToString(), $"async Task<{concern}Dto>", new List<DomainModel.ModelProperty>()
                {
                    new DomainModel.ModelProperty($"id", $"int")
                }, new StatementSyntax[]{
                    SyntaxFactory.ParseStatement($"return await Mediator.Send(new Get{concern}Query(id));"),
                })
                  .ImplementControllerHandlerAction(new DomainModel.ModelProperty("HttpGet", ""), Operation.Get.ToString(), $"async Task<PaginatedList<{concern}Dto>>", new List<DomainModel.ModelProperty>()
                {
                    new DomainModel.ModelProperty("query", $"Get{p.Pluralize(concern)}Query")
                }, new StatementSyntax[]{
                    SyntaxFactory.ParseStatement($"return await Mediator.Send(query);"),
                }, true)
                .BuildController()
                ;
        }


    }
}
