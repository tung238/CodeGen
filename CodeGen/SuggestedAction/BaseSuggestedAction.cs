using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using CodeGen.Models;

namespace CodeGen.SuggestedAction
{
    public abstract class BaseSuggestedAction : ISuggestedAction
    {
        protected readonly CodeProvider CodeProvider;
        protected readonly SnapshotSpan Range;
        protected Document Document;
        protected string ClassName;

        protected DomainModel Model { get; }

        public bool HasActionSets
        {
            get { return false; }
        }
        public virtual string DisplayText => "BaseSuggestedAction";

        public ImageMoniker IconMoniker
        {
            get { return default(ImageMoniker); }
        }
        public string IconAutomationText
        {
            get
            {
                return null;
            }
        }
        public string InputGestureText
        {
            get
            {
                return null;
            }
        }
        public bool HasPreview
        {
            get { return false; }
        }
        public BaseSuggestedAction(CodeProvider codeProvider, SnapshotSpan range, string className, DomainModel model)
        {
            this.CodeProvider = codeProvider;
            this.ClassName = className;
            this.Model = model;
        }
        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<object>(string.Empty);
        }
        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<SuggestedActionSet>>(null);
        }

        public async Task<bool> HasSuggestedActionsAsync(CancellationToken cancellationToken)
        {
            return await Task.FromResult(true);
        }

        public abstract void Invoke(CancellationToken cancellationToken);
        public void Dispose()
        {
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            // This is a sample action and doesn't participate in LightBulb telemetry
            telemetryId = Guid.Empty;
            return false;
        }
    }
}