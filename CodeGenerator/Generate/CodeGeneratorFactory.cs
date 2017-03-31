using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeGenerator.Generate
{
    public sealed class CodeGeneratorFactory
    {
        private static readonly string BaseTemplate;

        public static Dictionary<string, ICodeGenerator> CodeGenerators { get; set; }

        static CodeGeneratorFactory()
        {
            BaseTemplate = ReadBaseTemplate();
        }

        public List<ICodeGenerator> GetTemplateGenerateCodes()
        {
            var codeGenerators = new List<ICodeGenerator>();

            var generatorWrappers = new GeneratorWrapperFactory().GetGeneratorWrappers();

            foreach (var wrapper in generatorWrappers)
            {
                var usingStr = wrapper.Imports.Aggregate(string.Empty,
                    (current, import) => current + "using " + import.Namespace + ";\r\n");

                var generatorCode = BaseTemplate
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

        private static List<ICodeGenerator> GetTemplateGenerateCodes(List<string> templateNames, dynamic customProperty)
        {
            var codeGenerators = new List<ICodeGenerator>();

            var generatorWrappers = new GeneratorWrapperFactory().GetGeneratorWrappers();

            foreach (var wrapper in generatorWrappers.Where(w => templateNames.Contains(w.TemplateName)))
            {
                var usingStr = wrapper.Imports.Aggregate(string.Empty,
                    (current, import) => current + "using " + import.Namespace + ";\r\n");

                var generatorCode = BaseTemplate
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

        private static string ReadBaseTemplate()
        {
            string template;

            var templateDirectory = Path.Combine(Environment.CurrentDirectory, "Template\\BaseCodeGeneratorTemplate.txt");
            using (var sr = new StreamReader(templateDirectory))
            {
                template = sr.ReadToEnd();
            }

            return template;
        }

        /// <summary>
        /// 当模板改变或者自定义属性值改变时重新执行编译模板
        /// </summary>
        /// <param name="templateNames">需要重新编译的模板名集合</param>
        /// <param name="customProperty">自定义参数对象</param>
        /// <returns>所有模板编译后并各自实例化一个对象，返回对象集合</returns>
        public static Dictionary<string, ICodeGenerator> ReExecuteCompile(List<string> templateNames, dynamic customProperty)
        {

            return CodeGenerators;
        }

        /// <summary>
        /// 当模板改变或者自定义属性值改变时重新执行编译模板
        /// </summary>
        /// <param name="templateName">需要重新编译的模板名</param>
        /// <param name="customProperty">自定义参数对象</param>
        /// <returns>返回指定模板编译后的一个实例对象</returns>
        public static ICodeGenerator ReExecuteCompile(string templateName, dynamic customProperty)
        {
            ReExecuteCompile(new List<string> { templateName }, customProperty);

            return CodeGenerators[templateName];
        }
    }
}