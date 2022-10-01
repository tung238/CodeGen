using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGen.Models;
using CodeGen.SuggestedAction;
using CodeGen.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace CodeGen
{
    internal class EventSourcingActionsSource : ISuggestedActionsSource
    {
        private readonly CodeProvider _codeProvider;
        public event EventHandler<EventArgs> SuggestedActionsChanged;
        private string _className;
        private Document _document;
        private DomainModel _model;

        public EventSourcingActionsSource(CodeProvider codeProvider)
        {
            this._codeProvider = codeProvider;
        }
        public async Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            _document = range.Snapshot.TextBuffer.GetRelatedDocuments().FirstOrDefault();
            if (_document != null)
            {
                this._className = await GetClassNameAsync(range.Start, cancellationToken);
                _model = CsharpClassParser.Parse((await _document.GetTextAsync(cancellationToken)).ToString(), _document.FilePath);
            }
            else
                this._className = null;
            return this._className != null;
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            var actions = new List<ISuggestedAction>() { 
                new CreateCommandAction(this._codeProvider, range, _className, _model), 
            };

            return new List<SuggestedActionSet>() { new SuggestedActionSet("CodeGen", actions) };
        }
        public void Dispose()
        {
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            // This is a sample provider and doesn't participate in LightBulb telemetry
            telemetryId = Guid.Empty;
            return false;
        }

        private async Task<string> GetClassNameAsync(int tokenPosition, CancellationToken cancellationToken)
        {
            var semanticModel = await _document.GetSemanticModelAsync(cancellationToken);
            var syntaxTree = await semanticModel.SyntaxTree.GetRootAsync(cancellationToken);

            var token = syntaxTree.FindToken(tokenPosition);

            if (token.Parent != null)
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
    }
}