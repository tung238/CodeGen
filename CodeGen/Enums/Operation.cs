using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Enums
{
    public enum Operation
    {
        [Description("Create")]
        Create,
        [Description("Update")]
        Update,
        [Description("Delete")]
        Delete,
        [Description("Get")]
        Get,
    }
}
