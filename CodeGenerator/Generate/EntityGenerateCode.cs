using System.IO;
using System.Text;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate
{
	public class EntityGenerateCode : GenerateCodeBase, IGenerateCode
	{
		protected override string FileName => ".cs";

		public void Generate(TableInfo table, ArgumentInfo argumentInfo)
		{
			using (var fs = new FileStream(GetFullFilePath(table.TableName, argumentInfo.FileSavePath), FileMode.Create))
			using (var sw = new StreamWriter(fs, Encoding.UTF8))
			{
				#region using

				sw.WriteLine("using System;");
				if (table.ChildTableInfos != null && table.ChildTableInfos.Count > 0)
					sw.WriteLine("using System.Collections.Generic;");
				//				sw.WriteLine("using System.ComponentModel.DataAnnotations;");
				//				sw.WriteLine("using System.ComponentModel.DataAnnotations.Schema;");
				sw.WriteLine();

				#endregion

				sw.WriteLine("namespace {0}", argumentInfo.ClassNamespace);
				sw.WriteLine("{");
				sw.WriteLine("    /// <summary>");
				sw.WriteLine("    /// {0}", table.Comment);
				sw.WriteLine("    /// </summary>");

				//                sw.WriteLine("    [Table(\"{0}\")]", table.TableName);
				sw.WriteLine("    public class {0}", table.TableName);
				sw.WriteLine("    {");

				#region 构造函数

				//                if (table.ChildTableInfos.Count > 0)
				//                {
				//                    sw.WriteLine("        /// <summary>");
				//                    sw.WriteLine("        /// 构造函数");
				//                    sw.WriteLine("        /// </summary>");
				//                    sw.WriteLine("        public {0}()", table.TableName);
				//                    sw.WriteLine("        {");
				//
				//                    foreach (var childTable in table.ChildTableInfos)
				//                    {
				//                        sw.WriteLine("            {0} = new List<{1}>();",
				//                            GetListPropertyName(childTable.ChildPropertyName), childTable.ChildTable.TableName);
				//                    }
				//
				//                    sw.WriteLine("        }");
				//                    sw.WriteLine();
				//                }

				#endregion

				#region 属性

				var flag = false;
				foreach (var columnInfo in table.ColumnInfos)
				{
					if (flag)
						sw.WriteLine();
					sw.WriteLine("        /// <summary>");
					sw.WriteLine("        /// {0}", string.IsNullOrEmpty(columnInfo.Comment) && columnInfo.Code == table.PrimaryKeyCode ? "主键" : columnInfo.Comment);
					sw.WriteLine("        /// </summary>");
					//                    if (columnInfo.Code == table.PrimaryKeyCode)
					//                        sw.WriteLine("        [Key]");

					//                    if (columnInfo.GetColumnType() == "string")
					//                    {
					//                        if (columnInfo.Mandatory)
					//                            sw.WriteLine("        [Required]");
					//
					//                        sw.WriteLine("        [StringLength({0})]", columnInfo.Length);
					//                    }

					sw.WriteLine("        public {0} {1} {2} get; set; {3}", columnInfo.GetColumnType(), columnInfo.Code, "{", "}");

					if (!flag)
						flag = true;
				}

				foreach (var referenceTable in table.ReferenceTableInfos)
				{
					sw.WriteLine();
					sw.WriteLine("        /// <summary>");
					sw.WriteLine("        /// {0}", string.IsNullOrEmpty(referenceTable.ForeignKey.Comment) ? "" : referenceTable.ForeignKey.Comment.ToUpper().Replace("ID", ""));
					sw.WriteLine("        /// </summary>");
					sw.WriteLine("        public virtual {0} {1} {2} get; set; {3}",
						referenceTable.ParentTable.TableName, referenceTable.ParentPropertyName, "{", "}");
				}

				if (table.ChildTableInfos != null)
				{
					foreach (var childTable in table.ChildTableInfos)
					{
						sw.WriteLine();
						sw.WriteLine("        /// <summary>");
						sw.WriteLine("        /// {0}集合", childTable.ChildTable.Comment);
						sw.WriteLine("        /// </summary>");
						sw.WriteLine("        public virtual ICollection<{0}> {1} {2} get; set; {3}",
							childTable.ChildTable.TableName, GetListPropertyName(childTable.ChildPropertyName), "{", "}");
					}
				}

				#endregion

				sw.WriteLine("    }");
				sw.Write("}");
				sw.Flush();
			}
		}
	}
}