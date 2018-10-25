using System.IO;
using System.Linq;
using System.Text;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate.Code
{
	public class ViewIndexGenerateCode : GenerateCodeBase, IGenerateCode
	{
		protected override string FileNameTemplate => "Index.cshtml";

		public GenerateType GenerateType { get; set; } = GenerateType.ViewIndex;

		protected override string GetFullFilePath(string formatTableName, string midllePath, string fileSavePath)
		{
			var folderPath = Path.Combine(fileSavePath, midllePath, formatTableName);

			EnsurePathExist(folderPath);
			return Path.Combine(folderPath, string.Format(FileNameTemplate, formatTableName));
		}

		public void Generate(TableInfo table, GenerateArgument argument)
		{
			using (var fs = new FileStream(GetFullFilePath(table.TableName, GenerateArgument.ViewFilePath, argument.FilePath), FileMode.Create))
			using (var sw = new StreamWriter(fs, Encoding.UTF8))
			{
				#region html

				sw.WriteLine("<div class=\"row wrapper border-bottom white-bg page-heading\">");
				sw.WriteLine("    <div class=\"col-lg-10\">");
				sw.WriteLine("        <ol class=\"breadcrumb\">");
				sw.WriteLine("            <li>");
				sw.WriteLine("                {0}管理", table.Comment);
				sw.WriteLine("            </li>");
				sw.WriteLine("            <li class=\"active\">");
				sw.WriteLine("                <strong>{0}管理</strong>", table.Comment);
				sw.WriteLine("            </li>");
				sw.WriteLine("        </ol>");
				sw.WriteLine("    </div>");
				sw.WriteLine("</div>");
				sw.WriteLine();

				sw.WriteLine("<div class=\"wrapper wrapper-content animated\" id=\"@ViewBag.FormId\" data-form-id=\"@ViewBag.FormId\">");
				sw.WriteLine("    <div class=\"row\">");
				sw.WriteLine("        <div class=\"col-xs-12\">");
				sw.WriteLine("            <div class=\"panel panel-default\" id=\"search-parms\">");
				sw.WriteLine("                <div class=\"panel-heading accordion-inner\">");
				sw.WriteLine("                    <div class=\"filters col-sm-10 col-xs-12\">");
				sw.WriteLine("                        <form class=\"form-horizontal\" id=\"form_filter\">");
				sw.WriteLine("                            <div class=\"col-xs-12\">");
				sw.WriteLine("                                <label class=\"filter-caption col-md-4 input-sm\" for=\"txt_filter\"> 关键字</label>");
				sw.WriteLine("                                <div class=\"filter-input col-md-8\">");
				sw.WriteLine("                                    <input type=\"text\" id=\"txt_filter\" name=\"filter\" class=\"form-control input-sm\" placeholder=\"名称/描述\" />");
				sw.WriteLine("                                </div>");
				sw.WriteLine("                            </div>");
				sw.WriteLine("                        </form>");
				sw.WriteLine("                    </div>");
				sw.WriteLine("                    <div class=\"col-sm-2 col-xs-12 pull-right filter-querybar\">");
				sw.WriteLine("                        <button class=\"btn btn-primary btn-sm pull-right\" id=\"btn-search\" data-click-name=\"search\">");
				sw.WriteLine("                            <i class=\"fa fa-search\">查询</i>");
				sw.WriteLine("                        </button>");
				sw.WriteLine("                    </div>");
				sw.WriteLine("                    <div class=\"clearfix\"></div>");
				sw.WriteLine("                </div>");
				sw.WriteLine("            </div>");
				sw.WriteLine("        </div>");
				sw.WriteLine("        <div class=\"col-xs-12\">");
				sw.WriteLine("            <table id=\"tb_{0}s\" class=\"bhtable\"></table>", GetCamelVarName(table.TableName));
				sw.WriteLine("        </div>");
				sw.WriteLine("    </div>");
				sw.WriteLine("    <div id=\"toolbar\" class=\"btn-group\">");
				sw.WriteLine("        @if (ViewBag.AllowAdd ?? false)");
				sw.WriteLine("        {");
				sw.WriteLine("            <button id=\"btn_add\" type=\"button\" class=\"btn btn-default\" data-click-name=\"add\">");
				sw.WriteLine("                <span class=\"glyphicon glyphicon-plus\" aria-hidden=\"true\"></span>新增");
				sw.WriteLine("            </button>");
				sw.WriteLine("        }");
				sw.WriteLine("    </div>");
				sw.WriteLine("</div>");
				sw.WriteLine();

				#endregion

				#region script

				sw.WriteLine("<script>");
				sw.WriteLine("    $(function () {");
				sw.WriteLine("        var $win = $('#@ViewBag.FormId');");
				sw.WriteLine("        $('.datepicker', $win).datepicker({");
				sw.WriteLine("            format: 'yyyy/mm/d'");
				sw.WriteLine("        });");
				sw.WriteLine();
				sw.WriteLine("        $('#tb_{0}s').bootstrapTable({1}", GetCamelVarName(table.TableName), "{");
				sw.WriteLine("            url: '@Url.Action(\"Get{0}s\")',", table.TableName);
				sw.WriteLine("            columns: [");

				foreach (var columnInfo in table.ColumnInfos.Where(t => t.Code != table.PrimaryKeyCode))
				{
					sw.WriteLine("                {0} field: '{1}', title: '{2}', sortable: true, {3}{4}", "{",
						GetCamelVarName(columnInfo.Code), columnInfo.Comment,
						IsAlignCenter(columnInfo) ? "align: 'center', " : "", "}");
				}

				sw.WriteLine("                {");
				sw.WriteLine("                    field: '{0}',", GetCamelVarName(table.PrimaryKeyCode));
				sw.WriteLine("                    title: '操作',");
				sw.WriteLine("                    cardVisible: false,");
				sw.WriteLine("                    align: 'center',");
				sw.WriteLine("                    formatter: function(data, row, index) {");
				sw.WriteLine("                        var sOut = '<div>';");
				sw.WriteLine();
				sw.WriteLine("                        var edit = '@ViewBag.AllowEdit'.toLowerCase();");
				sw.WriteLine("                        if (edit === 'true') {");
				sw.WriteLine("                            sOut += '<button class=\"btn btn-primary  btn-xs\" data-click-name=\"edit\" data-id=\"' + data + '\"><i class=\"glyphicon glyphicon-edit\"></i>编辑</button>';");
				sw.WriteLine("                        }");
				sw.WriteLine();
				sw.WriteLine("                        var del = '@ViewBag.AllowDelete'.toLowerCase();");
				sw.WriteLine("                        if (del === 'true') {");
				sw.WriteLine("                            sOut += '<button class=\"btn btn-danger btn-xs\" data-click-name=\"delete\" data-id=\"' + data + '\"><i class=\"glyphicon glyphicon-remove\"></i>删除</button>';");
				sw.WriteLine("                        }");
				sw.WriteLine();
				sw.WriteLine("                        sOut += '</div>';");
				sw.WriteLine("                        return sOut;");
				sw.WriteLine("                    }");
				sw.WriteLine("                }");
				sw.WriteLine("            ]");
				sw.WriteLine("        });");
				sw.WriteLine();
				sw.WriteLine("        function refreshTable() {");
				sw.WriteLine("            $('#tb_{0}s', $win).bootstrapTable('refresh');", GetCamelVarName(table.TableName));
				sw.WriteLine("        }");
				sw.WriteLine();
				sw.WriteLine("        $win.on('click', 'button, a', function() {");
				sw.WriteLine("            if ($(this).has('data-click-name')) {");
				sw.WriteLine("                var clickName = $(this).attr('data-click-name');");
				sw.WriteLine("                var id = $(this).attr('data-id');");
				sw.WriteLine();
				sw.WriteLine("                switch (clickName) {");
				sw.WriteLine("                    case 'search':");
				sw.WriteLine("                        refreshTable();");
				sw.WriteLine("                        break;");
				sw.WriteLine("                    case 'add':");
				sw.WriteLine("                        openDialog('@Url.Action(\"Edit{0}\")', '新增{1}', 700, 'Save', [], refreshTable);", table.TableName, table.Comment);
				sw.WriteLine("                        break;");
				sw.WriteLine("                    case 'edit':");
				sw.WriteLine("                        openDialog('@Url.Action(\"Edit{0}\")?id=' + id, '编辑{1}', 700, 'Save', [], refreshTable);", table.TableName, table.Comment);
				sw.WriteLine("                        break;");
				sw.WriteLine("                    case 'delete':");
				sw.WriteLine("                        bhConfirm({");
				sw.WriteLine("                            'title': '提示',");
				sw.WriteLine("                            'text': '确认删除该{0}?',", table.Comment);
				sw.WriteLine("                            'parms': { Id: id },");
				sw.WriteLine("                            'fnConfirmCallback': function (parms) {");
				sw.WriteLine("                                var url = '@Url.Action(\"Delete{0}\")?id=' + parms.Id;", table.TableName);
				sw.WriteLine("                                bhPost(url, {}, null, function () {");
				sw.WriteLine("                                    bhSuccess('删除成功');");
				sw.WriteLine("                                    refreshTable();");
				sw.WriteLine("                                });");
				sw.WriteLine("                            }");
				sw.WriteLine("                        });");
				sw.WriteLine("                        break;");
				sw.WriteLine("                }");
				sw.WriteLine("            }");
				sw.WriteLine("        });");
				sw.WriteLine("    });");
				sw.WriteLine();
				sw.WriteLine("</script>");

				#endregion

				sw.Flush();
			}
		}

		private bool IsAlignCenter(ColumnInfo columnInfo)
		{
			var type = columnInfo.GetColumnType();

			return type.Contains("int") || type.Contains("short") || type.Contains("long") ||
				   type.Contains("decimal") || type.Contains("double") || type.Contains("DateTime");
		}
	}
}