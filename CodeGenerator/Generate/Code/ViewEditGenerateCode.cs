using System.IO;
using System.Linq;
using System.Text;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate.Code
{
	public class ViewEditGenerateCode : GenerateCodeBase, IGenerateCode
	{
		protected override string FileNameTemplate => "Edit{0}.cshtml";

		public GenerateType GenerateType { get; set; } = GenerateType.ViewEdit;

		protected override string GetFullFilePath(string formatTableName, string midllePath, string fileSavePath)
		{
			var folderPath = Path.Combine(fileSavePath, midllePath, formatTableName);

			EnsurePathExist(folderPath);
			return Path.Combine(folderPath, string.Format(FileNameTemplate, formatTableName));
		}

		public void Generate(TableInfo table, GenerateArgument argument)
		{
			var columnInfos =
				table.ColumnInfos.Where(t =>
					t.Code != table.PrimaryKeyCode && t.Code.ToUpper() != "CREATETIME" &&
					t.Code.ToUpper() != "MODIFYTIME").ToList();

			using (var fs = new FileStream(GetFullFilePath(table.TableName, GenerateArgument.ViewFilePath, argument.FilePath), FileMode.Create))
			using (var sw = new StreamWriter(fs, Encoding.UTF8))
			{
				#region html

				sw.WriteLine("@model int?");
				sw.WriteLine();
				sw.WriteLine("<form method=\"get\" class=\"form-horizontal\" id=\"@ViewBag.FormId\" data-form-id=\"@ViewBag.FormId\">");
				sw.WriteLine();

				var index = 1;
				foreach (var columnInfo in columnInfos)
				{
					if (index != 1)
						sw.WriteLine("    <div class=\"hr-line-dashed\"></div>");

					sw.WriteLine("    <div class=\"form-group\">");
					sw.WriteLine(
						"        <label class=\"col-sm-2 control-label\"><span style=\"color: red\">*&nbsp;</span>{0}</label>",
						columnInfo.Comment);
					sw.WriteLine();
					sw.WriteLine("        <div class=\"col-sm-10\">");
					sw.WriteLine(
						"            <input type=\"text\" id=\"txt-{0}\" name=\"{0}\" class=\"form-control\" placeholder=\"请输入{1}\" />",
						GetCamelVarName(columnInfo.Code), columnInfo.Comment);
					sw.WriteLine("        </div>");
					sw.WriteLine("    </div>");
					sw.WriteLine();

					index++;
				}

				sw.WriteLine("</form>");
				sw.WriteLine();

				#endregion

				#region script

				sw.WriteLine("<script>");
				sw.WriteLine("    $(function () {");
				sw.WriteLine("        var $win = getSubWinBody('@ViewBag.FormId');");
				sw.WriteLine("        var winButtons = $win.data('buttons');");
				sw.WriteLine("        var winEvents = $win.data('winEvents');");
				sw.WriteLine();
				sw.WriteLine("        var validator = $('#@ViewBag.FormId').validate({0}", "{");
				sw.WriteLine("            rules: {");

				index = 1;
				foreach (var columnInfo in columnInfos)
				{
					sw.WriteLine("                {0}: {1}", GetCamelVarName(columnInfo.Code), "{");
					sw.WriteLine("                    required: true,");
					sw.WriteLine("                    maxlength: 20");
					sw.WriteLine("                {0}{1}", "}", columnInfos.Count != index ? "," : "");

					index++;
				}

				sw.WriteLine("            },");
				sw.WriteLine("            messages: {0}", "{");

				index = 1;
				foreach (var columnInfo in columnInfos)
				{
					sw.WriteLine("                {0}: {1}", GetCamelVarName(columnInfo.Code), "{");
					sw.WriteLine("                    required: '请输入{0}',", columnInfo.Comment);
					sw.WriteLine("                    maxlength: '{0}长度不能大于20个字符'", columnInfo.Comment);
					sw.WriteLine("                {0}{1}", "}", columnInfos.Count != index ? "," : "");

					index++;
				}

				sw.WriteLine("            }");
				sw.WriteLine("        });");
				sw.WriteLine();

				sw.WriteLine("        winButtons.save.click = function () {");
				sw.WriteLine("            var result = $('#@ViewBag.FormId').valid();");
				sw.WriteLine();
				sw.WriteLine("            if (result === true) {");
				sw.WriteLine("                var data = $('#@ViewBag.FormId').frmSerialize();");
				sw.WriteLine("                data.Id = '@(Model ?? 0)';");
				sw.WriteLine("                var url = '@(Model.HasValue ? Url.Action(\"Update{0}\") : Url.Action(\"Add{0}\"))';", table.TableName);
				sw.WriteLine();
				sw.WriteLine("                $('.ladda-button-save').attr('data-style', 'expand-left');");
				sw.WriteLine("                var l = $('.ladda-button-save').ladda();");
				sw.WriteLine();
				sw.WriteLine("                l.ladda('start');");
				sw.WriteLine("                bhPost(url, data, l, function () {");
				sw.WriteLine("                    bhSuccess('保存成功');");
				sw.WriteLine("                    winEvents.SavedAndClose();");
				sw.WriteLine("                });");
				sw.WriteLine("            }");
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        if (!!'@Model') {");
				sw.WriteLine("            $('#@ViewBag.FormId').bindData('@Url.Action(\"Get{0}\")', {1});", table.TableName, "{ id: '@Model' }");
				sw.WriteLine("        }");
				sw.WriteLine("    });");
				sw.WriteLine();
				sw.WriteLine("</script>");

				#endregion

				sw.Flush();
			}
		}
	}
}