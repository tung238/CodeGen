using System.Linq;
using System.Text.RegularExpressions;
using CodeGen.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGen.Utilities
{
    public static class CsharpClassParser
    {
        public static DomainModel Parse(string content, string filePath)
        {
            var cls = new DomainModel();
            cls.FilePath = filePath;
            var tree = CSharpSyntaxTree.ParseText(content);
            var members = tree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>();

            foreach (var member in members)
            {
                if (member is PropertyDeclarationSyntax property)
                {
                    cls.Properties.Add(new DomainModel.ModelProperty(
                         property.Identifier.ValueText,
                         property.Type.ToString())
                     );
                }

                if (member is NamespaceDeclarationSyntax namespaceDeclaration)
                {
                    var _namespace = namespaceDeclaration.Name.ToString();
                    cls.Namespace = _namespace;
                    var arr = namespaceDeclaration.Name.ToString().Split('.');
                    cls.BaseNamespace = string.Join(".", arr.Take(arr.Length - 2));
                }

                if (member is ClassDeclarationSyntax classDeclaration)
                {
                    cls.Name = classDeclaration.Identifier.ValueText;

                    cls.PrimaryKeyType = FindPrimaryKeyType(classDeclaration);
                }

                //if (member is MethodDeclarationSyntax method)
                //{
                //    Console.WriteLine("Method: " + method.Identifier.ValueText);
                //}
            }


            return cls;
        }

        private static string FindPrimaryKeyType(ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration == null)
            {
                return null;
            }

            if (classDeclaration.BaseList == null)
            {
                return null;
            }

            foreach (var baseClass in classDeclaration.BaseList.Types)
            {
                var match = Regex.Match(baseClass.Type.ToString(), @"<(.*?)>");
                if (match.Success)
                {
                    var primaryKey = match.Groups[1].Value;

                    //if (AppConsts.PrimaryKeyTypes.Any(x => x.Value == primaryKey))
                    //{
                        return primaryKey;
                    //}
                }
            }

            return null;
        }
    }
}
