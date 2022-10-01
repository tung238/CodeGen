using CodeGen.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Resolver
{
    public static class ResponseTypeResolver
    {
        public static string Resolve(string concern, string operation, OperationType operationType, bool plural = false)
        {
            switch (operationType)
            {
                case OperationType.Command:
                    return operation == "Create" ? "int" : "Unit";
                case OperationType.Query:
                    var p = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentUICulture);
                    return plural ? $"PaginatedList<{concern}Dto>" : $"{concern}Dto";
                case OperationType.Event:
                    return $"DomainEventNotification<{concern}{operation}dEvent>";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
