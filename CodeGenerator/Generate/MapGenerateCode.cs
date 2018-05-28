using System.IO;
using System.Linq;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate
{
    public class MapGenerateCode : GenerateCodeBase, IGenerateCode
    {
        protected override string FileName => "Map.cs";

        public void Generate(TableInfo table, string classNamespace, string fileSavePath)
        {
            using (var fs = new FileStream(GetFullFilePath(table.TableName, fileSavePath), FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                #region using

                sw.WriteLine("using System.Data.Entity.ModelConfiguration;");
                sw.WriteLine();

                #endregion

                sw.WriteLine("namespace {0}", classNamespace);
                sw.WriteLine("{");

                sw.WriteLine("    public class {0}Map : EntityTypeConfiguration<{0}>", table.TableName);
                sw.WriteLine("    {");

                sw.WriteLine("        public {0}Map()", table.TableName);
                sw.WriteLine("        {");
                sw.WriteLine("            // Primary Key");
                sw.WriteLine("            HasKey(t => t.{0});", table.PrimaryKeyCode);
                sw.WriteLine();

                var properties =
                    table.ColumnInfos.Where(t => t.DataType.ToUpper().Contains("TEXT") ||
                                                 t.DataType.ToUpper().Contains("VARCHAR") ||
                                                 t.DataType.ToUpper().Contains("NUMERIC"));
                if (properties.Any())
                {
                    sw.WriteLine("            // Properties");
                    foreach (var info in table.ColumnInfos)
                    {
                        if (info.DataType.ToUpper().Contains("TEXT") && info.Mandatory)
                        {
                            sw.WriteLine("            Property(t => t.{0})", info.Code);
                            sw.WriteLine("                .IsRequired();");
                            sw.WriteLine();
                            continue;
                        }
                        if (info.DataType.ToUpper().Contains("VARCHAR"))
                        {
                            if (info.Length > 0 || info.Mandatory)
                            {
                                sw.WriteLine("            Property(t => t.{0})", info.Code);
                                if (info.Length == 0)
                                {
                                    if (info.Mandatory)
                                        sw.WriteLine("                .IsRequired();");
                                }
                                else
                                {
                                    if (info.Mandatory)
                                        sw.WriteLine("                .IsRequired()");
                                    sw.WriteLine("                .HasMaxLength({0});", info.Length);
                                }
                                sw.WriteLine();
                            }
                            continue;
                        }
                        if (info.DataType.ToUpper().Contains("NUMERIC"))
                        {
                            sw.WriteLine("            Property(t => t.{0})", info.Code);
                            if (info.Mandatory)
                                sw.WriteLine("                .IsRequired()");
                            sw.WriteLine("                .HasPrecision({0}, {1});", info.Length, info.Precision);
                            sw.WriteLine();
                        }
                    }
                }

                sw.WriteLine("            // Table & Column Mappings");
                sw.WriteLine("            ToTable(\"{0}\");", table.TableName);

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

                        sw.WriteLine("            {0}(t => t.{1})", reference.ForeignKey.Mandatory ? "HasRequired" : "HasOptional", reference.ParentPropertyName);
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