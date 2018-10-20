using System.IO;
using System.Linq;
using System.Text;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate.Code
{
	[CodeGenerator(GenerateType.Config)]
	public class ConfigGenerateCode : GenerateCodeBase, IGenerateCode
	{
		protected override string FileNameTemplate => "{0}Configuration.cs";

		public void Generate(TableInfo table, GenerateArgument argument)
		{
			using (var fs = new FileStream(GetFullFilePath(table.TableName, GenerateArgument.ConfigFilePath, argument.FilePath), FileMode.Create))
			using (var sw = new StreamWriter(fs, Encoding.UTF8))
			{
				#region using

				sw.WriteLine("using {0}.Data.Entity;", argument.ProjectName);
				sw.WriteLine("using Microsoft.EntityFrameworkCore;");
				sw.WriteLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
				sw.WriteLine();

				#endregion

				sw.WriteLine("namespace {0}.{1}", argument.ProjectName, GenerateArgument.ConfigNamespaceTemp);
				sw.WriteLine("{");

				sw.WriteLine("    /// <inheritdoc />");
				sw.WriteLine("    internal class {0}Configuration : IEntityTypeConfiguration<{0}>", table.TableName);
				sw.WriteLine("    {");

				sw.WriteLine("        /// <inheritdoc />");
				sw.WriteLine("        public void Configure(EntityTypeBuilder<{0}> builder)", table.TableName);
				sw.WriteLine("        {");
				sw.WriteLine("            builder.ToTable(\"{0}\");", table.TableName);
				sw.WriteLine("            builder.HasKey(t => t.{0});", table.PrimaryKeyCode);

				var properties =
					table.ColumnInfos.Where(t => t.DataType.ToUpper().Contains("TEXT") ||
												 t.DataType.ToUpper().Contains("VARCHAR") ||
												 t.DataType.ToUpper().Contains("NUMERIC"));
				if (properties.Any())
				{
					sw.WriteLine();
					sw.WriteLine("            // Properties");
					foreach (var info in table.ColumnInfos)
					{
						if (info.DataType.ToUpper().Contains("TEXT") && info.Mandatory)
						{
							sw.WriteLine("			builder.Property(t => t.{0}).IsRequired();", info.Code);
							continue;
						}

						var requiredStr = info.Mandatory ? ".IsRequired()" : "";

						if (info.DataType.ToUpper().Contains("VARCHAR"))
						{
							if (info.Length > 0 || info.Mandatory)
							{
								if (info.Length == 0)
								{
									if (info.Mandatory)
										sw.WriteLine("			builder.Property(t => t.{0}).IsRequired();", info.Code);
								}
								else
								{
									sw.WriteLine("			builder.Property(t => t.{0}){2}.HasMaxLength({1});",
										info.Code, info.Length, requiredStr);
								}
							}
							continue;
						}
						if (info.DataType.ToUpper().Contains("NUMERIC") || info.DataType.ToUpper().Contains("DECIMAL"))
						{
							sw.WriteLine(
								"			builder.Property(t => t.{0}){3}.HasColumnType(\"decimal({1}, {2})\");",
								info.Code, info.Length, info.Precision, requiredStr);
						}
					}
				}

				//				sw.WriteLine("            // Table & Column Mappings");

				//                foreach (var column in table.ColumnInfos)
				//                {
				//                    sw.WriteLine("            Property(t => t.{0}).HasColumnName(\"{0}\");", column.Code);
				//                }

				if (table.ReferenceTableInfos.Count > 0)
				{
					sw.WriteLine();
					sw.WriteLine("            // Relationships");

					var flag = false;
					foreach (var reference in table.ReferenceTableInfos)
					{
						if (flag)
							sw.WriteLine();

						var childTableInfo =
							reference.ParentTable.ChildTableInfos.FirstOrDefault(
								t => t.ChildTable.Id == table.Id && t.ForeignKey.Id == reference.ForeignKey.Id);
						if (childTableInfo == null) continue;

						sw.WriteLine("            builder.{0}(t => t.{1})", "HasOne", reference.ParentPropertyName);
						sw.WriteLine("                .WithMany(t => t.{0})", GetListPropertyName(childTableInfo.ChildPropertyName));
						sw.WriteLine("                .HasForeignKey(d => d.{0});", reference.ForeignKey.Code);

						flag = true;
					}
				}

				sw.WriteLine("        }");
				sw.WriteLine("    }");
				sw.Write("}");
				//sw.WriteLine();

				sw.Flush();
			}
		}
	}
}