using System.IO;
using CodeGenerator.Pdm;

namespace CodeGenerator.Generate
{
    public class EntityGenerateCode : GenerateCodeBase, IGenerateCode
    {
        protected override string FileName => ".cs";

        public void Generate(TableInfo table, string classNamespace, string fileSavePath)
        {
            var formatTableName = table.GetFormatTableName();
            using (var fs = new FileStream(GetFullFilePath(formatTableName, fileSavePath), FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                #region using

                sw.WriteLine("using System;");
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine();

                #endregion

                sw.WriteLine("namespace {0}", classNamespace);
                sw.WriteLine("{");
                sw.WriteLine("    /// <summary>");
                sw.WriteLine("    /// {0}", table.Comment);
                sw.WriteLine("    /// </summary>");

                sw.WriteLine("    public class {0}", formatTableName);
                sw.WriteLine("    {");

                #region 构造函数

                if (table.ChildTableInfos.Count > 0)
                {
                    sw.WriteLine("        /// <summary>");
                    sw.WriteLine("        /// 构造函数");
                    sw.WriteLine("        /// </summary>");
                    sw.WriteLine("        public {0}()", formatTableName);
                    sw.WriteLine("        {");

                    foreach (var childTable in table.ChildTableInfos)
                    {
                        var propertyName = childTable.ChildTable.GetFormatTableName();
                        var foreignKey = childTable.ForeignKey.Code.Substring(0, childTable.ForeignKey.Code.Length - 2);
                        if (foreignKey != table.GetFormatTableName())
                            propertyName = foreignKey + propertyName;

                        sw.WriteLine("            {0} = new List<{1}>();", GetListPropertyName(propertyName), childTable.ChildTable.GetFormatTableName());
                    }

                    sw.WriteLine("        }");
                    sw.WriteLine();
                }

                #endregion

                #region 属性

                var flag = false;
                foreach (var columnInfo in table.ColumnInfos)
                {
                    if (flag)
                        sw.WriteLine();
                    sw.WriteLine("        /// <summary>");
                    sw.WriteLine("        /// {0}", columnInfo.Comment);
                    sw.WriteLine("        /// </summary>");
                    sw.WriteLine("        public {0} {1} {2} get; set; {3}", columnInfo.GetColumnType(), columnInfo.Code, "{", "}");

                    if (!flag)
                        flag = true;
                }

                foreach (var referenceTable in table.ReferenceTableInfos)
                {
                    var propertyName = referenceTable.ForeignKey.Code.Substring(0, referenceTable.ForeignKey.Code.Length - 2);
                    if (propertyName != referenceTable.ParentTable.Code)
                        propertyName += referenceTable.ParentTable.Code;

                    sw.WriteLine();
                    sw.WriteLine("        /// <summary>");
                    sw.WriteLine("        /// {0}", referenceTable.ForeignKey.Comment);
                    sw.WriteLine("        /// </summary>");
                    sw.WriteLine("        public virtual {0} {1} {2} get; set; {3}", referenceTable.ParentTable.GetFormatTableName(), propertyName, "{", "}");
                }

                foreach (var childTable in table.ChildTableInfos)
                {
                    var propertyName = childTable.ChildTable.Code;
                    var foreignKey = childTable.ForeignKey.Code.Substring(0, childTable.ForeignKey.Code.Length - 2);
                    if (foreignKey != table.Code)
                        propertyName = foreignKey + propertyName;

                    sw.WriteLine();
                    sw.WriteLine("        public virtual ICollection<{0}> {1} {2} get; set; {3}", childTable.ChildTable.GetFormatTableName(), GetListPropertyName(propertyName), "{", "}");
                }

                #endregion

                sw.WriteLine("    }");
                sw.Write("}");
                sw.Flush();
            }
        }
    }
}