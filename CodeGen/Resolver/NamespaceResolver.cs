using System.Data.Entity.Design.PluralizationServices;

namespace CodeGen.Resolver
{
    public static class NamespaceResolver
    {
        public static string Resolve(string concern, string operationType)
        {
            var p = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentUICulture);

            return $"{p.Pluralize(concern)}.{operationType}";
        }
    }
}
