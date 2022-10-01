using CodeGen.Builders;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGen.Infrastructure;

namespace CodeGen
{
    [Export(typeof(ISuggestedActionsSourceProvider))]
    [Name("Test Suggested Actions")]
    [ContentType("code")]
    internal class EventSourcingActionsSourceProvider : ISuggestedActionsSourceProvider
    {
        private CodeProvider _codeProvider;

        [ImportingConstructor]
        public EventSourcingActionsSourceProvider([Import(typeof(VisualStudioWorkspace), AllowDefault = true)] Workspace workspace)
        {
            ClassAssembler.Workspace = workspace;
            _codeProvider = new CodeProvider(workspace);
        }

        public ISuggestedActionsSource CreateSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer)
        {
            return new EventSourcingActionsSource(this._codeProvider);
        }
    }
}
