using System.IO;
using System.Text;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate
{
	public class RepositoryGenerateCode : GenerateCodeBase, IGenerateCode
	{
		protected override string FileName => "Repository.cs";

		public void Generate(TableInfo table, ArgumentInfo argumentInfo)
		{
			using (var fs = new FileStream(GetFullFilePath(table.TableName, argumentInfo.FileSavePath), FileMode.Create))
			using (var sw = new StreamWriter(fs, Encoding.UTF8))
			{
				#region using

				sw.WriteLine("using Hxf.USORST.Infrastructure.Data;");
				sw.WriteLine("using {0};", argumentInfo.GenerateArgument.EntityNamespace);
				sw.WriteLine();

				#endregion

				sw.WriteLine("namespace {0}", argumentInfo.ClassNamespace);
				sw.WriteLine("{");
				sw.WriteLine("    /// <summary>");
				sw.WriteLine("    /// {0}Repository", table.Comment);
				sw.WriteLine("    /// </summary>");
				sw.WriteLine("    public class {0}Repository : ReadonlyRepository<{0}>", table.TableName);
				sw.WriteLine("    {");
				sw.WriteLine("        public {0}Repository(IEntityframeworkReadonlyContext unitWork)", table.TableName);
				sw.WriteLine("			: base(unitWork)");
				sw.WriteLine("        {");
				sw.WriteLine("        }");
				sw.WriteLine("    }");
				sw.Write("}");
				sw.Flush();
			}
		}
	}
}