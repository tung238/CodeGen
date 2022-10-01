using CodeGen.Enums;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGen.Resolver;

namespace CodeGen.Models
{
    public sealed class DomainSettingsModel
    {
        public string Concern { get; }
        public string ConcernPlural { get; }

        public string Operation { get; }
        public PatternDirectoryType PatternType { get; }
        public PatternFileType PatternFileType { get; }
        public string ClassName { get; }
        public string ProjectName { get; }
        public string DomainName { get; }

        public string ApplicationName { get; }

        public string WebUiName { get; }

        public string DomainAbsolutePath { get; }

        public string ApplicationAbsolutePath { get; }
        public string WebUiAbsolutePath { get; }

        public DomainSettingsModel(string concern, string operation, PatternDirectoryType patternType, Workspace workspace, bool plural = false)
        {
            var projectList = workspace.CurrentSolution.Projects;

            var proj =
                projectList.FirstOrDefault(x => x.Name.Equals("Logic") || x.Name.Equals("Application"));

            if (proj == null)
                throw new Exception("Missing domain or logic project in solution");

            var p = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentUICulture);
            ConcernPlural = p.Pluralize(concern);
            Concern = concern;
            Operation = operation;
            PatternType = patternType;

            //ProjectName = proj.RelativePath;
            DomainName = proj.AssemblyName;
            DomainAbsolutePath = ResolveProjectAbsolutePath(proj.FilePath);

            var appPrj = projectList.FirstOrDefault(xamlGeneratedNamespace => xamlGeneratedNamespace.Name.Equals("Application"));
            ApplicationAbsolutePath = ResolveProjectAbsolutePath(appPrj.FilePath);
            ApplicationName = appPrj.AssemblyName;
            var webPrj = projectList.FirstOrDefault(xamlGeneratedNamespace => xamlGeneratedNamespace.Name.Equals("WebUI"));
            WebUiAbsolutePath = ResolveProjectAbsolutePath(webPrj.FilePath);
            WebUiName = webPrj.AssemblyName;

            PatternFileType = PatternFileNameResolver.Resolve(patternType);
            switch (patternType)
            {
                case PatternDirectoryType.Responses:
                    ClassName = $"{Concern}Dto";
                    break;
                case PatternDirectoryType.Events:
                    ClassName = $"{concern}{operation}d{PatternFileNameResolver.Resolve(patternType)}";
                    break;
                case PatternDirectoryType.EventHandlers:
                    ClassName = $"{concern}{operation}d{PatternFileNameResolver.Resolve(patternType)}";
                    break;
                case PatternDirectoryType. Controllers:
                    ClassName = $"{ConcernPlural}{PatternFileNameResolver.Resolve(patternType)}";
                    break;
                default:
                    ClassName = plural ? $"{operation}{ConcernPlural}{PatternFileNameResolver.Resolve(patternType)}" : $"{operation}{concern}{PatternFileNameResolver.Resolve(patternType)}";
                    break;

            }
            //LogUtility.Info($"ProjectName: {ProjectName}");
            //LogUtility.Info($"DomainName: {DomainName}");
            //LogUtility.Info($"DomainAbsolutePath: {DomainAbsolutePath}");
        }

        private static string ResolveProjectAbsolutePath(string absolutePath)
        {
            var lastIndexOf = absolutePath.LastIndexOf("\\", StringComparison.Ordinal);
            var domainAbsolutePath = absolutePath.Substring(0, lastIndexOf);
            return domainAbsolutePath;
        }
    }
}
