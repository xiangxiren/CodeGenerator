using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;

namespace CodeGenerator.Generate
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class DynClassUtils
    {
        private static readonly string[] BaseAssemblies = { "System.dll", "System.Core.dll", "System.Data.dll", "System.Xml.dll" };

        /// <summary>
        /// 添加需要引用的程序集
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="code"></param>
        /// <param name="assemblyPath"></param>
        private static void AddReferencedAssemblies(CompilerParameters parameters, string code, string assemblyPath)
        {
            if (!string.IsNullOrEmpty(code))
            {
                int startIndex = 0;
                int num;
                while ((num = code.IndexOf('\n', startIndex)) >= 0)
                {
                    string codeSnippet = code.Substring(startIndex, num - startIndex).Trim();
                    startIndex = num + 1;
                    if (codeSnippet.StartsWith("using"))
                    {
                        return;
                    }
                    if (codeSnippet.StartsWith("//"))
                    {
                        codeSnippet = codeSnippet.Substring(2).TrimStart();
                        if (codeSnippet.StartsWith("import", StringComparison.CurrentCultureIgnoreCase))
                        {
                            codeSnippet = codeSnippet.Substring(6).TrimStart();
                            if (!Path.IsPathRooted(codeSnippet) && !codeSnippet.StartsWith("System", StringComparison.CurrentCultureIgnoreCase))
                            {
                                codeSnippet = Path.Combine(assemblyPath, codeSnippet);
                            }
                            parameters.ReferencedAssemblies.Add(codeSnippet);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取对象实例
        /// </summary>
        /// <param name="codeSnippet">代码段</param>
        /// <param name="className">类名</param>
        /// <param name="assemblyName">输出程序集名称</param>
        /// <param name="assemblyNames">需要引用的程序集名</param>
        /// <returns></returns>
        public static object GetDynClassObject(string codeSnippet, string className, string assemblyName, IEnumerable<string> assemblyNames)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            string directoryName = Path.GetDirectoryName(typeof(DynClassUtils).Assembly.CodeBase);
            if (string.IsNullOrEmpty(directoryName)) throw new Exception("无法获取路径");
            string assemblyPath = directoryName.Substring(6);

            CompilerParameters parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };
            if (!string.IsNullOrEmpty(assemblyName))
            {
                parameters.OutputAssembly = assemblyName;
            }

            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Data.dll");
            parameters.ReferencedAssemblies.Add("System.Xml.dll");

            foreach (var name in assemblyNames.Where(i => !BaseAssemblies.Contains(i)))
            {
                parameters.ReferencedAssemblies.Add(Path.Combine(assemblyPath, name));
            }
            AddReferencedAssemblies(parameters, codeSnippet, assemblyPath);

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, codeSnippet);
            if (results.Errors.Count != 0)
            {
                throw new Exception(results.Errors[0].ErrorText);
            }

            return results.CompiledAssembly.CreateInstance(className, false, BindingFlags.Default, null, null, CultureInfo.CurrentCulture, null);
        }
    }
}