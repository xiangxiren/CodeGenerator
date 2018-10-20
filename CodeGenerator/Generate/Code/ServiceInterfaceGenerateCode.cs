using System.IO;
using System.Text;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate.Code
{
	[CodeGenerator(GenerateType.Interface)]
	public class ServiceInterfaceGenerateCode : GenerateCodeBase, IGenerateCode
	{
		protected override string FileNameTemplate => "I{0}Service.cs";

		public void Generate(TableInfo table, GenerateArgument argument)
		{
			using (var fs = new FileStream(GetFullFilePath(table.TableName, GenerateArgument.InterfaceFilePath, argument.FilePath), FileMode.Create))
			using (var sw = new StreamWriter(fs, Encoding.UTF8))
			{
				#region using

				sw.WriteLine("using System.Collections.Generic;");
				sw.WriteLine("using {0}.Infrastructure;", argument.ProjectName);
				sw.WriteLine("using {0}.Service.Dto;", argument.ProjectName);
				sw.WriteLine("using {0}.Service.QueryParam;", argument.ProjectName);

				sw.WriteLine();

				#endregion

				sw.WriteLine("namespace {0}.{1}", argument.ProjectName, GenerateArgument.InterfaceNamespaceTemp);
				sw.WriteLine("{");
				sw.WriteLine("    /// <summary>");
				sw.WriteLine("    /// {0}服务接口", table.Comment);
				sw.WriteLine("    /// </summary>");
				sw.WriteLine("    public interface I{0}Service", table.TableName);
				sw.WriteLine("    {");
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// 新增{0}", table.Comment);
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        /// <param name=\"model\"></param>");
				sw.WriteLine("        void Add{0}({0}Dto model);", table.TableName);
				sw.WriteLine();
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// 修改{0}", table.Comment);
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        /// <param name=\"model\"></param>");
				sw.WriteLine("        void Update{0}({0}Dto model);", table.TableName);
				sw.WriteLine();
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// 获取{0}", table.Comment);
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        /// <param name=\"id\"></param>");
				sw.WriteLine("        {0}Dto Get{0}(int id);", table.TableName);
				sw.WriteLine();
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// 删除{0}", table.Comment);
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        /// <param name=\"id\"></param>");
				sw.WriteLine("        void Delete{0}(int id);", table.TableName);
				sw.WriteLine();
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// 获取{0}分页列表", table.Comment);
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        /// <param name=\"paramter\"></param>");
				sw.WriteLine("        PageList<{0}Dto> Get{0}PageList(QueryParamterBase<{0}QueryParam> paramter);", table.TableName);
				sw.WriteLine("    }");
				sw.Write("}");
				sw.Flush();
			}
		}
	}
}