using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Enums
{
    public enum PatternDirectoryType
    {
        [Description("Commands")]
        Commands,
        [Description("Queries")]
        Queries,
        [Description("Events")]
        Events,
        [Description("Responses")]
        Responses,
        [Description("QueryHandlers")]
        QueryHandlers,
        [Description("CommandHandlers")]
        CommandHandlers,
        [Description("EventHandlers")]
        EventHandlers,
        [Description("Controllers")]
        Controllers
    }
}
