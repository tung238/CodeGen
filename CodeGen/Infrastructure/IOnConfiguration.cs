using System.Collections.Generic;
using CodeGen.Models;

namespace CodeGen.Infrastructure
{
    public interface IOnConfiguration
    {
        IWithNamespace ImportNamespaces(List<NamespaceModel> namespaceModels = null);

    }
}