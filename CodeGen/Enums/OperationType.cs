using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Enums
{
    public enum OperationType
    {
        [Description("COMMAND")]
        Command,
        [Description("QUERY")]
        Query,
        [Description("EVENT")]
        Event,
        [Description("UNSUPPORTED")]
        Unsupported
    }
}
