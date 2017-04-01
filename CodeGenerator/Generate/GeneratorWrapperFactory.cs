using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CodeGenerator.Generate.GenerateModel;

namespace CodeGenerator.Generate
{
    public class GeneratorWrapperFactory
    {
        private const string CodeTemplateMatch = @"^<%@\s*CodeTemplate(\s+[A-Za-z]+\s*=\s*"".*"")*\s*%>$";
        private const string AssemblyMatch = @"^<%@\s*Assembly\s+Name\s*=\s*"".*""\s*%>$";
        private const string ImportMatch = @"^<%@\s*Import\s+Namespace\s*=\s*"".*""\s*%>$";
        private const string PropertyMatch = @"^<%@\s*Property(\s+[A-Za-z]+\s*=\s*"".*"")*\s*%>$";

        public List<GeneratorWrapper> GetGeneratorWrappers()
        {
            var generatorWrappers = new List<GeneratorWrapper>();
            var templates = GetTemplates();

            foreach (var template in templates)
            {
                var generatorWrapper = new GeneratorWrapper();

                var fileName = Path.GetFileNameWithoutExtension(template);

                using (var sr = new StreamReader(template))
                {
                    string tempLine = string.Empty;
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!string.IsNullOrEmpty(tempLine) || Regex.Matches(line, "<%").Count != Regex.Matches(line, "%>").Count)
                        {
                            tempLine += line;

                            if (Regex.Matches(tempLine, "<%").Count == Regex.Matches(tempLine, "%>").Count)
                            {
                                line = tempLine;
                                tempLine = string.Empty;
                            }
                            else
                                continue;
                        }

                        if (!IsMatchCodeSetting(line, generatorWrapper, fileName))
                        {
                            if (Regex.IsMatch(line, "<%.*%>"))
                            {
                                if (!line.Replace(" ", "").Contains("<%=") && (line.StartsWith("<%") || line.EndsWith("%>")))
                                {
                                    generatorWrapper.CodeBuilder.AppendLine("        " + line.Replace("<%", "").Replace("%>", "").TrimStart());
                                }
                                else
                                {

                                    int number = 0;
                                    string argumentStr = string.Empty;

                                    var list = new List<string>();

                                    if (line.Contains("{") && line.Contains("}"))
                                    {
                                        line = line.Replace("{", "$0$").Replace("}", "{1}").Replace("$0$", "{0}");
                                        number = 2;

                                        argumentStr = ", \"{\", \"}\"";
                                    }

                                    list.AddRange(GetCodeSegment(line));

                                    foreach (var segment in list.Distinct())
                                    {
                                        line = line.Replace(segment, "{" + number + "}");

                                        argumentStr += ", " + segment.Replace("<%=", "").Replace("%>", "");

                                        number++;
                                    }

                                    generatorWrapper.CodeBuilder.AppendLine(string.Format("        builder.AppendLine(string.Format(\"{0}\"{1}));", line.Replace("\"", "\\\""), argumentStr));
                                }
                            }
                            else
                                generatorWrapper.CodeBuilder.AppendLine(string.Format("        builder.AppendLine(\"{0}\");", line.Replace("\"", "\\\"")));
                        }
                    }
                }

                generatorWrapper.TemplateName = fileName;
                generatorWrappers.Add(generatorWrapper);
            }

            return generatorWrappers;
        }

        private bool IsMatchCodeSetting(string text, GeneratorWrapper wrapper, string templateFileName)
        {
            bool isMatch = false;
            if (Regex.IsMatch(text, CodeTemplateMatch))
            {
                var codeTemplate = GetModel<CodeTemplate>(text);
                if (string.IsNullOrEmpty(codeTemplate.Language)) codeTemplate.Language = "text";
                if (string.IsNullOrEmpty(codeTemplate.FileName)) codeTemplate.FileName = templateFileName;

                wrapper.CodeTemplate = codeTemplate;
                isMatch = true;
            }
            else if (Regex.IsMatch(text, AssemblyMatch))
            {
                var assembly = GetModel<Assembly>(text);
                if (string.IsNullOrEmpty(assembly.Extension))
                    assembly.Extension = ".dll";

                if (!wrapper.Assemblies.Select(i => i.Name).Contains(assembly.Name))
                    wrapper.Assemblies.Add(assembly);

                isMatch = true;
            }
            else if (Regex.IsMatch(text, ImportMatch))
            {
                var import = GetModel<Import>(text);
                if (!wrapper.Imports.Select(i => i.Namespace).Contains(import.Namespace))
                    wrapper.Imports.Add(import);

                isMatch = true;
            }
            else if (Regex.IsMatch(text, PropertyMatch))
            {
                var property = GetModel<Property>(text);
                if (string.IsNullOrEmpty(property.Type))
                    property.Type = "String";

                if (!string.IsNullOrEmpty(property.Name) &&
                    !wrapper.Properties.Select(i => i.Name).Contains(property.Name))
                    wrapper.Properties.Add(property);

                isMatch = true;
            }

            return isMatch;
        }

        private T GetModel<T>(string text) where T : class ,new()
        {
            T t = new T();
            text = Regex.Replace(text, "<%@\\s*", "<").Replace("%>", "/>");

            var doc = XDocument.Parse(text);
            if (doc.Root == null) return t;
            var type = typeof(T);
            var properties = type.GetProperties().Where(p => p.CanWrite);
            foreach (var property in properties)
            {
                var attribute = doc.Root.Attribute(property.Name) ??
                                new XAttribute(XName.Get(property.Name), string.Empty);
                if (!string.IsNullOrEmpty(attribute.Value))
                {
                    property.SetValue(t, attribute.Value.Trim());
                }

            }

            return t;
        }

        private string[] GetTemplates()
        {
            var templateDirectory = Path.Combine(Environment.CurrentDirectory, "Template");

            return Directory.GetFiles(templateDirectory, "*.cst");
        }

        private List<string> GetCodeSegment(string data)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(data)) return list;
            var headerIndex = data.IndexOf("<%=", StringComparison.Ordinal);
            var footIndex = data.IndexOf("%>", StringComparison.Ordinal);
            if (headerIndex >= 0 && footIndex >= 0)
            {
                var subStr = data.Substring(headerIndex, footIndex - headerIndex + 2);
                list.Add(subStr);
                list.AddRange(GetCodeSegment(data.Substring(footIndex + 2)));
            }

            return list;
        }
    }
}