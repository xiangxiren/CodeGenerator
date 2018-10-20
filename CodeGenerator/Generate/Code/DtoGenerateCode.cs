using System.IO;
using System.Text;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate.Code
{
	[CodeGenerator(GenerateType.Dto)]
	public class DtoGenerateCode : GenerateCodeBase, IGenerateCode
	{
		protected override string FileNameTemplate => "{0}Dto.cs";

		public void Generate(TableInfo table, GenerateArgument argument)
		{
			using (var fs = new FileStream(GetFullFilePath(table.TableName, GenerateArgument.DtoFilePath, argument.FilePath), FileMode.Create))
			using (var sw = new StreamWriter(fs, Encoding.UTF8))
			{
				#region using

				sw.WriteLine("using System;");
				if (table.ChildTableInfos != null && table.ChildTableInfos.Count > 0)
					sw.WriteLine("using System.Collections.Generic;");
				sw.WriteLine();

				#endregion

				sw.WriteLine("namespace {0}.{1}", argument.ProjectName, GenerateArgument.DtoNamespaceTemp);
				sw.WriteLine("{");
				sw.WriteLine("    /// <summary>");
				sw.WriteLine("    /// {0}Dto", table.Comment);
				sw.WriteLine("    /// </summary>");

				sw.WriteLine("    public class {0}Dto", table.TableName);
				sw.WriteLine("    {");

				#region 属性

				var flag = false;
				foreach (var columnInfo in table.ColumnInfos)
				{
					if (flag)
						sw.WriteLine();
					sw.WriteLine("        /// <summary>");
					sw.WriteLine("        /// {0}", string.IsNullOrEmpty(columnInfo.Comment) && columnInfo.Code == table.PrimaryKeyCode ? "主键" : columnInfo.Comment);
					sw.WriteLine("        /// </summary>");

					sw.WriteLine("        public {0} {1} {2} get; set; {3}", columnInfo.GetColumnType(), columnInfo.Code, "{", "}");

					if (!flag)
						flag = true;
				}

				#endregion

				sw.WriteLine("    }");
				sw.Write("}");
				sw.Flush();
			}
		}
	}
}