using System.Collections.Generic;
using System.Text;

namespace CodeGenerator.Generate.GenerateModel
{
    public class GeneratorWrapper
    {
        public StringBuilder CodeBuilder { get; set; }

        public CodeTemplate CodeTemplate { get; set; }

        public List<Assembly> Assemblies { get; set; }

        public List<Import> Imports { get; set; }

        public List<Property> Properties { get; set; }

        public GeneratorWrapper()
        {
            CodeBuilder = new StringBuilder();
            CodeTemplate = new CodeTemplate();
            Assemblies = new List<Assembly> { new Assembly { Name = "CodeGenerator", Extension = ".exe" } };
            Imports = new List<Import>
            {
                new Import {Namespace = "System.Linq"},
                new Import {Namespace = "System.Collections.Generic"},
                new Import {Namespace = "System.Text"},
                new Import {Namespace = "CodeGenerator.Pdm"},
                new Import {Namespace = "CodeGenerator.Generate"}
            };
            Properties = new List<Property>();
        }
    }
}