using System.Collections.Generic;
using System.IO;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate
{
    public class ContextGenerateCode : GenerateCodeBase
    {
        protected override string FileName => ".cs";

        public void Generate(IList<TableInfo> tables, GenerateArgument generateArgument)
        {
            using (var fs = new FileStream(GetFullFilePath(generateArgument.ContextName, generateArgument.ContextFileSavePath), FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                #region using

                sw.WriteLine("using System.Data.Entity;");
                sw.WriteLine("using {0};", generateArgument.ConfigNamespace);
                sw.WriteLine();

                #endregion

                sw.WriteLine("namespace {0}", generateArgument.ContextNamespace);
                sw.WriteLine("{");

                sw.WriteLine("    public partial class {0} : DbContext", generateArgument.ContextName);
                sw.WriteLine("    {");

                #region 构造函数
                sw.WriteLine("        /// <summary>");
                sw.WriteLine("        /// 构造函数");
                sw.WriteLine("        /// </summary>");
                sw.WriteLine("        public {0}(string connStr) : base(connStr)", generateArgument.ContextName);
                sw.WriteLine("        {");
                sw.WriteLine("        }");
                sw.WriteLine();

                #endregion

                #region 属性

                var flag = false;
                foreach (var table in tables)
                {
                    if (flag)
                        sw.WriteLine();
                    sw.WriteLine("        /// <summary>");
                    sw.WriteLine("        /// {0}", table.Comment);
                    sw.WriteLine("        /// </summary>");
                    sw.WriteLine("        public DbSet<{0}> {1} {2} get; set; {3}", table.TableName, GetListPropertyName(table.TableName), "{", "}");

                    if (!flag)
                        flag = true;
                }

                sw.WriteLine();
                sw.WriteLine("        protected override void OnModelCreating(DbModelBuilder modelBuilder)");
                sw.WriteLine("        {");

                flag = false;
                foreach (var table in tables)
                {
                    sw.WriteLine("            modelBuilder.Configurations.Add(new {0}Config());", table.TableName);
                }

                sw.WriteLine("        }");

                #endregion

                sw.WriteLine("    }");
                sw.Write("}");
                sw.Flush();
            }
        }
    }
}