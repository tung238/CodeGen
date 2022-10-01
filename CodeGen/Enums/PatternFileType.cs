using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Enums
{
    public enum PatternFileType
    {
        [Description("Command")]
        Command,
        [Description("Query")]
        Query,
        [Description("Event")]
        Event,
        [Description("Response")]
        Response,
        [Description("CommandHandler")]
        CommandHandler,
        [Description("QueryHandler")]
        QueryHandler,
        [Description("EventHandler")]
        EventHandler,
       [Description("Controller")]
        Controller
    }
}
