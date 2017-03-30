using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeGenerator.Generate
{
    public sealed class CodeGeneratorFactory
    {
        private readonly string _baseTemplate;

        public CodeGeneratorFactory()
        {
            _baseTemplate = ReadBaseTemplate();
        }

        public List<ICodeGenerator> GetTemplateGenerateCodes()
        {
            var codeGenerators = new List<ICodeGenerator>();

            var generatorWrappers = new GeneratorWrapperFactory().GetGeneratorWrappers();

            foreach (var wrapper in generatorWrappers)
            {
                var usingStr = wrapper.Imports.Aggregate(string.Empty,
                    (current, import) => current + "using " + import.Namespace + ";\r\n");

                var generatorCode = _baseTemplate
                    .Replace("$Using$", usingStr)
                    .Replace("$FileName$", wrapper.CodeTemplate.FileName)
                    .Replace("$Properies$", "")
                    .Replace("$ChildTemplate$", wrapper.CodeBuilder.ToString());

                var codeGenerator = (ICodeGenerator)DynClassUtils.GetDynClassObject(generatorCode,
                    wrapper.CodeTemplate.FileName + "GenerateCode",
                    string.Empty, wrapper.Assemblies.Select(i => i.Name + i.Extension));

                if (codeGenerator != null)
                    codeGenerators.Add(codeGenerator);
            }

            return codeGenerators;
        }

        private string ReadBaseTemplate()
        {
            string template;

            var templateDirectory = Path.Combine(Environment.CurrentDirectory, "Template\\BaseCodeGeneratorTemplate.txt");
            using (var sr = new StreamReader(templateDirectory))
            {
                template = sr.ReadToEnd();
            }

            return template;
        }
    }
}