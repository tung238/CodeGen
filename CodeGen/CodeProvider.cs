using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodeGen
{
    public class CodeProvider
    {
        private readonly Workspace _workspace;

        public CodeProvider(Workspace workspace)
        {
            this._workspace = workspace;
        }

        public async Task<string> GetClassNameAsync(Document document, int tokenPosition, CancellationToken cancellationToken)
        {
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
            var syntaxTree = await semanticModel.SyntaxTree.GetRootAsync(cancellationToken);

            var token = syntaxTree.FindToken(tokenPosition);

            if (token != null && token.Parent != null)
            {
                foreach (var node in token.Parent.AncestorsAndSelf())
                {
                    Type nodeType = node.GetType();
                    if (nodeType == typeof(ClassDeclarationSyntax))
                    {
                        return semanticModel.GetDeclaredSymbol(node).Name;
                    }
                    else if (nodeType == typeof(BlockSyntax))
                    {
                        // a block comes after the method declaration, the cursor is inside the block
                        // not what we want
                        return null;
                    }
                }
            }

            return null;
        }

        private void CreateFile(string documentName, string assemblyName, string content, string filePath)
        {
            //var project = workspace.CurrentSolution.Projects.FirstOrDefault(p => p.AssemblyName == assemblyName);
            //var document = project.Documents.FirstOrDefault(d => d.Name == fileName && d.FilePath == filePath);

            //if (document == null)
            //{
            //    document = project.AddDocument("newfile.cs", SourceText.From(content));
            //}
            //else
            //{
            //    document = document.WithText(SourceText.From(content));
            //}
            //if(!workspace.TryApplyChanges(document.Project.Solution)){
            //    Console.WriteLine("Something wrong");
            //}
            var project = this._workspace.CurrentSolution.Projects.First(p => p.AssemblyName == assemblyName);
            var document = project.Documents.FirstOrDefault(d => d.Name == documentName);
            var text = SourceText.From(content);

            if (document == null)
            {
                document = project.AddDocument(documentName, text, filePath: filePath) ;
            }
            else
            {
                document = document.WithText(text);
            }

            _workspace.TryApplyChanges(document.Project.Solution);
        }

        public void CreateFolders(Document currentDocument, string className, CancellationToken cancellationToken)
        {
            string[] folderToCreate = new string[] { "Commands", "Events", "Queries" };
            foreach(var folder in folderToCreate)
            {
                string pathString = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(currentDocument.FilePath), folder);
                System.IO.Directory.CreateDirectory(pathString);
            }
        }
    }
}
