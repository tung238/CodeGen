using System;
using System.Collections.Generic;
using CodeGen.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGen.B2
{
    public class CommandHandler : ICodeGenCommand
    {
        private readonly DomainModel _classDef;
        private readonly CompilationUnitSyntax _compilationUnit;

        public CommandHandler(DomainModel classDef)
        {
            this._classDef = classDef;
            this._compilationUnit = SyntaxFactory.CompilationUnit().WithUsings(BuildUsing()).WithMembers(BuildClass());


        }

        public string GetFileName()
        {
            return $"{_classDef.Name}CommandHandler.cs";
        }

        public string GetFilePath()
        {
            return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(_classDef.FilePath) ?? throw new InvalidOperationException(), "", GetFileName());
        }

        public SyntaxList<UsingDirectiveSyntax> BuildUsing()
        {
            return
       SyntaxFactory.List<UsingDirectiveSyntax>(
        new UsingDirectiveSyntax[]{
            SyntaxFactory.UsingDirective(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.IdentifierName("Ardalis"),
                    SyntaxFactory.IdentifierName("GuardClauses"))),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.IdentifierName("Focus"),
                            SyntaxFactory.IdentifierName("Common")),
                        SyntaxFactory.IdentifierName(_classDef.Name)),
                    SyntaxFactory.IdentifierName("Commands"))),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.IdentifierName("Focus"),
                            SyntaxFactory.IdentifierName("Common")),
                        SyntaxFactory.IdentifierName(_classDef.Name)),
                    SyntaxFactory.IdentifierName("Events"))),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.IdentifierName("Focus"),
                        SyntaxFactory.IdentifierName("Core")),
                    SyntaxFactory.IdentifierName("Commands"))),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.IdentifierName("Focus"),
                        SyntaxFactory.IdentifierName("Core")),
                    SyntaxFactory.IdentifierName("Events"))),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.IdentifierName("Focus"),
                        SyntaxFactory.IdentifierName("Core")),
                    SyntaxFactory.IdentifierName("Ids"))),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.IdentifierName("Focus"),
                        SyntaxFactory.IdentifierName("Core")),
                    SyntaxFactory.IdentifierName("Storage"))),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.IdentifierName("Marten")),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.IdentifierName("MediatR")),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.IdentifierName("System")),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.IdentifierName("System"),
                    SyntaxFactory.IdentifierName("Linq"))),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.IdentifierName("System"),
                    SyntaxFactory.IdentifierName("Threading"))),
            SyntaxFactory.UsingDirective(
                SyntaxFactory.QualifiedName(
                    SyntaxFactory.QualifiedName(
                        SyntaxFactory.IdentifierName("System"),
                        SyntaxFactory.IdentifierName("Threading")),
                    SyntaxFactory.IdentifierName("Tasks")))});
        }
        public SyntaxList<MemberDeclarationSyntax> BuildClass()
        {
            return
            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            SyntaxFactory.IdentifierName(_classDef.Namespace))
        .WithMembers(
            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.ClassDeclaration($"{_classDef.Name}CommandHandler")
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithBaseList(
                    SyntaxFactory.BaseList(
                        SyntaxFactory.SeparatedList<BaseTypeSyntax>(
                            new SyntaxNodeOrToken[]{
                                SyntaxFactory.SimpleBaseType(
                                    SyntaxFactory.GenericName(
                                        SyntaxFactory.Identifier("ICommandHandler"))
                                    .WithTypeArgumentList(
                                        SyntaxFactory.TypeArgumentList(
                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                SyntaxFactory.IdentifierName($"Create{_classDef.Name}"))))),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                SyntaxFactory.SimpleBaseType(
                                    SyntaxFactory.GenericName(
                                        SyntaxFactory.Identifier("ICommandHandler"))
                                    .WithTypeArgumentList(
                                        SyntaxFactory.TypeArgumentList(
                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                SyntaxFactory.IdentifierName($"Update{_classDef.Name}"))))),
                                SyntaxFactory.Token(SyntaxKind.CommaToken),
                                SyntaxFactory.SimpleBaseType(
                                    SyntaxFactory.GenericName(
                                        SyntaxFactory.Identifier("ICommandHandler"))
                                    .WithTypeArgumentList(
                                        SyntaxFactory.TypeArgumentList(
                                            SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                SyntaxFactory.IdentifierName($"Delete{_classDef.Name}")))))})))
                .WithMembers(
                    SyntaxFactory.List<MemberDeclarationSyntax>(
                        GetMember()))))); 
        }

        private IEnumerable<MemberDeclarationSyntax> GetMember()
        {
            var result = new List<MemberDeclarationSyntax>();
            #region Properties
            foreach (var p in _classDef.Properties)
            {
                result.Add(SyntaxFactory.PropertyDeclaration(
                                SyntaxFactory.ParseTypeName(p.Type),
                                SyntaxFactory.Identifier(p.Name))
                            .WithModifiers(
                                SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                            .WithAccessorList(
                                SyntaxFactory.AccessorList(
                                    SyntaxFactory.List<AccessorDeclarationSyntax>(
                                        new AccessorDeclarationSyntax[]{
                                            SyntaxFactory.AccessorDeclaration(
                                                SyntaxKind.GetAccessorDeclaration)
                                            .WithSemicolonToken(
                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                            SyntaxFactory.AccessorDeclaration(
                                                SyntaxKind.SetAccessorDeclaration)
                                            .WithSemicolonToken(
                                                SyntaxFactory.Token(SyntaxKind.SemicolonToken))}))));
            }
            #endregion
            #region Private constructor
            var constructorParams = new List<SyntaxNodeOrToken>();
            var constructorBody = new List<StatementSyntax>();
            foreach (var p in _classDef.Properties)
            {
                if (constructorParams.Count > 0)
                {
                    constructorParams.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                }
                constructorParams.Add(SyntaxFactory.Parameter(
                                                SyntaxFactory.Identifier(p.Name.FirstCharacterToLower()))
                                            .WithType(
                                                SyntaxFactory.ParseTypeName(p.Type)));
                constructorBody.Add(SyntaxFactory.ExpressionStatement(
                                        SyntaxFactory.AssignmentExpression(
                                            SyntaxKind.SimpleAssignmentExpression,
                                            SyntaxFactory.IdentifierName(p.Name),
                                            SyntaxFactory.IdentifierName(p.Name.FirstCharacterToLower()))));
            }
            result.Add(SyntaxFactory.ConstructorDeclaration(
                                SyntaxFactory.Identifier($"Create{_classDef.Name}"))
                            .WithModifiers(
                                SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword)))
                            .WithParameterList(
                                SyntaxFactory.ParameterList(
                                    SyntaxFactory.SeparatedList<ParameterSyntax>(
                                        constructorParams.ToArray())))
                            .WithBody(
                                SyntaxFactory.Block(constructorBody.ToArray())));

            #endregion

            #region Methods
            constructorParams.Clear();
            constructorBody.Clear();
            var constructorCallParams = new List<SyntaxNodeOrToken>();
            foreach (var p in _classDef.Properties)
            {
                if (constructorParams.Count > 0)
                {
                    constructorParams.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                    constructorCallParams.Add(SyntaxFactory.Token(SyntaxKind.CommaToken));
                }
                constructorParams.Add(SyntaxFactory.Parameter(
                                                SyntaxFactory.Identifier(p.Name.FirstCharacterToLower()))
                                            .WithType(
                                                SyntaxFactory.ParseTypeName(p.Type)));
                constructorBody.Add(SyntaxFactory.ExpressionStatement(
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    SyntaxFactory.IdentifierName("Guard"),
                                                    SyntaxFactory.IdentifierName("Against")),
                                                SyntaxFactory.IdentifierName("Default")))
                                        .WithArgumentList(
                                            SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SeparatedList<ArgumentSyntax>(
                                                    new SyntaxNodeOrToken[]{
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.IdentifierName(p.Name.FirstCharacterToLower())),
                                                        SyntaxFactory.Token(SyntaxKind.CommaToken),
                                                        SyntaxFactory.Argument(
                                                            SyntaxFactory.InvocationExpression(
                                                                SyntaxFactory.IdentifierName("nameof"))
                                                            .WithArgumentList(
                                                                SyntaxFactory.ArgumentList(
                                                                    SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                                        SyntaxFactory.Argument(
                                                                            SyntaxFactory.IdentifierName(p.Name.FirstCharacterToLower()))))))})))));
                constructorCallParams.Add(SyntaxFactory.Argument(
                                                            SyntaxFactory.IdentifierName(p.Name.FirstCharacterToLower())));
            }
            constructorBody.Add(SyntaxFactory.ReturnStatement(
                        SyntaxFactory.ObjectCreationExpression(
                            SyntaxFactory.IdentifierName($"Create{_classDef.Name}"))
                        .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList<ArgumentSyntax>(constructorCallParams.ToArray())))));

            result.Add(SyntaxFactory.MethodDeclaration(
                                SyntaxFactory.IdentifierName($"Create{_classDef.Name}"),
                                SyntaxFactory.Identifier("Create"))
                            .WithModifiers(
                                SyntaxFactory.TokenList(
                                    new[]{
                                        SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                        SyntaxFactory.Token(SyntaxKind.StaticKeyword)}))
                            .WithParameterList(
                                SyntaxFactory.ParameterList(
                                    SyntaxFactory.SeparatedList<ParameterSyntax>(constructorParams.ToArray())))
                            .WithBody(
                                SyntaxFactory.Block(constructorBody.ToArray())));
            #endregion

            return result.ToArray();
        }

        public string GetSyntaxTree()
        {
            return _compilationUnit.NormalizeWhitespace().ToString();
        }
    }
}
