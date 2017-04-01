using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CodeGenerator.Generate.GenerateModel;
using Codeplex.Data;

namespace CodeGenerator.Generate
{
    public sealed class CodeGeneratorFactory
    {
        private static readonly string BaseTemplate;

        private static readonly Dictionary<string, ICodeGenerator> CodeGenerators;

        private static readonly GeneratorWrapperFactory GeneratorWrapperFactory;

        static CodeGeneratorFactory()
        {
            BaseTemplate = ReadBaseTemplate();

            CodeGenerators = new Dictionary<string, ICodeGenerator>();

            GeneratorWrapperFactory = new GeneratorWrapperFactory();
        }

        private static void GetTemplateGenerateCodes(List<string> templateNames, string customProperty)
        {
            var generatorWrappers = GeneratorWrapperFactory.GetGeneratorWrappers().Where(w => templateNames.Contains(w.TemplateName));

            GetTemplateGenerateCodes(generatorWrappers, customProperty);
        }

        private static void GetTemplateGenerateCodes(IEnumerable<GeneratorWrapper> generatorWrappers, string customProperty)
        {
            foreach (var wrapper in generatorWrappers)
            {
                var usingStr = wrapper.Imports.Aggregate(string.Empty,
                    (current, import) => current + "using " + import.Namespace + ";\r\n");

                var generatorCode = BaseTemplate
                    .Replace("$Using$", usingStr)
                    .Replace("$FileName$", wrapper.CodeTemplate.FileName)
                    .Replace("$Properties$", GetCustomProperty(wrapper, customProperty))
                    .Replace("$ChildTemplate$", wrapper.CodeBuilder.ToString());

                var codeGenerator = (ICodeGenerator)DynClassUtils.GetDynClassObject(generatorCode,
                    wrapper.CodeTemplate.FileName + "GenerateCode",
                    string.Empty, wrapper.Assemblies.Select(i => i.Name + i.Extension));

                if (codeGenerator == null) continue;

                if (CodeGenerators.ContainsKey(wrapper.TemplateName))
                    CodeGenerators[wrapper.TemplateName] = codeGenerator;
                else
                    CodeGenerators.Add(wrapper.TemplateName, codeGenerator);
            }
        }

        private static string GetCustomProperty(GeneratorWrapper wrapper, string customProperty)
        {
            var propertyBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(customProperty)) return propertyBuilder.ToString();

            var propertyDynamic = DynamicJson.Parse(customProperty);

            foreach (var property in wrapper.Properties)
            {
                if (!propertyDynamic.IsDefined(property.Name) && string.IsNullOrEmpty(property.Default))
                    throw new Exception(string.Format("属性{0}未赋值",
                        string.IsNullOrEmpty(property.Description) ? property.Name : property.Description));

                propertyBuilder.AppendLine(string.Format("        var {0} = {2}{1}{2};", property.Name,
                    propertyDynamic[property.Name], property.Type.ToLower() == "string" ? "\"" : ""));
            }

            return propertyBuilder.ToString();
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
        /// 获取所有生成器
        /// </summary>
        /// <param name="customProperty">自定义参数对象</param>
        /// <returns>所有模板编译后并各自实例化一个对象，返回对象集合</returns>
        public static Dictionary<string, ICodeGenerator> GetCodeGenerators(string customProperty)
        {
            GetTemplateGenerateCodes(GeneratorWrapperFactory.GetGeneratorWrappers(), customProperty);

            return CodeGenerators;
        }

        /// <summary>
        /// 当模板改变或者自定义属性值改变时重新执行编译模板
        /// </summary>
        /// <param name="templateNames">需要重新编译的模板名集合</param>
        /// <param name="customProperty">自定义参数,json格式字符串例如：{"Name":"John", "City":"Chengdu"}</param>
        /// <returns>所有模板编译后并各自实例化一个对象，返回对象集合</returns>
        public static Dictionary<string, ICodeGenerator> ReExecuteCompile(List<string> templateNames, string customProperty)
        {
            GetTemplateGenerateCodes(templateNames, customProperty);

            return CodeGenerators;
        }

        /// <summary>
        /// 当模板改变或者自定义属性值改变时重新执行编译模板
        /// </summary>
        /// <param name="templateName">需要重新编译的模板名</param>
        /// <param name="customProperty">自定义参数,json格式字符串例如：{"Name":"John", "City":"Chengdu"}</param>
        /// <returns>返回指定模板编译后的一个实例对象</returns>
        public static ICodeGenerator ReExecuteCompile(string templateName, string customProperty)
        {
            ReExecuteCompile(new List<string> { templateName }, customProperty);

            return CodeGenerators[templateName];
        }
    }
}