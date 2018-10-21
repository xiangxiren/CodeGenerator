using System.IO;
using System.Linq;
using System.Text;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate.Code
{
	public class ServiceImplementGenerateCode : GenerateCodeBase, IGenerateCode
	{
		protected override string FileNameTemplate => "{0}Service.cs";

		public GenerateType GenerateType { get; set; } = GenerateType.Implement;

		public void Generate(TableInfo table, GenerateArgument argument)
		{
			using (var fs =
				new FileStream(
					GetFullFilePath(table.TableName, GenerateArgument.ImplementFilePath, argument.FilePath),
					FileMode.Create))
			using (var sw = new StreamWriter(fs, Encoding.UTF8))
			{
				#region using

				sw.WriteLine("using System;");
				sw.WriteLine("using System.Collections.Generic;");
				sw.WriteLine("using System.Linq;");
				sw.WriteLine("using {0}.Data;", argument.ProjectName);
				sw.WriteLine("using {0}.Data.Entity;", argument.ProjectName);
				sw.WriteLine("using {0}.Data.Enums;", argument.ProjectName);
				sw.WriteLine("using {0}.Infrastructure;", argument.ProjectName);
				sw.WriteLine("using {0}.Infrastructure.Cache;", argument.ProjectName);
				sw.WriteLine("using {0}.Service.Dto;", argument.ProjectName);
				sw.WriteLine("using {0}.Service.Interface;", argument.ProjectName);
				sw.WriteLine("using {0}.Service.QueryParam;", argument.ProjectName);
				sw.WriteLine("using Microsoft.EntityFrameworkCore;");

				sw.WriteLine();

				#endregion

				sw.WriteLine("namespace {0}.{1}", argument.ProjectName, GenerateArgument.ImplementNamespaceTemp);
				sw.WriteLine("{");
				sw.WriteLine("    /// <inheritdoc />");
				sw.WriteLine("    public class {0}Service : I{0}Service", table.TableName);
				sw.WriteLine("    {");
				sw.WriteLine("        private readonly BhDbContext _dbContext;");
				sw.WriteLine();
				sw.WriteLine("        /// <inheritdoc />");
				sw.WriteLine("        public {0}Service(BhDbContext dbContext)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            _dbContext = dbContext;");
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        /// <inheritdoc />");
				sw.WriteLine("        public void Add{0}({0}Dto model)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            _dbContext.Set<{0}>().Add(new {0}", table.TableName);
				sw.WriteLine("            {");

				var index = 1;
				var columnsNoPrimaryKey = table.ColumnInfos.Where(t => t.Code != table.PrimaryKeyCode).ToList();

				foreach (var columnInfo in columnsNoPrimaryKey)
				{
					sw.WriteLine("                {0} = model.{0}{1}", columnInfo.Code, index == columnsNoPrimaryKey.Count ? "" : ",");

					index++;
				}

				sw.WriteLine("            });");
				sw.WriteLine();
				sw.WriteLine("            _dbContext.SaveChanges();");
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        /// <inheritdoc />");
				sw.WriteLine("        public void Update{0}({0}Dto model)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            var {0} = _dbContext.Set<{1}>().Find(model.Id) ??",
					GetCamelVarName(table.TableName), table.TableName);
				sw.WriteLine("            		   throw new BusinessException($\"未能找到id为{0} model.Id }}的{1}信息\");", "{",
					table.Comment);
				sw.WriteLine();

				foreach (var columnInfo in columnsNoPrimaryKey)
				{
					sw.WriteLine("            {0}.{1} = model.{1};", GetCamelVarName(table.TableName), columnInfo.Code);
				}

				sw.WriteLine();
				sw.WriteLine("            _dbContext.SaveChanges();");
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        /// <inheritdoc />");
				sw.WriteLine("        public {0}Dto Get{0}(int id)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            var {0} = _dbContext.Set<{1}>().Find(id) ??",
					GetCamelVarName(table.TableName), table.TableName);
				sw.WriteLine("            		   throw new BusinessException($\"未能找到id为{0} id }}的{1}信息\");", "{",
					table.Comment);
				sw.WriteLine();
				sw.WriteLine("            return new {0}Dto", table.TableName);
				sw.WriteLine("            {");

				index = 1;
				foreach (var columnInfo in table.ColumnInfos)
				{
					sw.WriteLine("                {0} = {1}.{0}{2}", columnInfo.Code, GetCamelVarName(table.TableName), index == table.ColumnInfos.Count ? "" : ",");

					index++;
				}

				sw.WriteLine("            };");
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        /// <inheritdoc />");
				sw.WriteLine("        public void Delete{0}(int id)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            var {0} = _dbContext.Set<{1}>().Find(id) ??",
					GetCamelVarName(table.TableName), table.TableName);
				sw.WriteLine("            		   throw new BusinessException($\"未能找到id为{0} id }}的{1}信息\");", "{",
					table.Comment);
				sw.WriteLine();
				sw.WriteLine("            _dbContext.Set<{0}>().Remove({1});", table.TableName,
					GetCamelVarName(table.TableName));
				sw.WriteLine();
				sw.WriteLine("            _dbContext.SaveChanges();");
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        /// <inheritdoc />");
				sw.WriteLine(
					"        public PageList<{0}Dto> Get{0}PageList(QueryParamterBase<{0}QueryParam> paramter)",
					table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            return _dbContext.Set<{0}>()", table.TableName);
				sw.WriteLine("                .AsNoTracking()");
				sw.WriteLine("                .Where(paramter.ExtendParam)");
				sw.WriteLine("                .Select(t => new {0}Dto", table.TableName);
				sw.WriteLine("                {");

				index = 1;
				foreach (var columnInfo in table.ColumnInfos)
				{
					sw.WriteLine("                    {0} = t.{0}{1}", columnInfo.Code, index == table.ColumnInfos.Count ? "" : ",");

					index++;
				}

				sw.WriteLine("                })");
				sw.WriteLine("                .PageList(paramter);");
				sw.WriteLine("        }");
				sw.WriteLine("    }");
				sw.Write("}");
				sw.Flush();
			}
		}
	}
}