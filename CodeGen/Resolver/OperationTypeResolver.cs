using CodeGen.Enums;

namespace CodeGen.Resolver
{
    public static class OperationTypeResolver
    {
        public static OperationType Resolve(string operationType)
        {
            switch (operationType)
            {
                case "command":
                    return OperationType.Command;
                case "query":
                    return OperationType.Query;
                default:
                    return OperationType.Unsupported;
            }
        }
    }
}
