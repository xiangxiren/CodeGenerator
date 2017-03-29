using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeGenerator.Generate
{
    public sealed class CodeGeneratorFactory
    {
        private List<string> _assemblys = new List<string> { "CodeGenerator", "System.Core" };
        private List<string> _namespace = new List<string> { "System.Linq", "CodeGenerator.Pdm", "CodeGenerator.Generate" };

        private const string CodeTemplateMatch = @"^<%@ CodeTemplate Language=C# FileName=Entity %>$";


        public List<ICodeGenerator> GetTemplateGenerateCodes()
        {
            var codeGenerators = new List<ICodeGenerator>();

            var templates = GetTemplates();


            foreach (var template in templates)
            {
                var templateSb = new StringBuilder();

                using (var sr = new StreamReader(template))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {

                    }
                }
            }


            return null;
        }

        private string[] GetTemplates()
        {
            var templateDirectory = Path.Combine(Environment.CurrentDirectory, "Template");

            return Directory.GetFiles(templateDirectory, ".cst");
        }
    }
}