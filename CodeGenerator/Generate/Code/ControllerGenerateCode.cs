using System.IO;
using System.Text;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate.Code
{
	public class ControllerGenerateCode : GenerateCodeBase, IGenerateCode
	{
		protected override string FileNameTemplate => "{0}Controller.cs";

		public GenerateType GenerateType { get; set; } = GenerateType.Controller;

		public void Generate(TableInfo table, GenerateArgument argument)
		{
			using (var fs =
				new FileStream(
					GetFullFilePath(table.TableName, GenerateArgument.ControllerFilePath, argument.FilePath),
					FileMode.Create))
			using (var sw = new StreamWriter(fs, Encoding.UTF8))
			{
				#region using

				sw.WriteLine("using {0}.Infrastructure;", argument.ProjectName);
				sw.WriteLine("using {0}.Service.Dto;", argument.ProjectName);
				sw.WriteLine("using {0}.Service.Interface;", argument.ProjectName);
				sw.WriteLine("using {0}.Service.QueryParam;", argument.ProjectName);
				sw.WriteLine("using {0}.Web.Filter;", argument.ProjectName);
				sw.WriteLine("using Microsoft.AspNetCore.Mvc;");
				sw.WriteLine("using Microsoft.Extensions.Logging;");

				sw.WriteLine();

				#endregion

				sw.WriteLine("namespace {0}.{1}", argument.ProjectName, GenerateArgument.ControllerNamespaceTemp);
				sw.WriteLine("{");
				sw.WriteLine("    public class {0}Controller : BaseController<{0}Controller>", table.TableName);
				sw.WriteLine("    {");
				sw.WriteLine("        private readonly I{0}Service _{1}Service;", table.TableName, GetCamelVarName(table.TableName));
				sw.WriteLine();
				sw.WriteLine("        public {0}Controller(ILogger<{0}Controller> logger,", table.TableName);
				sw.WriteLine("            ILoginContext loginContext,");
				sw.WriteLine("            I{0}Service {1}Service)", table.TableName, GetCamelVarName(table.TableName));
				sw.WriteLine("            : base(logger, loginContext)");
				sw.WriteLine("        {");
				sw.WriteLine("            _{0}Service = {0}Service;", GetCamelVarName(table.TableName));
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// {0}列表", table.Comment);
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        /// <returns></returns>");
				sw.WriteLine("        [Permission(\"Query{0}\")]", table.TableName);
				sw.WriteLine("        public IActionResult Index()");
				sw.WriteLine("        {");
				sw.WriteLine("            ViewBag.AllowAdd = CheckPower(\"Add{0}\");", table.TableName);
				sw.WriteLine("            ViewBag.AllowEdit = CheckPower(\"Edit{0}\");", table.TableName);
				sw.WriteLine("            ViewBag.AllowDelete = CheckPower(\"Delete{0}\");", table.TableName);
				sw.WriteLine();
				sw.WriteLine("            return PartialView();");
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// 获取{0}分页集合", table.Comment);
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        /// <param name=\"paramter\"></param>");
				sw.WriteLine("        /// <returns></returns>");
				sw.WriteLine("        [Permission(\"Query{0}\")]", table.TableName);
				sw.WriteLine("        public IActionResult Get{0}s([FromBody]QueryParamterBase<{0}QueryParam> paramter)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            return ActionHandler(() => _{0}Service.Get{1}PageList(paramter));", GetCamelVarName(table.TableName), table.TableName);
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// 新增/修改{0}", table.Comment);
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        /// <param name=\"id\">id为null时表示新增</param>");
				sw.WriteLine("        /// <returns></returns>");
				sw.WriteLine("        [Permission(\"Add{0},Edit{0}\")]", table.TableName);
				sw.WriteLine("        public IActionResult Edit{0}(int? id)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            return PartialView(id);");
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// 获取{0}", table.Comment);
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        /// <param name=\"id\"></param>");
				sw.WriteLine("        /// <returns></returns>");
				sw.WriteLine("        [Permission(\"Edit{0}\")]", table.TableName);
				sw.WriteLine("        public IActionResult Get{0}(int id)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            return ActionHandler(() => _{0}Service.Get{1}(id));", GetCamelVarName(table.TableName), table.TableName);
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// 新增{0}", table.Comment);
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        /// <param name=\"model\"></param>");
				sw.WriteLine("        /// <returns></returns>");
				sw.WriteLine("        [Permission(\"Add{0}\")]", table.TableName);
				sw.WriteLine("        public IActionResult Add{0}({0}Dto model)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            return ActionHandler(() => _{0}Service.Add{1}(model));", GetCamelVarName(table.TableName), table.TableName);
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// 修改{0}", table.Comment);
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        /// <param name=\"model\"></param>");
				sw.WriteLine("        /// <returns></returns>");
				sw.WriteLine("        [Permission(\"Edit{0}\")]", table.TableName);
				sw.WriteLine("        public IActionResult Update{0}({0}Dto model)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            return ActionHandler(() => _{0}Service.Update{1}(model));", GetCamelVarName(table.TableName), table.TableName);
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        /// <summary>");
				sw.WriteLine("        /// 删除{0}", table.Comment);
				sw.WriteLine("        /// </summary>");
				sw.WriteLine("        /// <returns></returns>");
				sw.WriteLine("        [Permission(\"Delete{0}\")]", table.TableName);
				sw.WriteLine("        public IActionResult Delete{0}(int id)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            return ActionHandler(() => _{0}Service.Delete{1}(id));", GetCamelVarName(table.TableName), table.TableName);
				sw.WriteLine("        }");
				sw.WriteLine("    }");
				sw.Write("}");
				sw.Flush();
			}
		}
	}
}