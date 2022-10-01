using System.Collections.Generic;

namespace CodeGen.Models
{
    public class DomainModel
    {
        public string Name { get; set; }

        public string FilePath { get; set; }

        public string Namespace { get; set; }

        public List<ModelProperty> Properties { get; set; }

        public string PrimaryKeyType { get; set; }
        public string BaseNamespace { get; internal set; }

        public class ModelProperty
        {
            public string Name { get; set; }
            public string Type { get; set; }

            public ModelProperty(string name, string type)
            {
                Name = name;
                Type = type;
            }
        }

        public DomainModel()
        {
            Properties = new List<ModelProperty>();
        }
    }
}
