using System.IO;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate
{
	public class BlGenerateCode : GenerateCodeBase, IGenerateCode
	{
		protected override string FileName => "BL.cs";

		public void Generate(TableInfo table, ArgumentInfo argumentInfo)
		{
			using (var fs = new FileStream(GetFullFilePath(table.TableName, argumentInfo.FileSavePath), FileMode.Create))
			using (var sw = new StreamWriter(fs))
			{
				#region using

				sw.WriteLine("using JG.Core;");
				sw.WriteLine("using Scm.Component.Common;");
				sw.WriteLine("using Scm.Component.SecurityModel;");
				sw.WriteLine();

				#endregion

				sw.WriteLine("namespace {0}", argumentInfo.ClassNamespace);
				sw.WriteLine("{");
				sw.WriteLine("    /// <summary>");
				sw.WriteLine("    /// {0}BL", table.Comment);
				sw.WriteLine("    /// </summary>");
				sw.WriteLine("    public class {0}BL", table.TableName);
				sw.WriteLine("    {");
				sw.WriteLine("        private readonly I{0}Repository m{0}Repository;", table.TableName);
				sw.WriteLine();
				sw.WriteLine("        #region 构造函数");
				sw.WriteLine();
				sw.WriteLine("        public {0}BL()", table.TableName);
				sw.WriteLine("            : this(new {0}Repository())", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine();
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        public {0}BL(I{0}Repository repository)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            this.m{0}Repository = repository ?? new {0}Repository();", table.TableName);
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        #endregion");
				sw.WriteLine();
				sw.WriteLine("        #region 业务逻辑");
				sw.WriteLine();
				sw.WriteLine("        public {0}Entity Get(object ID, bool isCopy = false)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            var entity = this.m{0}Repository.Get(ID);", table.TableName);
				sw.WriteLine("            return entity != null && isCopy ? DataProcess.CloneObject(entity) : entity;");
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        public PageDataSet<{0}Entity> GetList(QueryModel queryModel, int pageSize, int pageIndex)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            return this.m{0}Repository.GetList(queryModel, pageSize, pageIndex);", table.TableName);
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        public {0}Entity Update({0}Entity entity)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            entity.UpdateUserID = UserContext.CurrentUser.UserID;");
				sw.WriteLine("            entity.UpdateDate = DateTimeUtil.GetNowDateTime();");
				sw.WriteLine("            this.m{0}Repository.Update(entity);", table.TableName);
				sw.WriteLine("            return entity;");
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        #endregion");
				sw.WriteLine("    }");
				sw.Write("}");
				sw.Flush();
			}
		}
	}
}