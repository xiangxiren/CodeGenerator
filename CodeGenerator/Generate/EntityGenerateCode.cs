using System.IO;
using System.Linq;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate
{
    public class EntityGenerateCode : GenerateCodeBase, IGenerateCode
    {
        public void Generate(TableInfo table, string classNamespace, string fileSavePath)
        {
            var formatTableName = table.GetFormatTableName();
            using (var fs = new FileStream(Path.Combine(fileSavePath, formatTableName) + "Entity.cs", FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                #region using

                sw.WriteLine("using System;");
                sw.WriteLine("using JG.Core.Cache;");
                sw.WriteLine("using PetaPoco;");
                sw.WriteLine("using Scm.Component.Common;");
                sw.WriteLine();

                #endregion

                sw.WriteLine("namespace {0}", classNamespace);
                sw.WriteLine("{");
                sw.WriteLine("    /// <summary>");
                sw.WriteLine("    /// {0}", table.Comment);
                sw.WriteLine("    /// </summary>");
                sw.WriteLine("    [Serializable]");
                sw.WriteLine("    [TableName(\"{0}\")]", table.Code);
                sw.WriteLine("    [PrimaryKey(\"{0}\", autoIncrement = false)]", table.GetPrimaryKeyColumnName());
                sw.WriteLine("    [CacheSetting(false, PropertyNameOfDBShard = \"ClientID\", PropertyNameOfCacheShard = \"ClientID\", PropertyNamesOfArea = \"ClientID\", ExpirationPolicy = EntityCacheExpirationPolicies.Stable)]");

                sw.WriteLine("    public class {0}Entity : ScmBaseEntity", formatTableName);
                sw.WriteLine("    {");

                #region New()

                sw.WriteLine("        public static {0}Entity New()", formatTableName);
                sw.WriteLine("        {");
                sw.WriteLine("            var entity = new {0}Entity();", formatTableName);
                sw.WriteLine("            entity.Init();");

                foreach (
                    var columnInfo in
                        table.ColumnInfos.Where(
                            c => !IgnoreColumns.Contains(formatTableName) && c.GetColumnType() == "string"))
                {
                    sw.WriteLine("            entity.{0} = string.Empty;", columnInfo.Code);
                }

                sw.WriteLine();
                sw.WriteLine("            return entity;");
                sw.WriteLine("        }\r\n");

                #endregion

                #region 属性

                sw.WriteLine("        #region 属性");
                foreach (var columnInfo in table.ColumnInfos.Where(c => !IgnoreColumns.Contains(c.Code) && c.Code != table.GetPrimaryKeyColumnName()))
                {
                    sw.WriteLine();
                    sw.WriteLine("        /// <summary>");
                    sw.WriteLine("        /// {0}", columnInfo.Comment);
                    sw.WriteLine("        /// </summary>");
                    sw.WriteLine("        public {0} {1} {2} get; set; {3}", columnInfo.GetColumnType(), columnInfo.Code, "{", "}");
                }
                sw.WriteLine();
                sw.WriteLine("        #endregion\r\n");

                #endregion

                #region 扩展属性

                sw.WriteLine("        #region 扩展属性");
                sw.WriteLine("        #endregion");

                #endregion

                sw.WriteLine("    }");
                sw.Write("}");
                sw.Flush();
            }
        }
    }
}