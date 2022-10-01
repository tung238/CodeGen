using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeGen.Builders;
using CodeGen.Models;

namespace CodeGen.SuggestedAction
{
    public class CreateCommandAction : BaseSuggestedAction
    {
        public override string DisplayText => "CreateCommandAction";

        public CreateCommandAction(CodeProvider codeProvider, SnapshotSpan range, string className, DomainModel model) :base(codeProvider, range, className, model)
        {

        }

        public override void Invoke(CancellationToken cancellationToken)
        {
            BuildCommand.Build(Model, ClassName, Enums.Operation.Create.ToString(), 
                Model.Properties.Where(m => !(new List<string>(){ "Id", "DomainEvents" }).Contains(m.Name)).ToList());
            BuildCommand.Build(Model, ClassName, Enums.Operation.Update.ToString(),
                Model.Properties.Where(m => !(new List<string>() {"DomainEvents"}).Contains(m.Name)).ToList());
            BuildCommand.Build(Model, ClassName, Enums.Operation.Delete.ToString(),
                Model.Properties.Where(m => (new List<string>() {"Id"}).Contains(m.Name)).ToList());

            BuildEvent.Build(Model, ClassName, Enums.Operation.Create.ToString(), 
                new List<DomainModel.ModelProperty> { new DomainModel.ModelProperty("Item", ClassName) });
            BuildEvent.Build(Model, ClassName, Enums.Operation.Update.ToString(),
                new List<DomainModel.ModelProperty> { new DomainModel.ModelProperty("Item", ClassName) });
            BuildEventHandler.Build(Model, ClassName, Enums.Operation.Create.ToString());
            BuildEventHandler.Build(Model, ClassName, Enums.Operation.Update.ToString());

            BuildCommandHandler.Build(Model, ClassName, Enums.Operation.Create.ToString());
            BuildCommandHandler.Build(Model, ClassName, Enums.Operation.Update.ToString());
            BuildCommandHandler.Build(Model, ClassName, Enums.Operation.Delete.ToString());

            BuildQuery.Build(Model, ClassName, Enums.Operation.Get.ToString(), Model.Properties.Where(m => new List<string> { "Id" }.Contains(m.Name)));
            
            var properties = Model.Properties.Where(m => !(new List<string> { "DomainEvents" }.Contains(m.Name))).ToList();
            properties.Add(new DomainModel.ModelProperty("PageNumber", "int"));
            properties.Add(new DomainModel.ModelProperty("PageSize", "int"));
            BuildQuery.Build(Model, ClassName, Enums.Operation.Get.ToString(), properties, true);
            
            BuildResponse.Build(Model, ClassName, Enums.Operation.Get.ToString(),
                Model.Properties.Where(m => !(new List<string>() { "Id", "DomainEvents" }).Contains(m.Name)).ToList());
            BuildQueryHandler.Build(Model, ClassName, Enums.Operation.Get.ToString());
            BuildQueryHandler.Build(Model, ClassName, Enums.Operation.Get.ToString(), true);

            BuildController.Build(Model, ClassName);


        }
    }
}
