using System.IO;
using System.Text;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate.Code
{
	public class QueryParamGenerateCode : GenerateCodeBase, IGenerateCode
	{
		protected override string FileNameTemplate => "{0}QueryParam.cs";

		public GenerateType GenerateType { get; set; } = GenerateType.QueryParam;

		public void Generate(TableInfo table, GenerateArgument argument)
		{
			using (var fs = new FileStream(GetFullFilePath(table.TableName, GenerateArgument.QueryParamFilePath, argument.FilePath), FileMode.Create))
			using (var sw = new StreamWriter(fs, Encoding.UTF8))
			{
				#region using

				sw.WriteLine("using System.Linq;");
				sw.WriteLine("using {0}.Data.Entity;", argument.ProjectName);
				sw.WriteLine();

				#endregion

				sw.WriteLine("namespace {0}.{1}", argument.ProjectName, GenerateArgument.QueryParamNamespaceTemp);
				sw.WriteLine("{");
				sw.WriteLine("    /// <summary>");
				sw.WriteLine("    /// {0}查询参数", table.Comment);
				sw.WriteLine("    /// </summary>");
				sw.WriteLine("    public class {0}QueryParam", table.TableName);
				sw.WriteLine("    {");
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// {0}查询参数", table.Comment);
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        public string Filter { get; set; }");
				sw.WriteLine("    }");
				sw.WriteLine();
				sw.WriteLine("    /// <summary>");
				sw.WriteLine("    /// {0}查询扩展", table.Comment);
				sw.WriteLine("    /// </summary>");
				sw.WriteLine("    public static class {0}QueryExtensions", table.TableName);
				sw.WriteLine("    {");
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// 查询扩展");
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        /// <param name=\"source\"></param>");
				sw.WriteLine("        /// <param name=\"param\"></param>");
				sw.WriteLine("        /// <returns></returns>");
				sw.WriteLine("        public static IQueryable<{0}> Where(this IQueryable<{0}> source, {0}QueryParam param)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            if (param == null) return source;");
				sw.WriteLine("            return source;");
				sw.WriteLine("        }");
				sw.WriteLine("    }");
				sw.Write("}");
				sw.Flush();
			}
		}
	}
}