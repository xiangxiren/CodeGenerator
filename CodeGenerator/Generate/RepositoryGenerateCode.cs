using System.IO;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate
{
	public class RepositoryGenerateCode : GenerateCodeBase, IGenerateCode
	{
		protected override string FileName => "Repository.cs";

		public void Generate(TableInfo table, ArgumentInfo argumentInfo)
		{
			using (var fs = new FileStream(GetFullFilePath(table.TableName, argumentInfo.FileSavePath), FileMode.Create))
			using (var sw = new StreamWriter(fs))
			{
				#region using

				sw.WriteLine("using System.Text;");
				sw.WriteLine("using JG.Core;");
				sw.WriteLine("using JG.Core.Repository;");
				sw.WriteLine("using JG.Core.Cache;");
				sw.WriteLine("using Scm.Component.Common;");
				sw.WriteLine("using Scm.Component.SecurityModel;");
				sw.WriteLine();

				#endregion

				sw.WriteLine("namespace {0}", argumentInfo.ClassNamespace);
				sw.WriteLine("{");
				sw.WriteLine("    /// <summary>");
				sw.WriteLine("    /// {0}Repository接口", table.Comment);
				sw.WriteLine("    /// </summary>");
				sw.WriteLine("    public interface I{0}Repository : IRepository<{0}Entity>", table.TableName);
				sw.WriteLine("    {");
				sw.WriteLine("        PageDataSet<{0}Entity> GetList(QueryModel queryModel, int pageSize, int pageIndex);", table.TableName);
				sw.WriteLine("    }");
				sw.WriteLine();
				sw.WriteLine("    /// <summary>");
				sw.WriteLine("    /// {0}Repository实现", table.Comment);
				sw.WriteLine("    /// </summary>");
				sw.WriteLine("    public class {0}Repository : ExtRepository<{0}Entity>, I{0}Repository", table.TableName);
				sw.WriteLine("    {");
				sw.WriteLine("        public PageDataSet<{0}Entity> GetList(QueryModel queryModel, int pageSize, int pageIndex)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            return this.GetPageEntities(pageSize, pageIndex, CachingExpirationType.ObjectCollection, () =>");
				sw.WriteLine("            {");
				sw.WriteLine("                return");
				sw.WriteLine("                    new StringBuilder().Append(CacheHelper.GetListCacheKeyPrefix(CacheVersionType.AreaVersion,");
				sw.WriteLine("                        \"ClientID\",");
				sw.WriteLine("                        UserContext.CurrentUser.ClientID))");
				sw.WriteLine("                        .Append(\"GetList\")");
				sw.WriteLine("                        .Append(queryModel.GetConditionCacheKey()).ToString();");
				sw.WriteLine("            },");
				sw.WriteLine("                () =>");
				sw.WriteLine("                {");
				sw.WriteLine("                    return queryModel.GetConditionSql().Append(queryModel.GetOrderSql());");
				sw.WriteLine("                });");
				sw.WriteLine("        }");
				sw.WriteLine("    }");
				sw.Write("}");
				sw.Flush();
			}
		}
	}
}