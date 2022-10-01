using System;
using CodeGen.Enums;

namespace CodeGen.Resolver
{
    public static class PatternFileNameResolver
    {
        public static PatternFileType Resolve(PatternDirectoryType dirType) {
            switch (dirType)
            {
                case PatternDirectoryType.Commands: return PatternFileType.Command;
                case PatternDirectoryType.QueryHandlers: return PatternFileType.QueryHandler;
                case PatternDirectoryType.Queries: return PatternFileType.Query;
                case PatternDirectoryType.Responses: return PatternFileType.Response;
                case PatternDirectoryType.Events: return PatternFileType.Event;
                case PatternDirectoryType.EventHandlers: return PatternFileType.EventHandler;
                case PatternDirectoryType.CommandHandlers: return PatternFileType.CommandHandler;
                case PatternDirectoryType.Controllers: return PatternFileType.Controller;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
