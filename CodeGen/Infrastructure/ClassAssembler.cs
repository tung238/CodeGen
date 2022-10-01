using System.Collections.Generic;
using System.Linq;
using CodeGen.Enums;
using CodeGen.Models;
using CodeGen.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGen.Infrastructure
{
    public sealed class ClassAssembler : IOnConfiguration, IWithNamespace, IWithInheritance
    {
        private readonly DomainSettingsModel _settings;
        private readonly DomainModel _model;
        private CompilationUnitSyntax _syntaxFactory;
        private NamespaceDeclarationSyntax _namespace;
        private ClassDeclarationSyntax _class;
        public static Workspace Workspace;

        private ClassAssembler()
        {
            _syntaxFactory = SyntaxFactory.CompilationUnit();
        }

        private ClassAssembler(DomainModel model, string concern, string operation, PatternDirectoryType patternType, bool plural)
        {
            _settings = new DomainSettingsModel(concern, operation, patternType, Workspace, plural);
            this._model = model;
            _syntaxFactory = SyntaxFactory.CompilationUnit();
        }

        public static IOnConfiguration ConfigureHandler(DomainModel model, string concern, string operation, PatternDirectoryType patternType, bool plural = false)
            => new ClassAssembler(model, concern, operation, patternType, plural);

        public static IOnConfiguration Configure()
            => new ClassAssembler();

        public IWithNamespace ImportNamespaces(List<NamespaceModel> namespaceModels = null)
        {
            namespaceModels?.ForEach(model =>
            {
                if (model.PrependWithDomainName)
                    model.Name = $"{_settings.DomainName}.{model.Name}";

                _syntaxFactory = _syntaxFactory.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(model.Name)));
            });

            return this;
        }

        public IWithNamespace CreateNamespace(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                switch (_settings.PatternType)
                {
                    case PatternDirectoryType.Controllers:
                        name = $"{_settings.WebUiName}.{_settings.PatternType.ToString()}";
                        break;
                    default:
                        name = $"{_settings.ApplicationName}.{_settings.ConcernPlural}.{_settings.PatternType.ToString()}";
                        break;
                }
            }   

            _namespace = SyntaxFactory.NamespaceDeclaration(
                SyntaxFactory.ParseName(name))
                .NormalizeWhitespace();

            return this;
        }
        public IWithInheritance CreateClass(SyntaxToken[] modifiers, string name = null)
        {
            if (string.IsNullOrEmpty(name))
                name = _settings.ClassName;

            LogUtility.Info($"Adding class: ${name}");
            _class = SyntaxFactory.ClassDeclaration(name);
            _class = _class.AddModifiers(modifiers);
            return this;
        }

        public IWithInheritance AddConstructor(IEnumerable<DomainModel.ModelProperty> properties)
        {
            var parameters = properties.Select(m => SyntaxFactory.Parameter(SyntaxFactory.Identifier(m.Name.FirstCharacterToLower()))
                .WithType(SyntaxFactory.ParseTypeName(m.Type))).ToArray();
            var statements = properties.Select(m => SyntaxFactory.ParseStatement($"{m.Name} = {m.Name.FirstCharacterToLower()};")).ToArray();

            var constructorDeclaration = SyntaxFactory.ConstructorDeclaration(_settings.ClassName)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(parameters)
                .WithBody(SyntaxFactory.Block(statements));

            _class = _class.AddMembers(constructorDeclaration);
            return this;
        }

        public IWithInheritance AddProperty(string propType, string name, SyntaxKind accessModifier)
        {
            var propertyDeclaration = SyntaxFactory
                .PropertyDeclaration(SyntaxFactory.ParseTypeName(propType), name)
                .AddModifiers(SyntaxFactory.Token(accessModifier))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            _class = _class.AddMembers(propertyDeclaration);

            return this;

        }

        public IWithInheritance AddProperties(IEnumerable<DomainModel.ModelProperty> properties)
        {
            var propertiesDeclaration = properties.Select(m =>
                SyntaxFactory
                .PropertyDeclaration(SyntaxFactory.ParseTypeName(m.Type), m.Name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))).ToArray();

            _class = _class.AddMembers(propertiesDeclaration);

            return this;

        }

        public IWithInheritance AddStartupConfigureServices()
        {
            var statements = new List<StatementSyntax>
            {
                SyntaxFactory.ParseStatement("services").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(".AddCorsRules()").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(".AddControllers()").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(".AddNewtonsoftJson()").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(";").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement("").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),

                SyntaxFactory.ParseStatement("services").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(".AddMvc()").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(";").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement("").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),

                SyntaxFactory.ParseStatement("services").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(".AddMicroserviceHealthChecks()").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(".AddDatabase(Configuration)").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(".AddLogic(Configuration)").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(";").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
            };

            var parameterList = new[]
            {
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("services"))
                    .WithType(SyntaxFactory.ParseTypeName("IServiceCollection")),
            };

            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName($"void"), "ConfigureServices")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(parameterList)
                .WithBody(SyntaxFactory.Block(statements));

            _class = _class.AddMembers(methodDeclaration);

            return this;
        }

        public IWithInheritance AddStartupConfigure()
        {
            var statements = new List<StatementSyntax>
            {
                SyntaxFactory.ParseStatement(@"if (env.IsEnvironment(""Local"") || env.IsDevelopment())"),
                SyntaxFactory.ParseStatement("app.UseDeveloperExceptionPage();").WithLeadingTrivia(SyntaxFactory.Tab),
                SyntaxFactory.ParseStatement("").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),

                SyntaxFactory.ParseStatement("app").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(".UseRouting()").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(".UseAuthorization()").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(".UseEndpoints(endpoints => endpoints.MapControllers())").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(".UseMicroserviceHealthChecks()").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
                SyntaxFactory.ParseStatement(";").WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed),
            };

            var parameterList = new[]
            {
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("app"))
                    .WithType(SyntaxFactory.ParseTypeName("IApplicationBuilder")),
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("env"))
                    .WithType(SyntaxFactory.ParseTypeName("IWebHostEnvironment")),
            };

            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName($"void"), "Configure")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(parameterList)
                .WithBody(SyntaxFactory.Block(statements));

            _class = _class.AddMembers(methodDeclaration);

            return this;
        }

        public IWithInheritance AddMethod(SyntaxToken[] modifiers, TypeSyntax returnType, string name, ParameterSyntax[] parameterArray, List<StatementSyntax> bodyStatementArray)
        {
            var methodDeclaration = SyntaxFactory.MethodDeclaration(returnType, name)
                .AddModifiers(modifiers)
                .AddParameterListParameters(parameterArray)
                .WithBody(SyntaxFactory.Block(bodyStatementArray));

            _class = _class.AddMembers(methodDeclaration);
            return this;
        }

        public IWithInheritance WithInheritance(List<string> inheritanceList)
        {
            if (inheritanceList == null)
                return this;

            foreach (var obj in inheritanceList)
            {
                _class = _class.AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(obj)));
            }

            return this;
        }

        public IWithInheritance ImplementMediatorHandlerInheritance(string responseTypeName, string requestTypeName, StatementSyntax[] statements = null)
        {
            if (statements == null)
            {
                statements = new StatementSyntax[] { SyntaxFactory.ParseStatement("throw new System.NotImplementedException();") };
            }
            var handleParameterList = new[]
            {
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("request")).WithType(SyntaxFactory.ParseTypeName(requestTypeName)),
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("cancellationToken")).WithType(SyntaxFactory.ParseTypeName("CancellationToken")),
            };

            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(responseTypeName), "Handle")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(handleParameterList)
                .WithBody(SyntaxFactory.Block(statements));

            _class = _class.AddMembers(methodDeclaration);

            return this;
        }

        public IWithInheritance ImplementControllerHandlerAction(DomainModel.ModelProperty attribute, string operation, string responseTypeName, List<DomainModel.ModelProperty> requestProperties, StatementSyntax[] statements = null, bool plural = false)
        {
            if (statements == null)
            {
                statements = new StatementSyntax[] { SyntaxFactory.ParseStatement("throw new System.NotImplementedException();") };
            }
            var handleParameterList = requestProperties.Select(p => SyntaxFactory.Parameter(SyntaxFactory.Identifier(p.Name.FirstCharacterToLower())).WithType(SyntaxFactory.ParseTypeName(p.Type))).ToArray();

            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(responseTypeName), $"{operation}{(plural ? _settings.ConcernPlural : _settings.Concern)}")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddParameterListParameters(handleParameterList)
                .AddAttributeLists(
                    SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(
                        SyntaxFactory.Attribute(
                            SyntaxFactory.IdentifierName(attribute.Name), 
                            SyntaxFactory.AttributeArgumentList(
                                SyntaxFactory.SingletonSeparatedList<AttributeArgumentSyntax>(
                                    SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        SyntaxFactory.Literal(attribute.Type)))))))))
                .WithBody(SyntaxFactory.Block(statements));

            _class = _class.AddMembers(methodDeclaration);

            return this;
        }

        public void Build()
        {
            _namespace = _namespace.AddMembers(_class);

            _syntaxFactory = _syntaxFactory.AddMembers(_namespace);

            var data = _syntaxFactory
                .NormalizeWhitespace()
                .ToFullString();

            var patternAbsolutePath = FileSystemUtility.CreateDirectory(new[] { _settings.ApplicationAbsolutePath, _settings.ConcernPlural, _settings.PatternType.ToString() });
          
            //LogUtility.Info($"patternAbsolutePath: ${patternAbsolutePath}");

            var concernAbsolutePath = FileSystemUtility.CreateDirectory(new[] { _settings.ApplicationAbsolutePath, _settings.ConcernPlural, _settings.PatternType.ToString() });
             
            //LogUtility.Info($"concernAbsolutePath: ${concernAbsolutePath}");

            var absoluteFilePath = FileSystemUtility.CreateFile(new[] { concernAbsolutePath, _settings.ClassName }, data);
            //LogUtility.Info($"absoluteFilePath: ${absoluteFilePath}");

            CleanUp();
        }

        public void BuildController()
        {
            _namespace = _namespace.AddMembers(_class);

            _syntaxFactory = _syntaxFactory.AddMembers(_namespace);

            var data = _syntaxFactory
                .NormalizeWhitespace()
                .ToFullString();

            var concernAbsolutePath = FileSystemUtility.CreateDirectory(new[] { _settings.WebUiAbsolutePath, _settings.PatternType.ToString() });

            //LogUtility.Info($"concernAbsolutePath: ${concernAbsolutePath}");

            var absoluteFilePath = FileSystemUtility.CreateFile(new[] { concernAbsolutePath, _settings.ClassName }, data);
            //LogUtility.Info($"absoluteFilePath: ${absoluteFilePath}");

            CleanUp();
        }

        public void Generate(string absolutePath, string className)
        {
            _namespace = _namespace.AddMembers(_class);

            _syntaxFactory = _syntaxFactory.AddMembers(_namespace);

            var data = _syntaxFactory
                .NormalizeWhitespace()
                .ToFullString();

            var absoluteFilePath = FileSystemUtility.CreateFile(new[] { absolutePath, className }, data);
            //LogUtility.Info($"absoluteFilePath: ${absoluteFilePath}");

            CleanUp();
        }

        private void CleanUp()
        {
            _class = null;
            _namespace = null;
            _syntaxFactory = null;
        }
    }
}
