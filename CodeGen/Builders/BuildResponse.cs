using CodeGen.Enums;
using CodeGen.Infrastructure;
using CodeGen.Models;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace CodeGen.Builders
{
    public static class BuildResponse
    {
        public static void Build(DomainModel model, string concern, string operation, List<DomainModel.ModelProperty> properties, bool plural = false)
        {
            ClassAssembler
                .ConfigureHandler(model, concern, operation, PatternDirectoryType.Responses, plural)
                .ImportNamespaces()
                .CreateNamespace()
                .CreateClass(new[] { SyntaxFactory.Token(SyntaxKind.PublicKeyword) })
                .AddProperties(properties)
                .Build()
                ;
        }
    }
}
