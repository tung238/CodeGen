using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Models
{
    public class NamespaceModel
    {
        public string Name { get; set; }
        public bool PrependWithDomainName { get; }

        public NamespaceModel(string name, bool prependWithDomainName = false)
        {
            Name = name;
            PrependWithDomainName = prependWithDomainName;
        }
    }
}
